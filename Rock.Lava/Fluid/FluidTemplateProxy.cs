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
using Fluid;
using Fluid.Ast;
using Rock.Lava;
using Rock.Lava.DotLiquid;
using Rock.Lava.Fluid;
using Rock.Lava.Shortcodes;

namespace Rock.Lava.Fluid
{

    /// <summary>
    /// An implementation of a LavaTemplate using the Fluid Framework.
    /// </summary>
    /// <remarks>
    /// This class should exist in the Rock.Lava.Fluid library.
    /// </remarks>
    public class FluidTemplateProxy : LavaTemplateBase
    {
        #region Constructors

        private TemplateContext _templateContext;
        private FluidTemplate _template;

        //public ILavaEngine LavaEngine
        //{
        //    get
        //    {
        //        return new FluidEngine();
        //    }
        //}

        //public IList<string> EnabledCommands { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override ILavaEngine LavaEngine => throw new NotImplementedException();

        public FluidTemplateProxy( FluidTemplate template )
        {
            _template = template;

            // Initialize the engine and create a new context.
            //FluidEngine.Initialize();

            //_templateContext = new TemplateContext();

            //_templateContext.ParserFactory = new LavaFluidParserFactory();

            //_templateContext.TemplateFactory = () => { return new FluidTemplateProxy(); };
            ////_templateContext.TemplateFactory

            //_templateContext.Filters.RegisterFiltersFromType( typeof( global::Rock.Lava.Fluid.Filters.FluidFilters ) );
        }

        #endregion

        public void Dispose()
        {
            //
        }

        public void RegisterSafeType( Type type, string[] allowedMembers = null )
        {
            // Not required?
        }

        public override void SetContextValue( string key, object value )
        {
            throw new NotImplementedException();
        }

        protected override bool OnTryRender( LavaRenderParameters parameters, out string output, out IList<Exception> errors )
        {
            output = null;
            errors = new List<Exception>();

            //LavaFluidTemplate template;

            //var parser = _templateContext.TemplateFactory.cre .ParserFactory.CreateParser();

            //bool isValidTemplate = parser.pa .TryParse(( inputTemplate, out template );
            //bool isValidTemplate = LavaFluidTemplate.TryParse( inputTemplate, out template );

            //_templateContext.TemplateFactory. .TemplateFactory

            //List<Statement> statements;
            //IEnumerable<string> errors;

            //isValidTemplate = LavaFluidTemplate.Factory.CreateParser().TryParse( inputTemplate, stripEmptyLines: false, out statements, out errors );

            //if ( isValidTemplate )
            //{
            //    template = new LavaFluidTemplate
            //    {
            //        Statements = statements
            //    };
            //}


            var templateContext = new TemplateContext();

            //parameters.EnabledCommands

            //parameters.InstanceAssigns = null;

            parameters.LocalVariables = null;
            parameters.Registers = null;
            //parameters.ValueTypeTransformers;

            foreach ( var p in parameters.LocalVariables )
            {
                templateContext.SetValue( p.Key, p.Value );
            }

            //var parser = LavaFluidTemplate.Factory.CreateParser();
            //parser.TryParse( inputTemplate, true, out statements, out errors );

            //if ( !isValidTemplate )
            //{
            //    output = null;
            //    return false;
            //}

            output = _template.Render( templateContext );
            // template.Render( _templateContext );

            return true;

        }
    }
}
