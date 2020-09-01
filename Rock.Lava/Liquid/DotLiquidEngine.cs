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
using System.Reflection;
using DotLiquid;

namespace Rock.Lava.DotLiquid
{
    
    /// <summary>
    /// Initialization class for the DotLiquid Templating Engine.
    /// </summary>
    public class DotLiquidEngineManager : LavaEngineBase //: ILavaEngine
    {

        public override string FrameworkName
        {
            get
            {
                return "DotLiquid";
            }
        }

        /// <summary>
        /// Initializes Rock's Fluid implementation of Lava.
        /// </summary>
        public void Initialize()
        {
            HideSnakeCaseFilters();
            RegisterBaseFilters();

            //
            // Register the Rock filters.
            //
            //TemplateContext.GlobalFilters.RegisterFiltersFromType( typeof( Filters.FluidFilters ) );

            // Set the default strategy for locating object properties to our custom implementation that adds
            // the ability to resolve properties of nested anonymous Types using Reflection.
            //TemplateContext.GlobalMemberAccessStrategy = new DynamicMemberAccessStrategy();
        }

        /// <summary>
        /// This method hides the snake-case filters that are registered
        /// by default. Rock uses CamelCase filter names and to ensure that
        /// a mistype doesn't cause it to work anyway we hide these.
        /// </summary>
        private void HideSnakeCaseFilters()
        {
            //TemplateContext.GlobalFilters.AddFilter( "join", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "first", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "last", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "concat", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "map", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "reverse", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "size", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "sort", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "sort_natural", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "uniq", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "where", NoOp );

            //TemplateContext.GlobalFilters.AddFilter( "default", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "date", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "format_date", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "raw", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "compact", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "url_encode", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "url_decode", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "strip_html", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "escape", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "escape_once", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "handle", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "handleize", NoOp );

            //TemplateContext.GlobalFilters.AddFilter( "abs", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "at_least", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "at_most", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "ceil", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "divided_by", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "floor", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "minus", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "modulo", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "plus", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "round", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "times", NoOp );

            //TemplateContext.GlobalFilters.AddFilter( "append", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "capitalize", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "downcase", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "lstrip", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "rstrip", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "newline_to_br", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "prepend", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "removefirst", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "remove", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "replacefirst", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "replace", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "slice", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "split", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "strip", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "strip_newlines", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "truncate", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "truncatewords", NoOp );
            //TemplateContext.GlobalFilters.AddFilter( "upcase", NoOp );
        }

