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
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using DotLiquid;
using Rock.Common;
using Rock.Lava.Utility;

//using Rock.Model;
//using Rock.Utility;
//using Rock.Web.Cache;

namespace Rock.Lava.Shortcodes
{
    public class DynamicShortcodeBlock: DynamicShortcode, IRockLavaBlock
    {
        public DynamicShortcodeBlock()
        {
            //
        }

        public DynamicShortcodeBlock( DynamicShortcodeDefinition definition )
            : base( definition )
        {
            //
        }
    }

    public class DynamicShortcodeTag : DynamicShortcode, IRockLavaTag
    {
        public DynamicShortcodeTag()
        {
            //
        }

        public DynamicShortcodeTag( DynamicShortcodeDefinition definition )
            : base ( definition )
        {
            //
        }
    }

    /// <summary>
    /// A shortcode that uses a parameterized Lava template supplied at runtime to generate an element in a Lava source document.
    /// </summary>
    public class DynamicShortcode : RockLavaShortcodeBase
    {
        //private static readonly Regex Syntax = new Regex( @"(\w+)" );

        string _elementAttributesMarkup = string.Empty;
        string _tagName = string.Empty;
        DynamicShortcodeDefinition _shortcode = null;
        //private LavaShortcodeTypeSpecifier? _elementType = null;
        private string _shortcodeParameters = null;
        private string _enabledLavaCommands = string.Empty;

        //Dictionary<string, object> _internalMergeFields;

        const int _maxRecursionDepth = 10;

        /// <summary>
        /// Default constructor required for internal use.
        /// An instance created using this constructor must call the Initialize() method before use.
        /// </summary>
        public DynamicShortcode()
        {
            //
        }

        public DynamicShortcode( DynamicShortcodeDefinition definition )
        {
            this.Initialize( definition );

            AssertShortcodeIsInitialized();
        }

        public void Initialize( DynamicShortcodeDefinition definition )
        {
            _shortcode = definition;
            _shortcodeParameters = string.Empty;
        }

        public override LavaShortcodeTypeSpecifier ElementType
        {
            get
            {
                return _shortcode.ElementType;
            }
        }

        /// <summary>
        /// Method that will be run at Rock startup
        /// </summary>
        public override void OnStartup()
        {
            // get all the inline dynamic shortcodes and register them
            //var inlineShortCodes = LavaShortcodeCache.All().Where( s => s.TagType == TagType.Inline );

            //foreach(var shortcode in inlineShortCodes )
            //{
            //    // register this shortcode
            //    Template.RegisterShortcode<DynamicShortcodeInline>( shortcode.TagName );
            //}
        }

