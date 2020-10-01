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
using System.IO;
using System.Linq;

using DotLiquid;
using Rock.Lava.DotLiquid;

namespace Rock.Lava.Blocks
{
    /// <summary>
    /// Wraps a Lava block element so that it can be rendered by the DotLiquid templating framework.
    /// </summary>
    public class DotLiquidBlockProxy : global::DotLiquid.Block
    {
        private IRockLavaBlock _lavaBlock;

        public DotLiquidBlockProxy( IRockLavaBlock lavaBlock )
        {
            _lavaBlock = lavaBlock;
        }

        public void Initialize( string tagName, string markup, IEnumerable<string> tokens )
        {
            _lavaBlock.OnInitialize( tagName, markup, tokens.ToList() );
        }

        public override void Render( Context context, TextWriter result )
        {
            var lavaContext = new DotLiquidLavaContext( context );

            // Call the Lava block rendering implementation.
            _lavaBlock.OnRender( lavaContext, result );

            //base.Render( context, result );
        }
    }
}