        /// <summary>
        /// Registers all the base Fluid filters with the proper CamelCase.
        /// </summary>
        private void RegisterBaseFilters()
        {
            //TemplateContext.GlobalFilters.AddFilter( "Join", global::Fluid.Filters.ArrayFilters.Join );
            //TemplateContext.GlobalFilters.AddFilter( "First", global::Fluid.Filters.ArrayFilters.First );
            //TemplateContext.GlobalFilters.AddFilter( "Last", global::Fluid.Filters.ArrayFilters.Last );
            //TemplateContext.GlobalFilters.AddFilter( "Map", global::Fluid.Filters.ArrayFilters.Map );
            //TemplateContext.GlobalFilters.AddFilter( "Reverse", global::Fluid.Filters.ArrayFilters.Reverse );
            //TemplateContext.GlobalFilters.AddFilter( "Size", global::Fluid.Filters.ArrayFilters.Size );
            //TemplateContext.GlobalFilters.AddFilter( "Sort", global::Fluid.Filters.ArrayFilters.Sort );
            //TemplateContext.GlobalFilters.AddFilter( "Uniq", global::Fluid.Filters.ArrayFilters.Uniq );
            //TemplateContext.GlobalFilters.AddFilter( "Where", global::Fluid.Filters.ArrayFilters.Where );

            //TemplateContext.GlobalFilters.AddFilter( "Default", global::Fluid.Filters.MiscFilters.Default );
            //TemplateContext.GlobalFilters.AddFilter( "Date", global::Fluid.Filters.MiscFilters.Date );
            //TemplateContext.GlobalFilters.AddFilter( "UnescapeDataString", global::Fluid.Filters.MiscFilters.UrlDecode );
            //TemplateContext.GlobalFilters.AddFilter( "EscapeDataString", global::Fluid.Filters.MiscFilters.UrlEncode );
            //TemplateContext.GlobalFilters.AddFilter( "StripHtml", global::Fluid.Filters.MiscFilters.StripHtml );
            //TemplateContext.GlobalFilters.AddFilter( "Escape", global::Fluid.Filters.MiscFilters.Escape );

            //TemplateContext.GlobalFilters.AddFilter( "AtLeast", global::Fluid.Filters.NumberFilters.AtLeast );
            //TemplateContext.GlobalFilters.AddFilter( "AtMost", global::Fluid.Filters.NumberFilters.AtMost );
            //TemplateContext.GlobalFilters.AddFilter( "Ceiling", global::Fluid.Filters.NumberFilters.Ceil );
            //TemplateContext.GlobalFilters.AddFilter( "DividedBy", global::Fluid.Filters.NumberFilters.DividedBy );
            //TemplateContext.GlobalFilters.AddFilter( "Floor", global::Fluid.Filters.NumberFilters.Floor );
            //TemplateContext.GlobalFilters.AddFilter( "Minus", global::Fluid.Filters.NumberFilters.Minus );
            //TemplateContext.GlobalFilters.AddFilter( "Modulo", global::Fluid.Filters.NumberFilters.Modulo );
            //TemplateContext.GlobalFilters.AddFilter( "Plus", global::Fluid.Filters.NumberFilters.Plus );
            //TemplateContext.GlobalFilters.AddFilter( "Times", global::Fluid.Filters.NumberFilters.Times );

            //TemplateContext.GlobalFilters.AddFilter( "Append", global::Fluid.Filters.StringFilters.Append );
            //TemplateContext.GlobalFilters.AddFilter( "Capitalize", global::Fluid.Filters.StringFilters.Capitalize );
            //TemplateContext.GlobalFilters.AddFilter( "Downcase", global::Fluid.Filters.StringFilters.Downcase );
            //TemplateContext.GlobalFilters.AddFilter( "NewlineToBr", global::Fluid.Filters.StringFilters.NewLineToBr );
            //TemplateContext.GlobalFilters.AddFilter( "Prepend", global::Fluid.Filters.StringFilters.Prepend );
            //TemplateContext.GlobalFilters.AddFilter( "RemoveFirst", global::Fluid.Filters.StringFilters.RemoveFirst );
            //TemplateContext.GlobalFilters.AddFilter( "Remove", global::Fluid.Filters.StringFilters.Remove );
            //TemplateContext.GlobalFilters.AddFilter( "ReplaceFirst", global::Fluid.Filters.StringFilters.ReplaceFirst );
            //TemplateContext.GlobalFilters.AddFilter( "Replace", global::Fluid.Filters.StringFilters.Replace );
            //TemplateContext.GlobalFilters.AddFilter( "Slice", global::Fluid.Filters.StringFilters.Slice );
            //TemplateContext.GlobalFilters.AddFilter( "Split", global::Fluid.Filters.StringFilters.Split );
            //TemplateContext.GlobalFilters.AddFilter( "StripNewlines", global::Fluid.Filters.StringFilters.StripNewLines );
            //TemplateContext.GlobalFilters.AddFilter( "Truncate", global::Fluid.Filters.StringFilters.Truncate );
            //TemplateContext.GlobalFilters.AddFilter( "Truncatewords", global::Fluid.Filters.StringFilters.TruncateWords );
            //TemplateContext.GlobalFilters.AddFilter( "Upcase", global::Fluid.Filters.StringFilters.Upcase );
        }

        public override Type GetShortcodeType( string name )
        {
            throw new NotImplementedException();
        }

        public override void RegisterSafeType( Type type, string[] allowedMembers = null )
        {
            throw new NotImplementedException();
        }

