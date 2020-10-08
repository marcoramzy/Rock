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
    [TestClass]
    public class BlockTests
    {
        private static LavaTestHelper _helper;

        #region Constructors

        [ClassInitialize]
        public static void Initialize( TestContext context )
        {
            _helper = LavaTestHelper.NewForDotLiquidProcessor();
        }

        #endregion

        #region Entity

        /// <summary>
        /// Using the Scripturize shortcode produces the expected output.
        /// </summary>
        [TestMethod]
        public void Entity_WhereLastNameIsDecker_ReturnsDeckers()
        {
            var input = @"
{% person where: 'LastName == ""Decker""' %}
    {% for person in personItems %}
        {{ person.FullName }} < br />
    {% endfor %}
{% endperson %}
            ";

            var expectedOutput = @"";

            _helper.AssertTemplateOutput( expectedOutput, input );
        }

        #endregion

        #region Execute

        /// <summary>
        /// Using the Scripturize shortcode produces the expected output.
        /// </summary>
        [TestMethod]
        public void Execute_HelloWorld_ReturnsExpectedOutput()
        {
            var input = @"
{% execute %}
    return ""Hello World!"";
{% endexecute %}
            ";

            var expectedOutput = @"Hello World!";

            var context = _helper.LavaEngine.NewContext();

            context.EnabledCommands.Add( "execute" );

            _helper.AssertTemplateOutput( expectedOutput, input, context );
        }

        [TestMethod]
        public void Execute_WithImports_ReturnsExpectedOutput()
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

            context.EnabledCommands.Add( "execute" );

            var output = _helper.GetTemplateOutput( input, context );
        }

        [TestMethod]
        public void Execute_WithImports_ReturnsExpectedOutput()
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
{% execute type:'class' %}
    using Rock;
    using Rock.Data;
    using Rock.Model;
    
    public class MyScript 
    {
        public string Execute() {
            using(RockContext rockContext = new RockContext()){
                var person = new PersonService(rockContext).Get({{ CurrentPerson.Id }});
                
                return person.FullName;
            }
        }
    }
{% endexecute %}
";

            var expectedOutput = @"Hello World!";

            var context = _helper.LavaEngine.NewContext();

            context.EnabledCommands.Add( "execute" );

            context.SetValue("CurrentPerson", LavaTestHelper. CoreModuleTestHelper.)
            var output = _helper.GetTemplateOutput( input, context );
        }
        #endregion
    }
}
