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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using DotLiquid;
using Fluid;
using Fluid.Ast;
using Fluid.Tags;
using Irony.Parsing;
using Rock.Lava.Blocks;

namespace Rock.Lava.Fluid
{
    /// <summary>
    /// A wrapper for a Lava Tag that can be rendered by the Fluid templating engine.
    /// </summary>
    /// <remarks>





    /// The DotLiquid framework processes a custom tag by creating a new instance of the Tag object from a registered Type and initializing the new instance
    /// by calling the Initialize() method.
    /// We need to intercept this process and generate a proxy Tag from a source class that does not inherit from the DotLiquid.Tag base class.
    /// This proxy class is instantiated by the DotLiquid framework, and we generate an internal instance of a Lava tag that performs the processing.
    /// </remarks>
    internal class FluidTagProxy : global::Fluid.Tags.ITag, ILiquidFrameworkRenderer
    {
        private static Dictionary<string, Func<string, IRockLavaTag>> _factoryMethods = new Dictionary<string, Func<string, IRockLavaTag>>( StringComparer.OrdinalIgnoreCase );

        public static void RegisterFactory( string name, Func<string, IRockLavaTag> factoryMethod )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
            {
                throw new ArgumentException( "Name must be specified." );
            }

            name = name.Trim().ToLower();

            _factoryMethods[name] = factoryMethod;
        }

        private IRockLavaTag _lavaElement = null;

        public string SourceElementName
        {
            get
            {
                return _lavaElement.SourceElementName;
            }
        }

        #region Fluid ITag Implementation

        public Statement Parse( ParseTreeNode node, ParserContext context )
        {
            // By default, this tag is parsed to a simple statement that will render the output to the text stream.
            return new DelegateStatement( WriteToAsync );
        }

        public BnfTerm GetSyntax( FluidGrammar grammar )
        {
            // By default, the Tag does not have any special grammar.
            return grammar.Empty;
        }

        public ValueTask<Completion> WriteToAsync( TextWriter writer, TextEncoder encoder, TemplateContext context )
        //public override void Render( Context context, TextWriter result )
        {
            var lavaContext = new FluidLavaContext( context );

            var tag = _lavaElement as ILiquidFrameworkRenderer;

            if ( tag == null )
            {
                throw new Exception( "Tag proxy cannot be rendered." );
            }

            // Call the renderer implemented by the wrapped Lava block.
            tag.Render( this, lavaContext, writer );

            return new ValueTask<Completion>( Completion.Normal );
        }

        #endregion

        #region IRockLavaTag implementation

        void ILiquidFrameworkRenderer.Render( ILiquidFrameworkRenderer baseRenderer, ILavaContext context, TextWriter result )
        {
            var fluidContext = ( (FluidLavaContext)context ).FluidContext;

            this.WriteToAsync( result, HtmlEncoder.Default, fluidContext );
            ;
        }

        public void OnStartup()
        {
            throw new NotImplementedException( "The OnStartup method is not a valid operation for the DotLiquidTagProxy." );
        }

        void ILiquidFrameworkRenderer.Parse( ILiquidFrameworkRenderer baseRenderer, List<string> tokens, out List<object> nodes )
        {
            // TODO: May need to rework this?
            // Make sure we are getting a list of text-based tokens here.
            // We should be returning a list of Fluid statements as nodes.

            //base.Parse( tokens );

            //nodes = base.NodeList;

            nodes = null;
        }

        #endregion

    }
}
