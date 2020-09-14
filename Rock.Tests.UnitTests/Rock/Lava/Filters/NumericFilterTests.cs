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
using Rock.Tests.Shared;

namespace Rock.Tests.UnitTests.Lava
{
    [TestClass]
    public class NumericFilterTests
    {
        private static LavaTestHelper _helper;

        #region Constructors

        [ClassInitialize]
        public static void Initialize( TestContext context )
        {
            _helper = LavaTestHelper.NewForDotLiquidProcessor();
        }

        #endregion

        #region Filter Tests: Format

        /*
         * The Format filter is implemented using the standard .NET Format() function, so we do not need to exhaustively test all supported formats.
        */

        /// <summary>
        /// The Format filter should format a numeric input using a recognized .NET format string correctly.
        /// </summary>
        [TestMethod]
        public void Format_UsingValidDotNetFormatString_ProducesValidNumber()
        {
            _helper.AssertTemplateOutput( "1,234,567.89", "{{ '1234567.89' | Format:'#,##0.00' }}" );
        }

        /// <summary>
        /// The Date filter should translate a date input using a standard .NET format string correctly.
        /// </summary>
        [TestMethod]
        public void Format_EmptyInput_ProducesZeroLengthStringOutput()
        {
            _helper.AssertTemplateOutput( "", "{{ '' | Format:'#,##0.00' }}" );
        }

        /// <summary>
        /// Non-numeric input should be returned unchanged in the output.
        /// </summary>
        [TestMethod]
        public void Format_NonnumericInput_ProducesUnchangedOutput()
        {
            _helper.AssertTemplateOutput( "not_a_number", "{{ 'not_a_number' | Format:'#,##0.00' }}" );
        }

        #endregion

        /// <summary>
        /// Input integer "1" should produce formatted output "1st".
        /// </summary>
        [TestMethod]
        public void NumberToOrdinal_IntegerInput_ProducesValidOrdinal()
        {
            _helper.AssertTemplateOutput( "1st", "{{ 1 | NumberToOrdinal }}" );
        }

        /// <summary>
        /// Input integer "3" should produce formatted output "third".
        /// </summary>
        [TestMethod]
        public void NumberToOrdinalWords_IntegerInput_ProducesValidWords()
        {
            _helper.AssertTemplateOutput( "third", "{{ 3 | NumberToOrdinalWords }}" );
        }

        /// <summary>
        /// Input integer "3" should produce formatted output "third".
        /// </summary>
        [TestMethod]
        public void NumberToRomanNumerals_IntegerInput_ProducesValidNumerals()
        {
            _helper.AssertTemplateOutput( "VII", "{{ 7 | NumberToRomanNumerals }}" );
        }

        /// <summary>
        /// Input integer "1" should produce formatted output "one".
        /// </summary>
        [TestMethod]
        public void NumberToWords_IntegerInput_ProducesValidWords()
        {
            _helper.AssertTemplateOutput( "one", "{{ 1 | NumberToWords }}" );
        }

        /// <summary>
        /// Input having count of "1" should produce a singular item description.
        /// </summary>
        [TestMethod]
        public void ToQuantity_EqualToOne_ProducesSingularDescription()
        {
            _helper.AssertTemplateOutput( "1 phone number", "{{ 'phone number' | ToQuantity:1 }}" );
        }

        /// <summary>
        /// Input having count of "3" should produce a pluralized item description.
        /// </summary>
        [TestMethod]
        public void ToQuantity_GreaterThanOne_ProducesPluralDescription()
        {
            _helper.AssertTemplateOutput( "3 phone numbers", "{{ 'phone number' | ToQuantity:3 }}" );
        }

        /// <summary>
        /// Common text equivalents of True should return a value of True.
        /// </summary>
        [DataTestMethod]
        [DataRow( "true" )]
        [DataRow( "T" )]
        [DataRow( "yes" )]
        [DataRow( "Y" )]
        [DataRow( "1" )]
        public void AsBoolean_Theory_CanConvertCommonTextRepresentationsOfTrue( string input )
        {
            var result = _helper.GetTemplateOutput( "{{ '" +  input + "' | AsBoolean }}" );

            Assert.That.True( result.AsBoolean( false ) );
        }

        /// <summary>
        /// Common text equivalents of False or unrecognized text should return a value of False.
        /// The purpose of this theory is to assure that none of these known or unknown inputs return True.
        /// </summary>
        [DataTestMethod]
        [DataRow( "false" )]
        [DataRow( "F" )]
        [DataRow( "no" )]
        [DataRow( "N" )]
        [DataRow( "0" )]
        [DataRow( "xyzzy" )]
        public void AsBoolean_Theory_CanConvertCommonTextRepresentationsOfFalse( string input )
        {
            var result = _helper.GetTemplateOutput( "{{ '" + input + "' | AsBoolean }}" );

            Assert.That.False( result.AsBoolean( false ) );
        }

        /// <summary>
        /// Common text representations of decimal values should return a decimal.
        /// </summary>
        [DataTestMethod]
        [DataRow( "0.1234", 0.1234 )]
        [DataRow( "1.1", 1.1 )]
        [DataRow( "1,234,567.89", 1234567.89 )]
        [DataRow( "-987.65", -987.65 )]
        [DataRow( "0", 0 )]
        [DataRow( "xyzzy", 0 )]
        public void AsDecimal_Theory_CanConvertText( string input, double expectedResult )
        {
            // Convert to a decimal here, because the DataRow Attribute does not allow a decimal parameter to be explicitly specified.
            var expectedDecimal = Convert.ToDecimal( expectedResult );

            var result = _helper.GetTemplateOutput( "{{ '" + input + "' | AsDecimal }}" );

            Assert.That.Equal( expectedDecimal, result.AsDecimalOrNull() );
        }

        /// <summary>
        /// Common text representations of double-precision values should return a double.
        /// </summary>
        [DataTestMethod]
        [DataRow( "0.1234", 0.1234 )]
        [DataRow( "1.1", 1.1 )]
        [DataRow( "1,234,567.89", 1234567.89 )]
        [DataRow( "-987.65", -987.65 )]
        [DataRow( "0", 0 )]
        [DataRow( "xyzzy", 0 )]
        public void AsDouble_Theory_CanConvertText( string input, double expectedResult )
        {
            var result = _helper.GetTemplateOutput( "{{ '" + input + "' | AsDouble }}" );

            Assert.That.Equal( expectedResult, result.AsDoubleOrNull() );
        }

        /// <summary>
        /// Common text representations of integer values should return an integer.
        /// </summary>
        [DataTestMethod]
        [DataRow( "123", 123 )]
        [DataRow( "-987", -987 )]
        [DataRow( "0", 0 )]
        [DataRow( "xyzzy", 0 )]
        public void AsInteger_Theory_CanConvertText( string input, int expectedResult )
        {
            var result = _helper.GetTemplateOutput( "{{ '" + input + "' | AsInteger }}" );

            Assert.That.Equal( expectedResult, result.AsIntegerOrNull() );
        }

        /// <summary>
        /// Common text representations of integer values should return an integer.
        /// </summary>
        [DataTestMethod]
        [DataRow( "10.4" )]
        [DataRow( "1,234,567" )]
        [DataRow( "$1000" )]
        public void AsInteger_Theory_PunctuatedNumbersReturn0( string input )
        {
            var result = _helper.GetTemplateOutput( "{{ '" + input + "' | AsInteger }}" );

            Assert.That.Equal( 0, result.AsIntegerOrNull() );
        }

    }

}
