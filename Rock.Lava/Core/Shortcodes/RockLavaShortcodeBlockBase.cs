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
using Rock.Lava.Blocks;

namespace Rock.Lava.Shortcodes
{

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
        /// Override this method to parse the supplied set of tokens into a set of nodes that can be processed by the rendering engine.
        /// </summary>
        /// <param name="tokens"></param>
        /// <param name="templateNodes"></param>
        protected override void Parse( List<string> tokens, out List<object> nodes )
        {
            nodes = null;
        }

        /// <summary>
        /// Renders the specified context to a text stream.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        //public abstract void Render( ILavaContext context, TextWriter result );
    }
}
