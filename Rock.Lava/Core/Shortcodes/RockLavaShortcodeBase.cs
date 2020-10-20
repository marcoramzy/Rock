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

namespace Rock.Lava
{

    public abstract class RockLavaShortcodeBlockBase : RockLavaShortcodeBase, IRockLavaBlock
    {
    }

    public abstract class RockLavaShortcodeTagBase : RockLavaShortcodeBase, IRockLavaTag
    {
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class RockLavaShortcodeBase : IRockShortcode, ILiquidFrameworkRenderer
    {
        private string _elementName = null;
        private string _internalName = null;

        //private IRockLavaElement _tagProxy = null;
        private IRockLavaElement _lavaElement = null;
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


            //nodes = null;
            _frameworkProxy = this as ILiquidFrameworkRenderer;

            if ( _frameworkProxy == null )
            {
                throw new Exception( "RenderFrameworkObject failed. The supplied Framework object is does not implement the ILiquidFrameworkComponent interface." );
            }

            // By default, call the underlying engine to parse this element.
            //_frameworkProxy.Parse( tokens, out nodes );

            //if ( _tagProxy != null )
            //{
            //    _tagProxy.onp.OnRender( context, result );
            //}
            //else if ( _blockProxy != null )
            //{
            //    _blockProxy.OnRender( context, result );
            //}

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
            //throw new SyntaxException( Liquid.ResourceManager.GetString( "BlockTagNotClosedException" ), BlockName );
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

        /*
        private string _blockName = null;
        private IRockLavaBlock _blockProxy;

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

            OnInitialize( tagName, markup, _tokens );
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

    */
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

            //return;

            //_frameworkProxy = frameworkObject as ILiquidFrameworkRenderer;

            //if ( _frameworkProxy == null )
            //{
            //    throw new Exception( "RenderFrameworkObject failed. The supplied Framework object is does not implement the ILiquidFrameworkComponent interface." );
            //}

            //_tagProxy = frameworkObject as IRockLavaTag;
            //_blockProxy = frameworkObject as IRockLavaBlock;

            //if ( _tagProxy == null
            //     && _blockProxy == null )
            //{
            //    throw new Exception( "RenderFrameworkObject failed. The supplied Framework object is not an expected Type." );
            //}

            //this.OnRender( context, result );
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

            // TODO: whatever called this and supplied frameworkObject should just call the Parse method directly?
            //_frameworkProxy = frameworkObject as ILiquidFrameworkRenderer;

            //if ( _frameworkProxy == null )
            //{
            //    throw new Exception( "RenderFrameworkObject failed. The supplied Framework object is does not implement the ILiquidFrameworkComponent interface." );
            //}

            //_tagProxy = frameworkObject as IRockLavaTag;
            //_blockProxy = frameworkObject as IRockLavaBlock;

            //if ( _tagProxy == null
            //     && _blockProxy == null )
            //{
            //    throw new Exception( "RenderFrameworkObject failed. The supplied Framework object is not an expected Type." );
            //}

            //if ( _blockProxy != null )
            //{
            //    _blockProxy.OnParse( tokens, out nodes );
            //}
            //else if ( _tagProxy != null )
            //{
            //    _tagProxy.OnParse( tokens, out nodes );
            //}
            //else
            //{
            //    nodes = null;
            //}

            //OnParse( tokens, out nodes );
        }

        #endregion

    }
}
