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
using Rock.Lava.Blocks;

namespace Rock.Lava.DotLiquid
{
    /// <summary>
    /// Represents an implementation of a Lava Block for the DotLiquid Templating Framework.
    /// </summary>
    /// <remarks>
    /// This class implements a Lava Shortcode using a DotLiquid.Block Type that can be processed by the DotLiquid framework.
    /// </remarks>
    internal class DotLiquidBlockProxy : Block, IRockLavaBlock
    {
        private static Dictionary<string, Func<string, IRockLavaBlock>> _tagFactoryMethods = new Dictionary<string, Func<string, IRockLavaBlock>>( StringComparer.OrdinalIgnoreCase );

        public static void RegisterFactory( string name, Func<string, IRockLavaBlock> factoryMethod )
        {
            _tagFactoryMethods.Add( name, factoryMethod );
        }

        private IRockLavaBlock _block = null;

        public string BlockName
        {
            get
            {
                return _block.BlockName;
            }
        }

        public override void Initialize( string tagName, string markup, List<string> tokens )
        {
            var factoryMethod = _tagFactoryMethods[tagName];

            _block = factoryMethod( tagName );

            // Initialize the DotLiquid block.
            base.Initialize( tagName, markup, tokens );

            // Call the Lava block initializer.
            _block.OnInitialize( tagName, markup, tokens );

            //base.Initialize( tagName, markup, tokens );
        }

        public override void Render( Context context, TextWriter result )
        {
            var lavaContext = new DotLiquidLavaContext( context );

            var block = _block as RockLavaBlockBase;

            //block.RenderInternal( lavaContext, result, LavaEngine.Instance );
            // Call the Lava block renderer.
            block.RenderInternal( lavaContext, result, this );

            //_block.OnRender( lavaContext, result );

            //base.Render( context, result );
        }

        public void OnInitialize( string tagName, string markup, List<string> tokens )
        {
            this.Initialize( tagName, markup, tokens );
        }

        public void OnRender( ILavaContext context, TextWriter result )
        {
            var dotLiquidContext = ( (DotLiquidLavaContext)context ).DotLiquidContext;

            base.Render( dotLiquidContext, result );
            //this.Render( dotLiquidContext, result );
        }

        public void OnStartup()
        {
            //
        }
    }
}
