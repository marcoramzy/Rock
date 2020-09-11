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
    public abstract class RockLavaTagBase : IRockLavaTag //: global::DotLiquid.Tag //, IRockStartup
    {
        /// <summary>
        /// All IRockStartup classes will be run in order by this value. If class does not depend on an order, return zero.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        //public int StartupOrder { get { return 0; } }

        ///// <summary>
        ///// Method that will be run at Rock startup
        ///// </summary>
        //public virtual void OnStartup()
        //{
        //}
        public string TagName { get; private set; }

        public virtual void Initialize( string tagName, string markup, List<string> tokens )
        {
            this.TagName = tagName;
        }

        public virtual void Render( ILavaContext context, TextWriter result )
        {
            //
        }
    }
}
