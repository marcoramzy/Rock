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

// TODO: Expected output is not completely processed.
            var expectedOutput = @"
             <div class=""panel-group"" id=""accordion-id-910e1c8a-ce44-4493-9949-9a606eb7cf73"" role=""tablist"" aria-multiselectable=""true"">
</div>

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

        #region Sparkline Chart

        [TestMethod]
        public void SparklineShortcode_DefaultOptions_EmitsHtmlWithDefaultSettings()
        {
            var input = @"
{[ sparkline type:'line' data:'5,6,7,9,9,5,3,2,2,4,6,7' ]}
";

            var expectedOutput = @"
xyzzy
";

            _helper.AssertTemplateOutputWithWildcard( expectedOutput, input, ignoreWhitespace: true, wildCard: "<<guid>>" );
        }

        #endregion

        #region Vimeo

        [TestMethod]
        public void VimeoShortcode_Basic_EmitsStyleAndDivElements()
        {
            var input = @"
{[ vimeo id:'180467014' ]}
";

            var expectedOutput = @"
<style>
.embed-container { 
    position: relative; 
    padding-bottom: 56.25%; 
    height: 0; 
    overflow: hidden; 
    max-width: 100%; } 
.embed-container iframe, 
.embed-container object, 
.embed-container embed { position: absolute; top: 0; left: 0; width: 100%; height: 100%; }
</style>

<div id='id-<<guid>>' style='width:;'>
    <div class='embed-container'><iframe src='https://player.vimeo.com/video/180467014?autoplay=0&autoplay=0&loop=0&color=&title=0&byline=0&portrait=0' frameborder='0' webkitallowfullscreen mozallowfullscreen allowfullscreen></iframe></div>
</div>
";

            _helper.AssertTemplateOutputWithWildcard( expectedOutput, input, ignoreWhitespace: true, wildCard: "<<guid>>" );
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
        public void YouTubeShortcode_Basic_EmitsStyleAndDivElements()
        {
            var input = @"
{[ youtube id:'8kpHK4YIwY4' ]}
";

            var expectedOutput = @"
<style>

#id-<<guid>> {
    width: ;
}

.embed-container { 
    position: relative; 
    padding-bottom: 56.25%; 
    height: 0; 
    overflow: hidden; 
    max-width: 100%; } 
.embed-container iframe, 
.embed-container object, 
.embed-container embed { position: absolute; top: 0; left: 0; width: 100%; height: 100%; }
</style>

<div id='id-<<guid>>'>
    <div class='embed-container'><iframe src='https://www.youtube.com/embed/8kpHK4YIwY4?rel=0&showinfo=0&controls=0&autoplay=0' frameborder='0' allowfullscreen></iframe></div>
</div>
";

            _helper.AssertTemplateOutputWithWildcard( expectedOutput, input, ignoreWhitespace: true, wildCard: "<<guid>>" );
        }

        #endregion


    }
}
