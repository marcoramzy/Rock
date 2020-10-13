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

namespace Rock.Lava.Blocks
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DotLiquid.Block" />
    public abstract class RockLavaTagBase : IRockLavaTag
    {
        private string _tagName = null;
        private IRockLavaTag _tagProxy;

        /// <summary>
        /// The name of the tag.
        /// </summary>
        public string TagName
        {
            get
            {
                if ( _tagName == null )
                {
                    return this.GetType().Name;
                }

                return _tagName;
            }

            set
            {
                _tagName = value;
            }
        }

        //public void Initialize( string tagName, string markup, List<string> tokens )
        //{
        //    _tagName = tagName;

        //    OnInitialize( tagName, markup, tokens );
        //}

        //public virtual void OnInitialize( string tagName, string markup, List<string> tokens )
        //{
        //    //
        //}

        public void Render( ILavaContext context, TextWriter result )
        {
            OnRender( context, result );
        }

        //public virtual void OnRender( ILavaContext context, TextWriter result )
        //{
        //    //
        //}

        //public void Initialize( string tagName, string markup, IEnumerable<string> tokens )
        //{
        //    throw new System.NotImplementedException();
        //}

        public virtual void OnStartup()
        {
            //throw new System.NotImplementedException();
        }

        #region DotLiquid Tag Implementation

        public virtual void OnInitialize( string tagName, string markup, IEnumerable<string> tokens )
        {
            //
        }

        internal void RenderInternal( ILavaContext context, TextWriter result, IRockLavaTag proxy )
        {
            _tagProxy = proxy;

            this.OnRender( context, result );
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public virtual void OnRender( ILavaContext context, TextWriter result )
        {
            // By default, call the underlying engine to render this element.
            _tagProxy.OnRender( context, result );
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

        #endregion

    }
}
