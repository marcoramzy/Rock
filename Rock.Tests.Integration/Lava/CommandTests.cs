// <copyright>
// Copyright by the Spark Development Network
//
// Licensed under the Rock Community License (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.rockrms.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//



//using Rock.Data;
//using Rock.Model;
//using Rock;
//using System;

//using System.Linq;
//using System.Net;
//using System.Security.Authentication;
using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json.Linq;
//using RestSharp;
//using Rock.Lava;

//using Rock.Web.Cache;

using System;
using Rock;
using Rock.Model;
using Rock.Data;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Authentication;

namespace Rock.Tests.Integration.Lava
{
    /// <summary>
    /// Tests for Lava-specific commands implemented as Liquid custom blocks and tags.
    /// </summary>
    [TestClass]
    public class CommandTests
    {
        private static LavaTestHelper _helper;

        #region Constructors

        [ClassInitialize]
        public static void Initialize( TestContext context )
        {
            _helper = LavaTestHelper.NewForDotLiquidProcessor();
        }

        #endregion

        #region Cache

        [TestMethod]
        public void CacheBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% cache key:'decker-page-list' duration:'3600' %}
    {% person where:'LastName == ""Decker""' %}
        {% for person in personItems %}
            {{ person.FullName }} < br />
        {% endfor %}
    {% endperson %}
{% endcache %}
";

            var expectedOutput = @"\s*The Lava command 'Cache' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void CacheBlock_ForEntityCommandResult_IsCached()
        {
            var input = @"
{% cache key:'decker-page-list' duration:'3600' %}
    {% person where:'LastName == ""Decker""' %}
        {% for person in personItems %}
            {{ person.FullName }} < br />
        {% endfor %}
    {% endperson %}
{% endcache %}
";

            var expectedOutput = @"
";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "Cache,RockEntity" );

            _helper.AssertTemplateOutput( expectedOutput, input, context, ignoreWhitespace: true );
        }

        [TestMethod]
        public void CacheBlock_WithInnerVariable_DoesNotModifyOuterVariable()
        {
            var input = @"
{% assign color = 'blue' %}
Color 1: {{ color }}

{% cache key:'fav-color' duration:'1200' %}
    Color 2: {{ color }}
    {% assign color = 'red' %}
    Color 3: {{color }}
{% endcache %}

Color 4: {{ color }}
";

            var expectedOutput = @"
Color 1: blue
Color 2: blue
Color 3: red
Color 4: blue
";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "Cache" );

            _helper.AssertTemplateOutput( expectedOutput, input, context, ignoreWhitespace: true );
        }

        #endregion

        #region Entity

