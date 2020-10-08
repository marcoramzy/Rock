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
using Rock.Lava.DotLiquid;

namespace Rock.Lava.Blocks
{

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DotLiquid.Block" />
    public abstract class RockLavaBlockBase : IRockLavaBlock // ILava DotLiquid.Block, IRockStartup
    {
        private string _blockName = null;

        

        /// <summary>
        /// The name of the block.
        /// </summary>
        public string BlockName
        {
            get
            {
                if ( _blockName == null )
                {
                    return this.GetType().Name;
                }

                return _blockName;
            }

            set
            {
                _blockName = value;
            }
        }

        public LavaElementTypeSpecifier ElementType
        {
            get
            {
                return LavaElementTypeSpecifier.Block;
            }
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

        /// <summary>
        /// Determines whether the specified command is authorized.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        protected bool IsAuthorized( ILavaContext context )
        {
            return LavaSecurityHelper.IsAuthorized( context, this.GetType().Name );
        }

        #region DotLiquid Block Implementation



        public virtual void OnInitialize( string tagName, string markup, List<string> tokens )
        {
            //
        }

        private IRockLavaBlock _blockProxy;

        internal void RenderInternal( ILavaContext context, TextWriter result, IRockLavaBlock proxy )
        {
            _blockProxy = proxy;

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
            _blockProxy.OnRender( context, result );
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
        public virtual void OnStartup()
        {
            //
        }


        protected virtual void AssertMissingDelimitation()
        {
            throw new Exception( string.Format( "BlockTagNotClosedException: {0}", this.BlockName ) );
            //throw new SyntaxException( Liquid.ResourceManager.GetString( "BlockTagNotClosedException" ), BlockName );
        }
    }
}
