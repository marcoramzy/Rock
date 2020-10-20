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
    /// <summary>
    /// Test for shortcodes that are defined and implemented as parameterized Lava templates rather than code components.
    /// </summary>
    [TestClass]
    public class ShortcodeTemplateTests
    {
        private static LavaTestHelper _helper;

        #region Constructors

        [ClassInitialize]
        public static void Initialize( TestContext context )
        {
            _helper = LavaTestHelper.NewForDotLiquidProcessor();


        }

        #endregion

        #region Accordion

        [TestMethod]
        public void AccordionShortcodeBlock_Basic_EmitsCorrectHtml()
        {
            var input = @"
{[ accordion ]}

    [[ item title:'Lorem Ipsum' ]]
        Lorem ipsum dolor sit amet, consectetur adipiscing elit. Ut pretium tortor et orci ornare 
        tincidunt. In hac habitasse platea dictumst. Aliquam blandit dictum fringilla. 
    [[ enditem ]]
    
    [[ item title:'In Commodo Dolor' ]]
        In commodo dolor vel ante porttitor tempor. Ut ac convallis mauris. Sed viverra magna nulla, quis 
        elementum diam ullamcorper et. 
    [[ enditem ]]
    
    [[ item title:'Vivamus Sollicitudin' ]]
        Vivamus sollicitudin, leo quis pulvinar venenatis, lorem sem aliquet nibh, sit amet condimentum
        ligula ex a risus. Curabitur condimentum enim elit, nec auctor massa interdum in.
    [[ enditem ]]

{[ endaccordion ]}
";

            var expectedOutput = @"

";

            _helper.AssertTemplateOutput( expectedOutput, input, ignoreWhitespace: true );
        }

        #endregion

        #region Chart

        [TestMethod]
        public void ChartShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Easy Pie Chart

        [TestMethod]
        public void EasyPieChartShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GoogleMap

        [TestMethod]
        public void GoogleMapShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region GoogleStaticMap

        [TestMethod]
        public void GoogleStaticMapShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Panel

        [TestMethod]
        public void PanelShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Parallax

        [TestMethod]
        public void ParallaxShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region ScheduledContent

        [TestMethod]
        public void ScheduledContentShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Scripturize

        /// <summary>
        /// Using the Scripturize shortcode produces the expected output.
        /// </summary>
        [DataTestMethod]
        [DataRow( "John 3:16", "<a href=\"https://www.bible.com/bible/116/JHN.3.16.NLT\"  class=\"scripture\" title=\"YouVersion\">John 3:16</a>" )]
        [DataRow( "Jn 3:16", "<a href=\"https://www.bible.com/bible/116/JHN.3.16.NLT\"  class=\"scripture\" title=\"YouVersion\">Jn 3:16</a>" )]
        [DataRow( "John 3", "<a href=\"https://www.bible.com/bible/116/JHN.3..NLT\"  class=\"scripture\" title=\"YouVersion\">John 3</a>" )]

        public void ScripturizeShortcode_YouVersion_SimpleCase( string input, string expectedResult )
        {
            _helper.AssertTemplateOutput( expectedResult,
                                          "{[ scripturize defaulttranslation:'NLT' landingsite:'YouVersion' cssclass:'scripture' ]}" + input + "{[ endscripturize ]}" );
        }

        #endregion

        #region Sparkline Chart

        [TestMethod]
        public void SparklineChartShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Vimeo

        [TestMethod]
        public void VimeoShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Wistia Embed

        [TestMethod]
        public void WistiaEmbedShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Word Cloud

        [TestMethod]
        public void WordCloudShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion

        #region YouTube

        [TestMethod]
        public void YouTubeShortcode_Basic_EmitsCorrectHtml( string input, string expectedResult )
        {
            throw new NotImplementedException();
        }

        #endregion


    }
}