        [TestMethod]
        public void EntityBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% person where: 'LastName == ""Decker""' %}
    {% for person in personItems %}
        {{ person.FullName }} < br />
    {% endfor %}
{% endperson %}
            ";

            var expectedOutput = @"\s*The Lava command 'RockEntity' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void EntityBlock_PersonWhereLastNameIsDecker_ReturnsDeckers()
        {
            var input = @"
{% person where:'LastName == ""Decker""' %}
    {% for person in personItems %}
        {{ person.FullName }} <br/>
    {% endfor %}
{% endperson %}
            ";

            var expectedOutput = @"";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "RockEntity" );

            _helper.AssertTemplateOutput( expectedOutput, input, context );
        }

        #endregion

        #region Execute

        [TestMethod]
        public void ExecuteBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% execute %}
    return ""Hello World!"";
{% endexecute %}
            ";

            var expectedOutput = @"\s*The Lava command 'Execute' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void ExecuteBlock_HelloWorld_ReturnsExpectedOutput()
        {
            var input = @"
{% execute %}
    return ""Hello World!"";
{% endexecute %}
            ";

            var expectedOutput = @"Hello World!";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "execute" );

            _helper.AssertTemplateOutput( expectedOutput, input, context );
        }

        [TestMethod]
        public void ExecuteBlock_WithImports_ReturnsExpectedOutput()
        {
            var client = new RestClient( "http://api.github.com" );
            var request = new RestRequest( "repos/SparkDevNetwork/Rock/commits", Method.GET );

            const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
            const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
            ServicePointManager.SecurityProtocol = Tls12;

            //ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

            IRestResponse response = client.Execute( request );

            JArray commitArray = JArray.Parse( response.Content );

            dynamic firstCommit = commitArray.First["commit"];

            var message = firstCommit.message + "<br/>" + firstCommit.author.name;

            // Get the most recent commits from the Rock GitHub repository.

            // https://api.github.com/repos/SparkDevNetwork/Rock/commits?page=917
            var input = @"
{% execute import:'RestSharp,Newtonsoft.Json,Newtonsoft.Json.Linq,System.Net,System.Security.Authentication' %}

const SslProtocols _Tls12 = (SslProtocols)0x00000C00;
const SecurityProtocolType Tls12 = (SecurityProtocolType)_Tls12;
ServicePointManager.SecurityProtocol = Tls12;

//ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;

var client = new RestClient( ""https://api.github.com"" );
var request = new RestRequest( ""repos/SparkDevNetwork/Rock/commits"", Method.GET );

IRestResponse response = client.Execute( request );

JArray commitArray = JArray.Parse( response.Content );

dynamic firstCommit = commitArray.First[""commit""];
return firstCommit.message + ""<br />"" + firstCommit.author.name;

{% endexecute %}
";

            var expectedOutput = @"Hello World!";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "execute" );

            var output = _helper.GetTemplateOutput( input, context );
        }

        [TestMethod]
        public void ExecuteBlock_ClassType_ReturnsExpectedOutput()
        {
            var input = @"
{% execute type:'class' %}
    using Rock;
    using Rock.Data;
    using Rock.Model;
    
    public class MyScript 
    {
        public string Execute() {
            using(RockContext rockContext = new RockContext()){
                var person = new PersonService(rockContext).Get({{ Person | Property: 'Id' }});
                
                return person.FullName;
            }
        }
    }
{% endexecute %}
";

            var expectedOutput = @"Ted Decker";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "execute" );

            context.SetMergeFieldValue( "Person", _helper.GetTestPersonTedDecker() );

            _helper.AssertTemplateOutput( expectedOutput, input, context );
        }
        #endregion

        #region InteractionWrite

        [TestMethod]
        public void InteractionWriteBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% interactionwrite channeltypemediumvalueid:'1' channelentityid:'1' channelname:'Some Channel' componententitytypeid:'1' interactionentitytypeid:'1' componententityid:'1' componentname:'Some Component' entityid:'1' operation:'View' summary:'Viewed Some Page' relatedentitytypeid:'1' relatedentityid:'1' channelcustom1:'Some Custom Value' channelcustom2:'Another Custom Value' channelcustomindexed1:'Some Indexed Custom Value'  personaliasid:'10' %}
    Here is the interaction data.
{% endinteractionwrite %}
";

            var expectedOutput = @"\s*The Lava command 'InteractionWrite' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void InteractionWriteBlock_ForEntityCommandResult_IsCached()
        {
            var input = @"
{% interactionwrite channeltypemediumvalueid:'1' channelentityid:'1' channelname:'Some Channel' componententitytypeid:'1' interactionentitytypeid:'1' componententityid:'1' componentname:'Some Component' entityid:'1' operation:'View' summary:'Viewed Some Page' relatedentitytypeid:'1' relatedentityid:'1' channelcustom1:'Some Custom Value' channelcustom2:'Another Custom Value' channelcustomindexed1:'Some Indexed Custom Value'  personaliasid:'10' %}
    Here is the interaction data.
{% endinteractionwrite %}
";

            var expectedOutput = @"
";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "InteractionWrite" );

            _helper.AssertTemplateOutput( expectedOutput, input, context, ignoreWhitespace: true );
        }

        #endregion

        #region InteractionContentChannelItemWrite

        [TestMethod]
        public void InteractionContentChannelItemWriteBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% interactioncontentchannelitemwrite contentchannelitemid:'1' operation:'View' summary:'Viewed content channel item #1' personaliasid:'10' %}
";

            var expectedOutput = @"\s*The Lava command 'InteractionContentChannelItemWrite' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void InteractionContentChannelItemWriteBlock_ForEntityCommandResult_IsCached()
        {
            var input = @"
{% interactioncontentchannelitemwrite contentchannelitemid:'1' operation:'View' summary:'Viewed content channel item #1' personaliasid:'10' %}
";

            var expectedOutput = @"
";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "Cache,RockEntity" );

            _helper.AssertTemplateOutput( expectedOutput, input, context, ignoreWhitespace: true );
        }

        #endregion

        #region Javascript

        [TestMethod]
        public void JavascriptBlock_HelloWorld_ReturnsJavascriptScript()
        {
            var input = @"
{% javascript %}
    alert('Hello world!');
{% endjavascript %}
";

            var expectedOutput = @"
<script>
    (function(){
        alert('Hello world!');    
    })();
</script>
";

            _helper.AssertTemplateOutput( expectedOutput, input, ignoreWhitespace: true );
        }

        #endregion

        #region Search

        [TestMethod]
        public void SearchBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% search query: 'ted decker' %}
    {% for result in results %}
        {{ result.DocumentName }}
    {% endfor %}
{% endsearch %}
";

            var expectedOutput = @"\s*The Lava command 'Search' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void SearchBlock_UniversalSearchNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% search query:'ted decker' %}
    {% for result in results %}
        {{ result.DocumentName }}
    {% endfor %}
{% endsearch %}
";

            var expectedOutput = @"
