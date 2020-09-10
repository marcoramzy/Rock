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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Lava;
using Rock.Tests.Shared;

namespace Rock.Tests.Lava
{
    public class LavaTestHelper
    {
        public static LavaTestHelper NewForDotLiquidProcessor()
        {
            global::Rock.Lava.LavaEngine.InitializeDotLiquidFramework( null, new List<Type> { typeof( RockFilters ) } );

            var helper = new LavaTestHelper();

            return helper;
        }

        public ILavaEngine LavaEngine
        {
            get
            {
                return global::Rock.Lava.LavaEngine.Instance;
            }
        }

        /// <summary>
        /// Process the specified input template and return the result.
        /// </summary>
        /// <param name="inputTemplate"></param>
        /// <returns></returns>
        public string GetTemplateOutput( string inputTemplate, LavaDictionary mergeValues = null )
        {
            string outputString;

            inputTemplate = inputTemplate ?? string.Empty;

            bool isValidTemplate = global::Rock.Lava.LavaEngine.Instance.TryRender( inputTemplate.Trim(), out outputString, mergeValues );

            Assert.That.True( isValidTemplate, "Lava Template is invalid." );

            return outputString;
        }

        /// <summary>
        /// Process the specified input template and verify against the expected output.
        /// </summary>
        /// <param name="expectedOutput"></param>
        /// <param name="inputTemplate"></param>
        public void AssertTemplateOutput( string expectedOutput, string inputTemplate, LavaDictionary mergeValues = null )
        {
            var outputString = GetTemplateOutput( inputTemplate, mergeValues );

            Assert.That.Equal( expectedOutput, outputString );
        }

        /// <summary>
        /// Process the specified input template and verify against the expected output regular expression.
        /// </summary>
        /// <param name="expectedOutputRegex"></param>
        /// <param name="inputTemplate"></param>
        public void AssertTemplateOutputRegex( string expectedOutputRegex, string inputTemplate, LavaDictionary mergeValues = null )
        {
            var outputString = GetTemplateOutput( inputTemplate, mergeValues );

            var regex = new Regex(expectedOutputRegex);

            StringAssert.Matches( outputString, regex );
        }

        /// <summary>
        /// Process the specified input template and verify the output against an expected DateTime result.
        /// </summary>
        /// <param name="expectedDateTime"></param>
        /// <param name="inputTemplate"></param>
        /// <param name="maximumDelta"></param>
        public void AssertTemplateOutputDate( DateTime? expectedDateTime, string inputTemplate, TimeSpan? maximumDelta = null )
        {
            var outputString = GetTemplateOutput( inputTemplate );

            DateTime outputDate;

            var isValidDate = DateTime.TryParse( outputString, out outputDate );

            Assert.That.True( isValidDate, $"Template Output does not represent a valid DateTime. [Output=\"{ outputString }\"]" );

            if ( maximumDelta != null )
            {
                Assert.That.AreProximate( expectedDateTime, outputDate, maximumDelta.Value );
            }
            else
            {
                Assert.That.AreEqual( expectedDateTime, outputDate );
            }
        }

        /// <summary>
        /// Resolve the specified template to a date and verify that it is equivalent to the expected date.
        /// </summary>
        /// <param name="expectedDateString"></param>
        /// <param name="inputTemplate"></param>
        /// <param name="maximumDelta"></param>
        public void AssertTemplateOutputDate( string expectedDateString, string inputTemplate, TimeSpan? maximumDelta = null )
        {
            bool isValid;
            DateTime expectedDate;

            isValid = DateTime.TryParse( expectedDateString, out expectedDate );

            Assert.That.True( isValid, "Expected Date String input is not a valid date." );

            AssertTemplateOutputDate( expectedDate, inputTemplate, maximumDelta );
        }
    }
}
