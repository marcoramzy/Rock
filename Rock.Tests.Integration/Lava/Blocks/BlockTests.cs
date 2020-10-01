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
using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Lava;
using Rock.Model;
using Rock.Web.Cache;

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


        #endregion
    }
}
