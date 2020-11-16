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
using Fluid;
using Fluid.Ast;
using Fluid.Tags;
using Irony.Parsing;

namespace Rock.Lava.Fluid
{
    /// <summary>
    /// A wrapper for a Lava Block that can be rendered by the Fluid templating engine.
    /// </summary>
    /// <remarks>


    public class LavaGrammar : FluidGrammar
    {
        public override void OnGrammarDataConstructed( LanguageData language )
        {
            base.OnGrammarDataConstructed( language );

            FilterArguments.Rule |= MakeListRule( FilterArguments, ToTerm(" "), FilterArgument );

        }
    }



    /// The DotLiquid framework processes a custom Block by creating a new instance of the Block object from a registered Type and initializing the new instance
    /// by calling the Initialize() method.
    /// We need to intercept this process and generate a proxy Block from a source class that does not inherit from the DotLiquid.Block base class.
    /// This proxy class is instantiated by the DotLiquid framework, and we generate an internal instance of a Lava Block that performs the processing.
    /// </remarks>
    internal class FluidBlockProxy : global::Fluid.Tags.ITag, ILiquidFrameworkRenderer
    {
        #region Static factory methods

        private static Dictionary<string, Func<string, IRockLavaBlock>> _factoryMethods = new Dictionary<string, Func<string, IRockLavaBlock>>( StringComparer.OrdinalIgnoreCase );

        public static void RegisterFactory( string name, Func<string, IRockLavaBlock> factoryMethod )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
            {
                throw new ArgumentException( "Name must be specified." );
            }

            name = name.Trim().ToLower();

            _factoryMethods[name] = factoryMethod;
        }

        #endregion

        private IRockLavaBlock _lavaBlock = null;

        public string SourceElementName
        {
            get
            {
                return _lavaBlock.SourceElementName;
            }
        }

        #region Fluid Block Implementation

        public BnfTerm GetSyntax( FluidGrammar grammar )
        {
            // The grammar for the block open tag allows for zero or more arguments.
            // TODO: Modify the grammar rule to make the argument separator (comma) optional.
            return grammar.Empty | grammar.FilterArguments.Rule;
        }

        private string _tagName = null;

        public Statement Parse( ParseTreeNode node, ParserContext context )
        {
            // Get the block instance.
            var blockName = node.Term.Name;

            var factoryMethod = _factoryMethods[blockName];

            _lavaBlock = factoryMethod( blockName );

            _tagName = blockName;

            var e = context.CurrentBlock.Tag.ChildNodes[0];

            var arguments = new FilterArgument[e.ChildNodes.Count];

            string elementAttributesMarkup = string.Empty;
            
            for ( var i = 0; i < e.ChildNodes.Count; i++ )
            {
                if ( i > 0 )
                {
                    elementAttributesMarkup += " ";
                }

                var argParts = e.ChildNodes[i].ChildNodes;

                if ( argParts.Count > 0 )
                {
                    elementAttributesMarkup += argParts[0].FindTokenAndGetText();

                    if ( argParts.Count > 1 )
                    {
                        elementAttributesMarkup += ":" + argParts[1].FindTokenAndGetText();
                    }

                    elementAttributesMarkup += " ";
                }
            }

            elementAttributesMarkup = elementAttributesMarkup.Trim();

            _statements = context.CurrentBlock.Statements;

            return new DelegateStatement( ( writer, encoder, ctx ) => WriteToAsync( writer, encoder, ctx, elementAttributesMarkup, _statements ) );
        }


        //public Statement Parse( ParseTreeNode node, ParserContext context )
        //{
        //    // By default, this Block is parsed to a simple statement that will render the output to the text stream.
        //    return new DelegateStatement( WriteToAsync );
        //}

        //public BnfTerm GetSyntax( FluidGrammar grammar )
        //{
        //    // By default, the Block does not have any special grammar.
        //    return grammar.Empty;
        //}

