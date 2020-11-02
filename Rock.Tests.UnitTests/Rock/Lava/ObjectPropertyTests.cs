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
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Lava;
using Rock.Tests.Shared;

namespace Rock.Tests.UnitTests.Lava
{
    [TestClass]
    public class ObjectPropertyTests
    {
        private static LavaTestHelper _helper;

        #region Constructors

        [ClassInitialize]
        public static void Initialize( TestContext context )
        {
            _helper = LavaTestHelper.NewForCurrentProcessor();

            _helper.LavaEngine.RegisterSafeType( typeof( TestPerson ) );
            _helper.LavaEngine.RegisterSafeType( typeof( TestCampus ) );

        }

        #endregion

        /// <summary>
        /// Referencing a valid property of an input object should return the property value.
        /// </summary>
        [TestMethod]
        public void ObjectProperty_DotNotationPropertyAccess_ReturnsPropertyValue()
        {
            System.Diagnostics.Debug.Print( _helper.GetTestPersonTedDecker().ToString() );

            var mergeValues = new LavaDictionary { { "CurrentPerson", _helper.GetTestPersonTedDecker() } };

            _helper.AssertTemplateOutput( "Decker", "{{ CurrentPerson.LastName }}", mergeValues );
        }

        /// <summary>
        /// Accessing a nested property using dot-notation "Campus.Name" should return the correct value.
        /// </summary>
        [TestMethod]
        public void ObjectProperty_DotNotationNestedPropertyAccess_ReturnsPropertyValue()
        {
            var mergeValues = new LavaDictionary { { "CurrentPerson", _helper.GetTestPersonTedDecker() } };

            _helper.AssertTemplateOutput( "North Campus", "{{ CurrentPerson.Campus.Name' }}", mergeValues );
        }

        /// <summary>
        /// Referencing a non-existent property of an input object should return an empty string.
        /// </summary>
        [TestMethod]
        public void ObjectProperty_DotNotationInvalidPropertyName_ReturnsEmptyString()
        {
            var mergeValues = new LavaDictionary { { "CurrentPerson", _helper.GetTestPersonTedDecker() } };

            _helper.AssertTemplateOutput( string.Empty, "{{ CurrentPerson.NonexistentProperty' }}", mergeValues );
        }

        /// <summary>
        /// Accessing the property of a nested dynamically-typed object should return the correct value.
        /// </summary>
        [TestMethod]
        public void ObjectProperty_DotNotationPropertyAccessForAnonymousObject_ReturnsValue()
        {
            var groupMember = new
            {
                GroupName = "Group 1",
                GroupRole = new { Name = "Member", IsLeader = false },
                Person = new { FirstName = "Alex", LastName = "Andrews", Address = new { Street = "1 Main St", City = "MyTown" } }
            };

            var mergeValues = new LavaDictionary { { "GroupMember", groupMember } };

            _helper.AssertTemplateOutput( "Group 1: Andrews, Alex (1 Main St)",
                "{{ GroupMember.GroupName }}: {{ GroupMember.Person.LastName }}, {{ GroupMember.Person.FirstName }} ({{ GroupMember.Person.Address.Street }})",
                mergeValues );

        }
    }
}