        /// <summary>
        /// Initializes the specified tag name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="markup">The markup.</param>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.Exception">Could not find the variable to place results in.</exception>
        public override void OnInitialize( string tagName, string markup, List<string> tokens )
        {
            _elementAttributesMarkup = markup;
            _tagName = tagName;
            //_shortcode = LavaShortcodeCache.All().Where( c => c.TagName == tagName ).FirstOrDefault();

            base.OnInitialize( tagName, markup, tokens );
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OnRender( ILavaContext context, TextWriter result )
        {
            AssertShortcodeIsInitialized();

            //if (_shortcode != null )
            //{
                var parms = ParseMarkup( _elementAttributesMarkup, context );

                // add a unique id so shortcodes have easy access to one
                parms.AddOrReplace( "uniqueid", "id-" + Guid.NewGuid().ToString() );

                // keep track of the recursion depth
                int currentRecurrsionDepth = 0;
                if ( parms.ContainsKey( "RecursionDepth" ) )
                {
                    currentRecurrsionDepth = parms["RecursionDepth"].ToString().AsInteger() + 1;

                    if ( currentRecurrsionDepth > _maxRecursionDepth )
                    {
                        result.Write( "A recursive loop was detected and processing of this shortcode has stopped." );
                        return;
                    }
                }
                parms.AddOrReplace( "RecursionDepth", currentRecurrsionDepth );

                var results = context.ResolveMergeFields( _shortcode.TemplateMarkup, parms, _enabledLavaCommands );
            //var results = _shortcode.TemplateMarkup.ResolveMergeFields( parms, "" ); // _shortcode..EnabledLavaCommands );
                result.Write( results );
            //}
            //else
            //{
            //    result.Write( $"An error occurred while processing the {0} shortcode.", _tagName );
            //}

            base.OnRender( context, result );
        }

        private void AssertShortcodeIsInitialized()
        {
            if ( _shortcode == null )
            {
                throw new Exception( $"Shortcode configuration error. \"{_tagName}\" is not initialized." );
            }
        }

        /// <summary>
        /// Parses the markup.
        /// </summary>
        /// <param name="markup">The markup.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private Dictionary<string, object> ParseMarkup( string markup, ILavaContext context )
        {
            //var parms = new Dictionary<string, object>();

            // first run lava across the inputted markup
            var _internalMergeFields = new Dictionary<string, object>();

            var parms = new Dictionary<string, object>( _internalMergeFields );

            // get merge fields loaded by the block or container

            //if ( context.Environments.Count > 0 )
            //{
            foreach ( var item in context.GetMergeFieldsInContainerScope() )
            {
                _internalMergeFields.AddOrReplace( item.Key, item.Value );
                parms.AddOrReplace( item.Key, item.Value );
            }
            //}

            // get variables defined in the lava source
            foreach ( var item in context.GetMergeFieldsInScope() )
            {
                _internalMergeFields.AddOrReplace( item.Key, item.Value );
                parms.AddOrReplace( item.Key, item.Value );
            }

            var resolvedMarkup = context.ResolveMergeFields( markup, _internalMergeFields );

            // create all the parameters from the shortcode with their default values
            var shortcodeParms = RockSerializableDictionary.FromUriEncodedString( _shortcodeParameters );

            foreach ( var shortcodeParm in shortcodeParms.Dictionary )
            {
                parms.AddOrReplace( shortcodeParm.Key, shortcodeParm.Value );
            }

            var markupItems = Regex.Matches( resolvedMarkup, @"(\S*?:'[^']+')" )
                .Cast<Match>()
                .Select( m => m.Value )
                .ToList();

            foreach ( var item in markupItems )
            {
                var itemParts = item.ToString().Split( new char[] { ':' }, 2 );
                if ( itemParts.Length > 1 )
                {
                    parms.AddOrReplace( itemParts[0].Trim().ToLower(), itemParts[1].Trim().Substring( 1, itemParts[1].Length - 2 ) );
                }
            }

            // OK, now let's look for any passed variables ala: name:variable
            var variableTokens = Regex.Matches( resolvedMarkup, @"\w*:\w+" )
                .Cast<Match>()
                .Select( m => m.Value )
                .ToList();

            foreach ( var item in variableTokens )
            {
                var itemParts = item.Trim().Split( new char[] { ':' }, 2 );
                if ( itemParts.Length > 1 )
                {
                    var scopeKey = itemParts[1].Trim();

                    // context.Scopes is a weird beast can't find a cleaner way to get the object than to iterate over it
                    var scopeObject = context.GetMergeFieldValue( scopeKey, null );

                    if ( scopeObject != null )
                    {
                        parms.AddOrReplace( itemParts[0].Trim().ToLower(), scopeObject );
                    }

                    //foreach ( var scopeItem in context.GetScopes )
                    //{
                    //    var scopeObject = scopeItem.Where( x => x.Key == scopeKey ).FirstOrDefault();

                    //    if ( scopeObject.Value != null )
                    //    {
                    //        parms.AddOrReplace( itemParts[0].Trim().ToLower(), scopeObject.Value );
                    //        break;
                    //    }
                    //}
                }
            }

            return parms;
        }
    }
}