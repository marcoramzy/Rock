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
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Rock.Utility;

namespace Rock.Lava.Blocks
{
    /// <summary>
    /// Tag which allows you to list all of the registered Lava commands on your server.
    /// </summary>
    public class TagList : RockLavaTagBase //, IRockStartup
    {
        //private static readonly Regex Syntax = new Regex( @"(\w+)" );

        //string _markup = string.Empty;

        /// <summary>
        /// Method that will be run at Rock startup
        /// </summary>
        //public void OnStartup()
        //{
        //    //Template.RegisterTag<TagList>( "taglist" );
        //}

        /// <summary>
        /// All IRockStartup classes will be run in order by this value. If class does not depend on an order, return zero.
        /// </summary>
        /// <value>
        /// The order.
        /// </value>
        //public int StartupOrder { get { return 0; } }

        /// <summary>
        /// Initializes the specified tag name.
        /// </summary>
        /// <param name="tagName">Name of the tag.</param>
        /// <param name="markup">The markup.</param>
        /// <param name="tokens">The tokens.</param>
        /// <exception cref="System.Exception">Could not find the variable to place results in.</exception>
        //public override void OnInitialize() // string tagName, string markup, IEnumerable<string> tokens )
        //{
        //    _markup = markup;

        //    base.OnInitialize( tagName, markup, tokens );
        //}

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="result">The result.</param>
        public override void OnRender( ILavaContext context, TextWriter result )
        {
           var tags = LavaEngine.Instance.GetRegisteredElements();

           if ( !tags.Any() )
           {
                return;
           }
           
           var tagList = new StringBuilder();

           tagList.Append( "<strong>Lava Tag List</strong>" );
           tagList.Append( "<ul>" );

           foreach( var kvp in tags.OrderBy( t => t.Key ) )
           {
                var tag = kvp.Value;
                
                tagList.Append( $"<li>{tag.Name} - {tag.SystemTypeName}</li>" );
           }

           tagList.Append( "</ul>" );

           result.Write( tagList.ToString() );

           base.OnRender( context, result );
        }
    }
}