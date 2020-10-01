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

namespace Rock.Lava.DotLiquid
{
    /// <summary>
    /// Represents an implementation of a Lava Shortcode as a DotLiquid Block.
    /// </summary>
    /// <remarks>
    /// This class wraps a Lava Shortcode in a DotLiquid.Block Type that can be processed by the DotLiquid framework.
    /// </remarks>
    internal class DotLiquidStaticShortcodeBlockProxy : Block
    {
        private static DotLiquidShortcodeElementCore _core = new DotLiquidShortcodeElementCore();

        public static void RegisterFactory( string name, Func<string, IRockShortcode> factoryMethod )
        {
            _core.RegisterFactory( name, factoryMethod );
        }

        public override void Initialize( string tagName, string markup, List<string> tokens )
        {
            _core.Initialize( tagName, markup, tokens );

            base.Initialize( tagName, markup, tokens );
        }

        public override void Render( Context context, TextWriter result )
        {
            _core.Render( context, result );

            base.Render( context, result );
        }

    }
}
