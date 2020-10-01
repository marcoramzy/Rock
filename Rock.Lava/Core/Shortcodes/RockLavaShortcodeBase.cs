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
using System.Linq;
using DotLiquid;

namespace Rock.Lava.Shortcodes
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class RockLavaShortcodeBase : IRockShortcode //global::DotLiquid.Tag, IRockStartup
    {
        private string _registeredTagName;
        private string _markup;
        private List<string> _tokens;

        public virtual string Name
        {
            get
            {
                return this.GetType().Name.ToLower();
            }
        }

        public abstract LavaElementTypeSpecifier ElementType { get; }

        public void Initialize( string tagName, string markup, IEnumerable<string> tokens )
        {
            _registeredTagName = tagName;
            _markup = markup;
            _tokens = tokens.ToList();

            OnInitialize( tagName, markup, tokens.ToList() );
        }

        public abstract void OnInitialize( string tagName, string markup, List<string> tokens );

        public void Render( ILavaContext context, TextWriter result )
        {
            OnRender( context, result );
            //throw new System.Exception( "Render is not implemented. This needs to be forwarded to a Liquid framework" );
            //base.Render( context, result );
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public virtual void OnRender( ILavaContext context, TextWriter result )
        {
            result.Write( _markup );
        }

        public void Parse( List<string> tokens, out List<object> nodes )
        {
            OnParse( tokens, out nodes );
        }

        /// <summary>
        /// Parse a set of Lava tokens into a set of document nodes that can be processed by the underlying rendering framework.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="nodes"></param>
        protected virtual void OnParse( List<string> tokens, out List<object> nodes )
        {
            nodes = null;
        }

        /// <summary>
        /// All IRockStartup classes will be run in order by this value. If class does not depend on an order, return zero.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        //public int StartupOrder { get { return 0; } }

        /// <summary>
        /// Method that will be run at Rock startup
        /// </summary>
        //public virtual void OnStartup()
        //{
        //}

        protected virtual void AssertMissingDelimitation()
        {
            throw new Exception( string.Format( "Block Tag not closed: {0}", _registeredTagName ) );
        }

    }
}
