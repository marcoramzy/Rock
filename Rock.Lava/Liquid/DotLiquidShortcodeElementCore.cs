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
    /// Provides common functions for a DotLiquid implementation of a Lava Shortcode,
    /// that can be used with a Tag or Block implementation.
    /// </summary>
    internal class DotLiquidShortcodeElementCore
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

        private static Dictionary<string, Func<string, IRockShortcode>> _tagFactoryMethods = new Dictionary<string, Func<string, IRockShortcode>>();
        private IRockShortcode _shortcode = null;

        //public Func<string, IRockShortcode> ShortcodeFactoryMethod { get; set; }

        public void Initialize( string tagName, string markup, List<string> tokens )
        {
            var factoryMethod = _tagFactoryMethods[tagName];

            _shortcode = factoryMethod( tagName );

            _shortcode.Initialize( tagName, markup, tokens );

            //base.Initialize( tagName, markup, tokens );
        }

        public void Render( Context context, TextWriter result )
        {
            var lavaContext = new DotLiquidLavaContext( context );

            _shortcode.Render( lavaContext, result );

            //base.Render( context, result );
        }

    }
}
