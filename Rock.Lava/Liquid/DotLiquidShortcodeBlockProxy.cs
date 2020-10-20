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
using System.IO;
using DotLiquid;
using Rock.Lava.Shortcodes;

namespace Rock.Lava.DotLiquid
{
    /// <summary>
    /// Provides common functions for a DotLiquid implementation of a Lava Shortcode,
    /// that can be used with a Tag or Block implementation.
    /// </summary>
    internal class DotLiquidShortcodeBlockProxy : Block, IRockShortcode
    {
        public void RegisterFactory( string name, Func<string, IRockShortcode> factoryMethod )
        {
            _tagFactoryMethods.Add( name, factoryMethod );
        }

        public static Dictionary<string, Func<string, IRockShortcode>> TagFactoryMethods
        {
            get
            {
                return _tagFactoryMethods;
            }
        }

        public LavaElementTypeSpecifier ElementType
        {
            get
            {
                return LavaElementTypeSpecifier.Block;
            }
        }
        

        private static Dictionary<string, Func<string, IRockShortcode>> _tagFactoryMethods = new Dictionary<string, Func<string, IRockShortcode>>();
        private IRockShortcode _shortcode = null;

        //public Func<string, IRockShortcode> ShortcodeFactoryMethod { get; set; }

        #region DotLiquid Block Overrides

        public override void OnInitialize( string tagName, string markup, List<string> tokens )
        {
            var factoryMethod = _tagFactoryMethods[tagName];

            _shortcode = factoryMethod( tagName );

            // Initialize the DotLiquid block.
            base.OnInitialize( tagName, markup, tokens );

            // Call the Lava block initializer.
            _shortcode.OnInitialize( tagName, markup, tokens );
        }

        public override void Render( Context context, TextWriter result )
        {
            var lavaContext = new DotLiquidLavaContext( context );

            var shortcode = _shortcode as RockLavaShortcodeBase;

            // Call the Lava block renderer.
            shortcode.RenderInternal( lavaContext, result, this );
        }

        #endregion

        #region IRockLavaBlock implementation

        public void Initialize( string tagName, string markup, List<string> tokens )
        {
            var factoryMethod = _tagFactoryMethods[tagName];

            _shortcode = factoryMethod( tagName );

            _shortcode.OnInitialize( tagName, markup, tokens );

            //base.Initialize( tagName, markup, tokens );
        }

        public void OnRender( ILavaContext context, TextWriter result )
        {
            var dotLiquidContext = ( (DotLiquidLavaContext)context ).DotLiquidContext;

            base.Render( dotLiquidContext, result );
        }

        #endregion

    }
}
