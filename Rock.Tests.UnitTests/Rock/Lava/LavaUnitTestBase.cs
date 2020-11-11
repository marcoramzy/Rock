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
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Lava;

namespace Rock.Tests.UnitTests.Lava
{
    [TestClass]
    public class LavaUnitTestBase
    {
        public static LavaEngineTypeSpecifier EngineType { get; set; }
        public static LavaTestHelper _helper { get; set; }

        [AssemblyInitialize]
        public static void ClassInitialize( TestContext testContext )
        {
            LavaEngineTypeSpecifier engineType;

            var isValid = Enum.TryParse<LavaEngineTypeSpecifier>( testContext.Properties["LavaEngineType"].ToString(), out engineType );

            if ( !isValid )
            {
                engineType = LavaEngineTypeSpecifier.DotLiquid;
            }

            EngineType = engineType;

            _helper = LavaTestHelper.New( engineType );
        }
    }
}