        //public override ValueTask<Completion> WriteToAsync( TextWriter writer, TextEncoder encoder, TemplateContext context, FilterArgument[] arguments, List<Statement> statements )
        //public ValueTask<Completion> WriteToAsync( TextWriter writer, TextEncoder encoder, TemplateContext context )
        //public override void Render( Context context, TextWriter result )

        private FilterArgument[] _arguments = null;
        private List<Statement> _statements = null;


        public ValueTask<Completion> WriteToAsync( TextWriter writer, TextEncoder encoder, TemplateContext context, string elementAttributesMarkup, List<Statement> statements )
        {
            //_arguments = arguments;
            //_statements = statements;

            //if ( !_factoryMethods.ContainsKey( tagName ) )
            //{
            //    throw new Exception( "Block factory could not be found." );
            //}

            //var factoryMethod = _factoryMethods[tagName];

            //_lavaBlock = factoryMethod( tagName );

            //if ( _lavaBlock == null )
            //{
            //    throw new Exception( "Block factory could not provide a compatible block instance." );
            //}

            //// Initialize the Lava block first, because it may be called during the DotLiquid.Block initialization process.
            //_lavaBlock.OnInitialize( tagName, markup, tokens );

            //// Initialize the DotLiquid block.
            //base.Initialize( tagName, markup, tokens );




            var lavaContext = new FluidLavaContext( context );


            //var block = _lavaBlock as IRock ILiquidFrameworkRenderer;

            //if ( block == null )
            //{
            //    throw new Exception( "Block proxy cannot be rendered." );
            //}

            // Initialize the Lava block first, because it may be called during the DotLiquid.Block initialization process.


            // Initialize the DotLiquid block.
            var tokens = new List<string>();

            _lavaBlock.OnInitialize( this.SourceElementName, elementAttributesMarkup, tokens );

            //base.Initialize( tagName, markup, tokens );

            // Call the renderer implemented by the wrapped Lava block.

            // TODO: We need to render each of the Statements individually here?


            var element = _lavaBlock as ILiquidFrameworkRenderer;

            if ( element == null )
            {
                throw new Exception( "Block proxy cannot be rendered." );
            }

            element.Render( this, lavaContext, writer );

            return new ValueTask<Completion>( Completion.Normal );
        }

        #endregion

        private async ValueTask<Completion> WriteToDefaultAsync( TextWriter writer, TextEncoder encoder, TemplateContext context, List<Statement> statements )
        {
            //await writer.WriteAsync( "simple" );

            //await base.RenderStatementsAsync( writer, encoder, context, statements );

            Completion completion;

            foreach ( var statement in statements )
            {
                completion = await statement.WriteToAsync( writer, encoder, context );

                if ( completion != Completion.Normal )
                {
                    // Stop processing the block statements
                    return completion;
                }
            }

            return Completion.Normal;
        }

        #region ILiquidFrameworkRenderer implementation

        void ILiquidFrameworkRenderer.Render( ILiquidFrameworkRenderer baseRenderer, ILavaContext context, TextWriter writer )
        {
            var fluidContext = ( (FluidLavaContext)context ).FluidContext;
            
            var result = WriteToDefaultAsync( writer, HtmlEncoder.Default, fluidContext, _statements );

            //var block = _lavaBlock as ILiquidFrameworkRenderer;

            //if ( block == null )
            //{
            //    throw new Exception( "Block proxy cannot be rendered." );
            //}

            //// Call the renderer implemented by the wrapped Lava block.

            //// TODO: We need to render each of the Statements individually here?

            

            //block.Render( this, context, writer );




            //var fluidContext = ( (FluidLavaContext)context ).FluidContext;

            //// TODO: Rock Block implementations tend to parse their own arguments list from the content.
            //this.WriteToAsync( result, HtmlEncoder.Default, fluidContext, _arguments, _statements );
            ;
        }

        public void OnStartup()
        {
            throw new NotImplementedException( "The OnStartup method is not a valid operation for the DotLiquidBlockProxy." );
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
