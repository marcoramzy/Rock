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
using System.IO;

using DotLiquid;

namespace Rock.Lava.Shortcodes
{
    /// <summary>
    /// 
    /// </summary>
    public class RockLavaShortcodeBase : IRockShortcode //global::DotLiquid.Tag, IRockStartup
    {
        public string TemplateElementName => throw new System.NotImplementedException();

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public void Render( ILavaContext context, TextWriter result )
        {
            throw new System.Exception( "Render is not implemented. This needs to be forwarded to a Liquid framework" );
            //base.Render( context, result );
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

    }
}