Liquid error: Search results not available. Universal search is not enabled for this Rock instance.
";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "Search" );

            _helper.AssertTemplateOutput( expectedOutput, input, context, ignoreWhitespace: true );
        }

        #endregion Search

        #region SQL

        [TestMethod]
        public void SqlBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% sql %}
    SELECT   [NickName], [LastName]
    FROM     [Person] 
    WHERE    [LastName] = 'Decker'
    AND      [NickName] IN ('Ted', 'Alex')
    ORDER BY [NickName]
{% endsql %}
";

            var expectedOutput = @"\s*The Lava command 'Sql' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void SqlBlock_PersonWhereLastNameIsDecker_ReturnsDeckers()
        {
            var input = @"
{% sql %}
    SELECT   [NickName], [LastName]
    FROM     [Person] 
    WHERE    [LastName] = 'Decker'
    AND      [NickName] IN ('Ted', 'Alex')
    ORDER BY [NickName]
{% endsql %}

{% for item in results %}{{ item.NickName }} {{ item.LastName }}; {% endfor %}
";

            var expectedOutput = @"\s*Alex Decker; Ted Decker\s*";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "Sql" );

            _helper.AssertTemplateOutputRegex( expectedOutput, input, context );
        }

        #endregion

        #region Stylesheet

        [TestMethod]
        public void StylesheetBlock_HelloWorld_ReturnsJavascriptScript()
        {
            var input = @"
{% stylesheet %}
#content-wrapper {
    background-color: red !important;
    color: #fff;
}
{% endstylesheet %}
";

            var expectedOutput = @"
<style>
    #content-wrapper {background-color:red!important;color:#fff;}
</style> 
";

            _helper.AssertTemplateOutput( expectedOutput, input, ignoreWhitespace: true );
        }

        #endregion

        #region Web Request

        [TestMethod]
        public void WebRequestBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% webrequest url:'https://api.github.com/repos/SparkDevNetwork/Rock/commits' %}
    <ul>
    {% for item in results %}
	    <li>
            <strong>{{ item.commit.author.name }}</strong><br />
            {{ item.commit.message }}
        </li>
    {% endfor %}
    </ul>
{% endwebrequest %}
";

            var expectedOutput = @"\s*The Lava command 'WebRequest' is not configured for this template\.\s*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void WebRequestBlock_RockRepoCommits_ReturnsCommits()
        {
            var input = @"
{% webrequest url:'https://api.github.com/repos/SparkDevNetwork/Rock/commits' %}
    <ul>
    {% for item in results %}
	    <li>
            <strong>{{ item.commit.author.name }}</strong><br />
            {{ item.commit.message }}
        </li>
    {% endfor %}
    </ul>
{% endwebrequest %}
";

            var expectedOutput = @"??";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "WebRequest" );

            _helper.AssertTemplateOutputRegex( expectedOutput, input, context );
        }

        #endregion

        #region WorkflowActivate

        [TestMethod]
        public void WorkflowActivateBlock_CommandNotEnabled_ReturnsConfigurationErrorMessage()
        {
            var input = @"
{% workflowactivate workflowtype:'8fedc6ee-8630-41ed-9fc5-c7157fd1eaa4' %}
  Activated new workflow with the id of #{{ Workflow.Id }}.
{% endworkflowactivate %}
";

            // TODO: If the security check fails, the content of the block is still returned with the erro message.
            // Is this correct behavior, or should the content of the block be hidden?
            var expectedOutput = @"\s*The Lava command 'WorkflowActivate' is not configured for this template\.\s*.*";

            _helper.AssertTemplateOutputRegex( expectedOutput, input );
        }

        [TestMethod]
        public void WorkflowActivateBlock_ActivateSupportWorkflow_CreatesNewWorkflow()
        {
            // Activate Workflow: IT Support
            var input = @"
{% workflowactivate workflowtype:'51FE9641-FB8F-41BF-B09E-235900C3E53E' %}
  Activated new workflow with the name '{{ Workflow.Name }}'.
{% endworkflowactivate %}
";

            var expectedOutput = @"Activated new workflow with the name ''.";

            var context = _helper.LavaEngine.NewContext();

            context.SetEnabledCommands( "WorkflowActivate" );

            _helper.AssertTemplateOutputRegex( expectedOutput, input, context );
        }

        #endregion



    }
}