        public override void RegisterShortcode( IRockShortcode shortcode )
        {
            //Tag WrapperFactoryMethod( string shortcodeName )
            //{
            //    return shortcode as Tag;
            //};

            // TODO: We need to register the shortcode using Reflection to access the private variable Template.shortcodes.
            //Template.RegisterShortcodeFactory( shortcode.Name, WrapperFactoryMethod );

            // Create an instance of the shortcode object and register the Type with DotLiquid.
            //var shortcodeObject = shortcodeFactoryMethod.Invoke( name );

            ////Tag shortcode = ( Tag ) Activator.CreateInstance( shortcodeType );

            //var shortcodesCollectionInfo = typeof( Template ).GetProperty( "Shortcodes", BindingFlags.Static | BindingFlags.NonPublic );

            //var shortcodesCollection = shortcodesCollectionInfo.GetValue( null ) as Dictionary<string, Type>;

            //shortcodesCollection.Add( name, shortcodeObject.GetType() );


            //Template.RegisterShortcode<DynamicShortcodeBlock>( shortcode.TagName );
            //Template.RegisterShortcode(<>
            //Template.RegisterShortcode<DynamicShortcodeBlock>( shortcode.TagName );

            //throw new NotImplementedException();
        }

        public override void RegisterShortcode( string name, Func<string, IRockShortcode> shortcodeFactoryMethod )
        {
            //Tag WrapperFactoryMethod(string shortcodeName )
            //{
            //    return shortcodeFactoryMethod( shortcodeName ) as Tag;
            //};

            // TODO: We need to register the shortcode using Reflection to access the private variable Template.shortcodes.
            //Template.regis .RegisterShortcodeFactory( name, WrapperFactoryMethod );

            // Create an instance of the shortcode object and register the Type with DotLiquid.
            //var shortcodeObject = shortcodeFactoryMethod.Invoke( name );

            ////Tag shortcode = ( Tag ) Activator.CreateInstance( shortcodeType );

            //var shortcodesCollectionInfo = typeof( Template ).GetProperty( "Shortcodes", BindingFlags.Static | BindingFlags.NonPublic );

            //var shortcodesCollection = shortcodesCollectionInfo.GetValue( null ) as Dictionary<string, Type>;

            //shortcodesCollection.Add( name, shortcodeObject.GetType() );


            //Template.RegisterShortcode<DynamicShortcodeBlock>( shortcode.TagName );
            //Template.RegisterShortcode(<>
            //Template.RegisterShortcode<DynamicShortcodeBlock>( shortcode.TagName );

            //throw new NotImplementedException();
        }

        public void RegisterShortcode<T>( string name )
            where T : IRockShortcode
        {
            var shortcodesCollectionInfo = typeof( Template ).GetProperty( "Shortcodes", BindingFlags.Static | BindingFlags.NonPublic );

            var shortcodesCollection = shortcodesCollectionInfo.GetValue( null ) as Dictionary<string, Type>;

            shortcodesCollection.Add( name, typeof( T ) );
        }

        public override void SetContextValue( string key, object value )
        {
            throw new NotImplementedException();
        }

        public override bool TryRender( string inputTemplate, out string output )
        {
            throw new NotImplementedException();
        }

        public override void UnregisterShortcode( string name )
        {
            throw new NotImplementedException();
        }

        public override ILavaTemplate ParseTemplate( string inputTemplate )
        {
            var template = Template.Parse( inputTemplate );

            /* 
             * 2/19/2020 - JPH
             * The DotLiquid library's Template object was not originally designed to be thread safe, but a PR has since
             * been merged into that repository to add this functionality (https://github.com/dotliquid/dotliquid/pull/220).
             * We have cherry-picked the PR's changes into our DotLiquid project, allowing the Template to operate safely
             * in a multithreaded context, which can happen often with our cached Template instances.
             *
             * Reason: Rock Issue #4084, Weird Behavior with Lava Includes
             */
            template.MakeThreadSafe();

            return new DotLiquidLavaTemplate( template );
        }
    }
}
