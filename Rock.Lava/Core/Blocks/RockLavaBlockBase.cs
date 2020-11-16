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
    /// Provides base functionality for implementation of a Rock Lava block.
    /// </summary>
    public abstract class RockLavaBlockBase : IRockLavaBlock, ILiquidFrameworkRenderer
    {
        private string _sourceElementName = null;

        /// <summary>
        /// The name of the block as it appears in the source tag.
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

        /// <summary>
        /// Determines if this block is authorized in the specified Lava context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected bool IsAuthorized( ILavaContext context )
        {
            return IsAuthorized( context, this.SourceElementName );
        }

        /// <summary>
        /// Determines if this block is authorized in the specified Lava context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected bool IsAuthorized( ILavaContext context, string commandName )
        {
            return LavaSecurityHelper.IsAuthorized( context, commandName );
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
        /// Override this method to provide custom rendering for the block.
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
        public void Parse( List<string> tokens, out List<object> nodes )
        {
            OnParse( tokens, out nodes );
        }

        /// <summary>
        /// Override this method to provide custom parsing for the block.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="nodes"></param>
        public virtual void OnParse( List<string> tokens, out List<object> nodes )
        {
            nodes = null;
        }

        /// <summary>
        /// Override this method to perform tasks when the block is first loaded at startup.
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

        #endregion

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
            // If this block was previously called with a different base renderer, exit to prevent a circular reference.
            if ( _baseRenderer != null )
            {
                return;
            }

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