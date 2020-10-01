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
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rock.Lava;
using Rock.Model;
using Rock.Tests.Shared;
using Rock.Web.Cache;

namespace Rock.Tests.Integration.Lava
{
    public class LavaTestHelper
    {
        public static LavaTestHelper NewForDotLiquidProcessor()
        {
            global::Rock.Lava.LavaEngine.InitializeDotLiquidFramework( null, new List<Type> { typeof( RockFilters ) } );

            var engine = global::Rock.Lava.LavaEngine.Instance;

            RegisterBlocks( engine );

            RegisterStaticShortcodes( engine );
            RegisterDynamicShortcodes( engine );


            Debug.Print( "** Registered Tags:" );

            foreach (var tag in engine.GetRegisteredTags())
            {
                Debug.Print( "{0} [{1}]", tag.Key, tag.Value.SystemTypeName );
            }
            Debug.Print( "**" );

            var helper = new LavaTestHelper();

            return helper;
        }

        private static void RegisterBlocks( ILavaEngine engine )
        {
            // Get all blocks and call OnStartup methods
            try
            {
                var elementTypes = Rock.Reflection.FindTypes( typeof( IRockLavaBlock ) ).Select( a => a.Value ).ToList();

                foreach ( var elementType in elementTypes )
                {
                    var shortcodeInstance = Activator.CreateInstance( elementType ) as IRockLavaBlock;

                    var blockName = shortcodeInstance.BlockName;

                    if ( string.IsNullOrWhiteSpace( blockName ) )
                    {
                        blockName = elementType.Name;
                    }

                    engine.RegisterBlock( blockName, ( shortcodeName ) =>
                    {
                        var shortcode = Activator.CreateInstance( elementType ) as IRockLavaBlock;

                        return shortcode;
                    } );

                    try
                    {
                        shortcodeInstance.OnStartup();
                    }
                    catch ( Exception ex )
                    {
                        var lavaException = new Exception( string.Format( "Lava component initialization failure. Startup failed for Lava Block \"{0}\".", elementType.FullName ), ex );

                        ExceptionLogService.LogException( lavaException, null );
                    }

                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex, null );
            }
        }

        private static void RegisterStaticShortcodes( ILavaEngine engine )
        {
            // Get all shortcodes and call OnStartup methods
            try
            {
                var shortcodeTypes = Rock.Reflection.FindTypes( typeof( IRockShortcode ) ).Select( a => a.Value ).ToList();

                foreach ( var shortcodeType in shortcodeTypes )
                {
                    var shortcodeInstance = Activator.CreateInstance( shortcodeType ) as IRockShortcode;

                    engine.RegisterStaticShortcode( shortcodeInstance.Name, ( shortcodeName ) =>
                    {
                        var shortcode = Activator.CreateInstance( shortcodeType ) as IRockShortcode;

                        return shortcode;
                    } );

                    //try
                    //{
                    //    shortcodeInstance.OnStartup();
                    //}
                    //catch ( Exception ex )
                    //{
                    //    ExceptionLogService.LogException( ex, null );
                    //}

                }
            }
            catch ( Exception ex )
            {
                ExceptionLogService.LogException( ex, null );
            }
        }

        private static void RegisterDynamicShortcodes( ILavaEngine engine )
        {
            // Register dynamic shortcodes with a factory method to ensure that the latest definition is retrieved from the global cache.
            Func<string, DynamicShortcodeDefinition> shortCodeFactory = ( shortcodeName ) =>
            {
                DynamicShortcodeDefinition newShortcode = null;

                var shortcodeDefinition = LavaShortcodeCache.All().Where( c => c.TagName == shortcodeName ).FirstOrDefault();

                if ( shortcodeDefinition != null )
                {
                    newShortcode = new DynamicShortcodeDefinition();

                    newShortcode.Name = shortcodeDefinition.Name;
                    newShortcode.TemplateMarkup = shortcodeDefinition.Markup;
                    newShortcode.Tokens = shortcodeDefinition.Parameters.SplitDelimitedValues( ";" ).ToList();

                    if ( shortcodeDefinition.TagType == TagType.Block )
                    {
                        newShortcode.ElementType = LavaElementTypeSpecifier.Block;
                    }
                    else
                    {
                        newShortcode.ElementType = LavaElementTypeSpecifier.Inline;
                    }
                }

                return newShortcode;
            };

            var blockShortCodes = LavaShortcodeCache.All();
            //.Where( s => s.TagType == TagType.Block );

            foreach ( var shortcode in blockShortCodes )
            {
                // register this shortcode
                engine.RegisterDynamicShortcode( shortcode.TagName, shortCodeFactory );
            }
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
        /// Process the specified input template and return the result.
        /// </summary>
        /// <param name="inputTemplate"></param>
        /// <returns></returns>
        public string GetTemplateOutput( string inputTemplate, ILavaContext context )
        {
            string outputString;

            inputTemplate = inputTemplate ?? string.Empty;

            bool isValidTemplate = global::Rock.Lava.LavaEngine.Instance.TryRender( inputTemplate.Trim(), out outputString, context );

            Assert.That.True( isValidTemplate, "Lava Template is invalid." );

            return outputString;
        }

        /// <summary>
        /// Process the specified input template and verify against the expected output.
        /// </summary>
        /// <param name="expectedOutput"></param>
        /// <param name="inputTemplate"></param>
        public void AssertTemplateOutput( string expectedOutput, string inputTemplate, ILavaContext context )
        {
            var outputString = GetTemplateOutput( inputTemplate, context );

            Assert.That.Equal( expectedOutput, outputString );
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
