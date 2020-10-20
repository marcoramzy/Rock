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

namespace Rock.Lava.Blocks
{
    /// <summary>
    /// Provides base functionality for a Lava Tag element.
    /// </summary>
    public abstract class RockLavaTagBase : IRockLavaTag, ILiquidFrameworkRenderer
    {
        private string _sourceElementName = null;
        private string _attributesMarkup;

        /// <summary>
        /// The raw markup for any additional Attributes contained in the element source tag.
        /// </summary>
        public string ElementAttributesMarkup
        {
            get
            {
                return _attributesMarkup;
            }
        }
        public void Initialize( string tagName, string markup, List<string> tokens )
        {
            _sourceElementName = tagName;
            _attributesMarkup = markup;

            OnInitialize( tagName, markup, tokens );
        }

        public void Render( ILavaContext context, TextWriter result )
        {
            OnRender( context, result );
        }

        public void Parse( List<string> tokens, out List<object> nodes )
        {
            OnParse( tokens, out nodes );
        }

        #region IRockLavaElement implementation

        /// <summary>
        /// The name of the tag.
        /// </summary>
        /// <summary>
        /// The name of the block.
        /// </summary>
        public string SourceElementName
        {
            get
            {
                if ( _sourceElementName == null )
                {
                    return this.GetType().Name.ToLower();
                }

                return _sourceElementName;
            }

            set
            {
                _sourceElementName = ( value == null ) ? null : value.Trim().ToLower();
            }
        }

        #endregion

        #region Customisation methods.

        public virtual void OnStartup()
        {
            //throw new System.NotImplementedException();
        }

        public virtual void OnInitialize( string tagName, string markup, List<string> tokens )
        {
            //
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public virtual void OnRender( ILavaContext context, TextWriter result )
        {
            // By default, call the underlying engine to render this element.
            if ( _baseRenderer != null )
            {
                _baseRenderer.Render( null, context, result );
            }
        }

        /// <summary>
        /// Parse a set of Lava tokens into a set of document nodes that can be processed by the underlying rendering framework.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="nodes"></param>
        public virtual void OnParse( List<string> tokens, out List<object> nodes )
        {
            nodes = null;
        }

        #endregion

        [Obsolete( "???" )]
        internal void RenderInternal( ILavaContext context, TextWriter result, IRockLavaTag proxy )
        {
            this.OnRender( context, result );
        }

        #region ILiquidFrameworkRenderer implementation

        private ILiquidFrameworkRenderer _baseRenderer = null;

        /// <summary>
        /// Render this component using the Liquid templating engine.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="result"></param>
        /// <param name="proxy"></param>
        void ILiquidFrameworkRenderer.Render( ILiquidFrameworkRenderer baseRenderer, ILavaContext context, TextWriter result )
        {
            _baseRenderer = baseRenderer;

            OnRender( context, result );
        }

        void ILiquidFrameworkRenderer.Parse( ILiquidFrameworkRenderer baseRenderer, List<string> tokens, out List<object> nodes )
        {
            baseRenderer.Parse( baseRenderer, tokens, out nodes );
        }

        #endregion
    }
}
