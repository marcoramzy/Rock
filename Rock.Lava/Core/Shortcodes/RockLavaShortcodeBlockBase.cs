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
using Rock.Lava.Blocks;
using Rock.Utility;

namespace Rock.Lava.Shortcodes
{
    /// <summary>
    /// A DotLiquid implementation of the Lava Shortcode block.
    /// </summary>
    public abstract class DotLiquidShortcodeBlock : global::DotLiquid.Block, IRockShortcode //, IRockStartup,
    {
        private IRockShortcode _shortcode;

        public DotLiquidShortcodeBlock( IRockShortcode shortcode )
        {
            _shortcode = shortcode;
        }

        public string TemplateElementName => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="DotLiquid.Block" />
    public abstract class RockLavaShortcodeBlockBase : RockLavaBlockBase // IRockShortcode //DotLiquid.Block, IRockStartup,
    {
        /// <summary>
        /// Get the name of the shortcode as it is used in the Liquid template.
        /// </summary>
        public virtual string TemplateElementName
        {
            get
            {
                return this.GetType().Name.ToLower();
            }
        }
        //public virtual void Initialize( string tagName, string markup, List<string> tokens )
        //{
        //    //_markup = markup;

        //    //base.Initialize( tagName, markup, tokens );
        //}

        /// <summary>
        /// Override this method to parses the supplied set of tokens into a set of nodes that can be processed by the rendering engine.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="templateNodes"></param>
        protected override void Parse( List<string> tokens, out List<object> nodes )
        {
            nodes = null;
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        //public virtual void Render( ILavaContext context, TextWriter result )
        //{
        //    //throw new NotImplementedException();

        //    //base.Render( context, result );
        //}

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

    }
}
