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
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using System.Runtime.CompilerServices;
using System.Linq;

namespace Rock.Lava.Fluid
{
    public class BaseFluidTemplateEx<T,TParserFactory> : IFluidTemplate where T : IFluidTemplate, new()
        where TParserFactory : IFluidParserFactory, new()
    {
        static BaseFluidTemplateEx()
        {
            // Necessary to force the custom template class static constructor
            // as the only member accessed is defined on this class
            RuntimeHelpers.RunClassConstructor( typeof( T ).TypeHandle );
        }
        
        public static TParserFactory Factory { get; } = new TParserFactory();
        private static Func<IFluidTemplate> TemplateFactory { get; } = () => new T();

        public List<Statement> Statements { get; set; } = new List<Statement>();

        public static T Parse( string template )
        {
            if ( !TryParse( template, out var result, out var errors ) )
            {
                return ExceptionHelper.ThrowParseException<T>( errors.FirstOrDefault() ?? "" );
            }
            else
            {
                return result;
            }
        }

        public static bool TryParse( string template, out T result, out IEnumerable<string> errors )
        {
            return TryParse( template, true, out result, out errors );
        }

        public static bool TryParse( string template, out T result )
        {
            return TryParse( template, out result, out var errors );
        }

        public static bool TryParse( string template, bool stripEmptyLines, out T result, out IEnumerable<string> errors )
        {
            if ( Factory.CreateParser().TryParse( template, stripEmptyLines, out var statements, out errors ) )
            {
                result = new T
                {
                    Statements = statements
                };
                return true;
            }
            else
            {
                result = default( T );
                return false;
            }
        }

        public ValueTask RenderAsync( TextWriter writer, TextEncoder encoder, TemplateContext context )
        {
            if ( writer == null )
            {
                ExceptionHelper.ThrowArgumentNullException( nameof( writer ) );
            }

            if ( encoder == null )
            {
                ExceptionHelper.ThrowArgumentNullException( nameof( encoder ) );
            }

            if ( context == null )
            {
                return ExceptionHelper.ThrowArgumentNullException<ValueTask>( nameof( context ) );
            }

            context.ParserFactory = Factory;
            context.TemplateFactory = TemplateFactory;

            var count = Statements.Count;
            for ( var i = 0; i < count; i++ )
            {
                var task = Statements[i].WriteToAsync( writer, encoder, context );
                if ( !task.IsCompletedSuccessfully )
                {
                    return Awaited(
                        task,
                        writer,
                        encoder,
                        context,
                        Statements,
                        startIndex: i + 1 );
                }
            }

            return new ValueTask();
        }

        private static async ValueTask Awaited(
            ValueTask<Completion> task,
            TextWriter writer,
            TextEncoder encoder,
            TemplateContext context,
            List<Statement> statements,
            int startIndex )
        {
            await task;
            for ( var i = startIndex; i < statements.Count; i++ )
            {
                await statements[i].WriteToAsync( writer, encoder, context );
            }
        }
    }



    

}
