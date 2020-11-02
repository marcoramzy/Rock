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

namespace Rock.Lava
{

    /// <summary>
    /// Provides base functionality for a Lava shortcode element.
    /// </summary>
    public abstract class RockLavaShortcodeBase : IRockShortcode, ILiquidFrameworkRenderer
    {
        private string _elementName = null;
        private string _internalName = null;

        private ILiquidFrameworkRenderer _frameworkProxy = null;

        /// <summary>
        /// The name of the element.
        /// </summary>
        public string SourceElementName
        {
            get
            {
                if ( _elementName == null )
                {
                    return this.GetType().Name.ToLower();
                }

                return _elementName;
            }

            set
            {
                if ( string.IsNullOrWhiteSpace( value ) )
                {
                    _elementName = null;
                }
                else
                {
                    _elementName = value.Trim().ToLower();
                }
            }
        }

        /// <summary>
        /// The key used to identify the shortcode internally.
        /// This is an augmented version of the tag name to prevent collisions with standard block and tag element names.
        /// </summary>
        public string InternalElementName
        {
            get
            {
                if ( _internalName == null )
                {
                    return this.SourceElementName + LavaEngine.ShortcodeNameSuffix;
                }

                return _internalName;
            }

            set
            {
                _internalName = value;
            }
        }

        /// <summary>
        /// Determines if this block is authorized in the specified Lava context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected bool IsAuthorized( ILavaContext context )
        {
            return LavaSecurityHelper.IsAuthorized( context, this.SourceElementName );
        }

        #region DotLiquid Block Implementation

        /// <summary>
        /// Override this method to provide custom initialization for the block.
        /// </summary>
        /// <param name="tagName"></param>
        /// <param name="markup"></param>
        /// <param name="tokens"></param>
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
            // By default, no parsing is implemented.
            nodes = null;

            return;
        }

        #endregion

        /// <summary>
        /// Method that will be run at Rock startup
        /// </summary>
        public virtual void OnStartup()
        {
            //
        }

        protected virtual void AssertMissingDelimitation()
        {
            throw new Exception( string.Format( "BlockTagNotClosedException: {0}", this.SourceElementName ) );
        }

        /// <summary>
        /// Gets the not authorized message.
        /// </summary>
        /// <value>
        /// The not authorized message.
        /// </value>
        public static string NotAuthorizedMessage
        {
            get
            {
                return "The Lava command '{0}' is not configured for this template.";
            }
        }

        public abstract LavaShortcodeTypeSpecifier ElementType { get; }

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

            // Forward this call through to the implementation provided by the shortcode component.
            OnRender( context, result );
        }

        void ILiquidFrameworkRenderer.Parse( ILiquidFrameworkRenderer baseRenderer, List<string> tokens, out List<object> nodes )
        {
            if ( baseRenderer == null )
            {
                nodes = null;
                return;
            }

            // Forward this call through to the implementation provided by the shortcode component.
            baseRenderer.Parse( null, tokens, out nodes );
        }

        #endregion

    }
}
