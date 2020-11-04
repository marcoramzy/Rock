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
using Fluid;
using Fluid.Tags;
using Fluid.Values;
using Irony.Parsing;
using Rock.Lava;

namespace Rock.Lava.Fluid
{    
    public class FluidEngine : LavaEngineBase // ILavaEngine
    {
        //public string FrameworkName
        //{
        //    get
        //    {
        //        return "Fluid";
        //    }
        //}

        public override string EngineName
        {
            get
            {
                return "Fluid";
            }
        }

        public override LavaEngineTypeSpecifier EngineType
        {
            get
            {
                return LavaEngineTypeSpecifier.Fluid;
            }
        }

        public override ILavaContext NewContext()
        {
            var fluidContext = new global::Fluid.TemplateContext();

            // Set the transformation function for converting a Lava Context to a Fluid TemplateContext.
            // This is needed to allow the Lava context parameter in a filter function to be identified and 
            // injected in a way that is framework-agnostic.
            //fluidContext.FilterContextParameterType = Template.FilterContextParameterType;
            //fluidContext.FilterContextParameterTransformer = Template.FilterContextParameterTransformer;

            return new FluidLavaContext( fluidContext );
        }

        /// <summary>
        /// Initializes Rock's Lava system (which uses DotLiquid)
        /// Doing this in startup will force the static Liquid class to get instantiated
        /// so that the standard filters are loaded prior to the custom RockFilter.
        /// This is to allow the custom 'Date' filter to replace the standard Date filter.
        /// </summary>
        public override void Initialize( ILavaFileSystem fileSystem, IList<Type> filterImplementationTypes = null )
        {
            // Re-register the basic Liquid filters implemented by Fluid using CamelCase rather than the default snakecase.
            HideSnakeCaseFilters();
            RegisterBaseFilters();

            //
            // Register the Rock filters.
            //
            //TemplateContext.GlobalFilters.RegisterFiltersFromType( typeof( Filters.FluidFilters ) );
            //RegisterLegacyFilters( Rock.)

            // Set the default strategy for locating object properties to our custom implementation that adds
            // the ability to resolve properties of nested anonymous Types using Reflection.
            TemplateContext.GlobalMemberAccessStrategy = new DynamicMemberAccessStrategy();

        }
        /// <summary>
        /// This method hides the snake-case filters that are registered
        /// by default. Rock uses CamelCase filter names and to ensure that
        /// a mistype doesn't cause it to work anyway we hide these.
        /// </summary>
        private static void HideSnakeCaseFilters()
        {
            TemplateContext.GlobalFilters.AddFilter( "join", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "first", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "last", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "concat", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "map", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "reverse", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "size", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "sort", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "sort_natural", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "uniq", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "where", NoOp );

            TemplateContext.GlobalFilters.AddFilter( "default", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "date", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "format_date", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "raw", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "compact", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "url_encode", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "url_decode", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "strip_html", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "escape", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "escape_once", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "handle", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "handleize", NoOp );

            TemplateContext.GlobalFilters.AddFilter( "abs", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "at_least", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "at_most", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "ceil", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "divided_by", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "floor", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "minus", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "modulo", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "plus", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "round", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "times", NoOp );

            TemplateContext.GlobalFilters.AddFilter( "append", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "capitalize", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "downcase", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "lstrip", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "rstrip", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "newline_to_br", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "prepend", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "removefirst", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "remove", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "replacefirst", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "replace", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "slice", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "split", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "strip", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "strip_newlines", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "truncate", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "truncatewords", NoOp );
            TemplateContext.GlobalFilters.AddFilter( "upcase", NoOp );
        }

        /// <summary>
        /// Registers all the base Fluid filters with the proper CamelCase.
        /// </summary>
        private static void RegisterBaseFilters()
        {
            TemplateContext.GlobalFilters.AddFilter( "Join", global::Fluid.Filters.ArrayFilters.Join );
            TemplateContext.GlobalFilters.AddFilter( "First", global::Fluid.Filters.ArrayFilters.First );
            TemplateContext.GlobalFilters.AddFilter( "Last", global::Fluid.Filters.ArrayFilters.Last );
            TemplateContext.GlobalFilters.AddAsyncFilter( "Map", global::Fluid.Filters.ArrayFilters.Map );
            TemplateContext.GlobalFilters.AddFilter( "Reverse", global::Fluid.Filters.ArrayFilters.Reverse );
            TemplateContext.GlobalFilters.AddAsyncFilter( "Size", global::Fluid.Filters.ArrayFilters.Size );
            TemplateContext.GlobalFilters.AddAsyncFilter( "Sort", global::Fluid.Filters.ArrayFilters.Sort );
            TemplateContext.GlobalFilters.AddFilter( "Uniq", global::Fluid.Filters.ArrayFilters.Uniq );
            TemplateContext.GlobalFilters.AddAsyncFilter( "Where", global::Fluid.Filters.ArrayFilters.Where );

            TemplateContext.GlobalFilters.AddFilter( "Default", global::Fluid.Filters.MiscFilters.Default );
            TemplateContext.GlobalFilters.AddFilter( "Date", global::Fluid.Filters.MiscFilters.Date );
            TemplateContext.GlobalFilters.AddFilter( "UnescapeDataString", global::Fluid.Filters.MiscFilters.UrlDecode );
            TemplateContext.GlobalFilters.AddFilter( "EscapeDataString", global::Fluid.Filters.MiscFilters.UrlEncode );
            TemplateContext.GlobalFilters.AddFilter( "StripHtml", global::Fluid.Filters.MiscFilters.StripHtml );
            TemplateContext.GlobalFilters.AddFilter( "Escape", global::Fluid.Filters.MiscFilters.Escape );

            TemplateContext.GlobalFilters.AddFilter( "AtLeast", global::Fluid.Filters.NumberFilters.AtLeast );
            TemplateContext.GlobalFilters.AddFilter( "AtMost", global::Fluid.Filters.NumberFilters.AtMost );
            TemplateContext.GlobalFilters.AddFilter( "Ceiling", global::Fluid.Filters.NumberFilters.Ceil );
            TemplateContext.GlobalFilters.AddFilter( "DividedBy", global::Fluid.Filters.NumberFilters.DividedBy );
            TemplateContext.GlobalFilters.AddFilter( "Floor", global::Fluid.Filters.NumberFilters.Floor );
            TemplateContext.GlobalFilters.AddFilter( "Minus", global::Fluid.Filters.NumberFilters.Minus );
            TemplateContext.GlobalFilters.AddFilter( "Modulo", global::Fluid.Filters.NumberFilters.Modulo );
            TemplateContext.GlobalFilters.AddFilter( "Plus", global::Fluid.Filters.NumberFilters.Plus );
            TemplateContext.GlobalFilters.AddFilter( "Times", global::Fluid.Filters.NumberFilters.Times );

            TemplateContext.GlobalFilters.AddFilter( "Append", global::Fluid.Filters.StringFilters.Append );
            TemplateContext.GlobalFilters.AddFilter( "Capitalize", global::Fluid.Filters.StringFilters.Capitalize );
            TemplateContext.GlobalFilters.AddFilter( "Downcase", global::Fluid.Filters.StringFilters.Downcase );
            TemplateContext.GlobalFilters.AddFilter( "NewlineToBr", global::Fluid.Filters.StringFilters.NewLineToBr );
            TemplateContext.GlobalFilters.AddFilter( "Prepend", global::Fluid.Filters.StringFilters.Prepend );
            TemplateContext.GlobalFilters.AddFilter( "RemoveFirst", global::Fluid.Filters.StringFilters.RemoveFirst );
            TemplateContext.GlobalFilters.AddFilter( "Remove", global::Fluid.Filters.StringFilters.Remove );
            TemplateContext.GlobalFilters.AddFilter( "ReplaceFirst", global::Fluid.Filters.StringFilters.ReplaceFirst );
            TemplateContext.GlobalFilters.AddFilter( "Replace", global::Fluid.Filters.StringFilters.Replace );
            TemplateContext.GlobalFilters.AddFilter( "Slice", global::Fluid.Filters.StringFilters.Slice );
            TemplateContext.GlobalFilters.AddFilter( "Split", global::Fluid.Filters.StringFilters.Split );
            TemplateContext.GlobalFilters.AddFilter( "StripNewlines", global::Fluid.Filters.StringFilters.StripNewLines );
            TemplateContext.GlobalFilters.AddFilter( "Truncate", global::Fluid.Filters.StringFilters.Truncate );
            TemplateContext.GlobalFilters.AddFilter( "Truncatewords", global::Fluid.Filters.StringFilters.TruncateWords );
            TemplateContext.GlobalFilters.AddFilter( "Upcase", global::Fluid.Filters.StringFilters.Upcase );
        }

        /// <summary>
        /// Registers a set of Liquid-style filter functions for use with the Fluid templating engine.
        /// The original filters are wrapped in a function with a Fluid-compatible signature so they can be called by Fluid.
        /// </summary>
        /// <param name="type">The type that contains the Liquid filter functions.</param>
        public static void RegisterLegacyFilters( Type type )
        {
            var methods = type.GetMethods( System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public );

            foreach ( var method in methods )
            {
                var parameters = method.GetParameters();

                // Define the Fluid-compatible filter function that will wrap the legacy filter method.
                FluidValue LegacyFilter( FluidValue input, FilterArguments arguments, TemplateContext context )
                {
                    var p = new object[parameters.Length];

                    for ( int i = 0; i < parameters.Length; i++ )
                    {
                        FluidValue arg = null;

                        if ( i == 0 )
                        {
                            arg = input;
                        }
                        else if ( arguments.Count > ( i - 1 ) )
                        {
                            arg = arguments.At( i - 1 );
                        }

                        if ( arg == null && parameters[i].IsOptional )
                        {
                            p[i] = parameters[i].DefaultValue;
                        }
                        else
                        {
                            if ( parameters[i].ParameterType == typeof( string ) )
                            {
                                p[i] = arg.ToStringValue();
                            }
                            else if ( parameters[i].ParameterType == typeof( int ) )
                            {
                                p[i] = (int)arg.ToNumberValue();
                            }
                            else if ( parameters[i].ParameterType == typeof( bool ) )
                            {
                                p[i] = arg.ToBooleanValue();
                            }
                            else if ( parameters[i].ParameterType == typeof( object ) )
                            {
                                p[i] = arg.ToObjectValue();
                            }
                            else
                            {
                                throw new ArgumentOutOfRangeException( parameters[i].Name, $"Parameter type '{parameters[i].ParameterType.Name}' is not supported for legacy filters." );
                            }
                        }
                    }

                    var result = method.Invoke( null, p );

                    return FluidValue.Create( result );
                }



                //
                // Skip any filters that require the DotLiquid context.
                //
                if ( parameters.Length >= 1 && parameters[0].ParameterType.FullName == "DotLiquid.Context" )
                {
                    continue;
                }

                TemplateContext.GlobalFilters.AddFilter( method.Name, LegacyFilter );
            }
        }



        /// <summary>
        /// Performs a no-operation filter. Just return the input. This simulates using
        /// a filter that doesn't exist.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="context">The context.</param>
        /// <returns></returns>
        private static FluidValue NoOp( FluidValue input, FilterArguments arguments, TemplateContext context )
        {
            return input;
        }

        public override bool AreEqualValue( object left, object right )
        {
            throw new NotImplementedException();
        }

        //public Type GetShortcodeType( string name )
        //{
        //    throw new NotImplementedException();
        //}

        public override Type GetShortcodeType( string name )
        {
            throw new NotImplementedException();
        }

        public override ILavaTemplate ParseTemplate( string inputTemplate )
        {
            throw new NotImplementedException();
        }

        //public void RegisterSafeType( Type type, string[] allowedMembers = null )
        //{
        //    throw new NotImplementedException();
        //}

        public override void RegisterSafeType( Type type, string[] allowedMembers = null )
        {
            throw new NotImplementedException();
        }

        //private static _shortcodeList<string, >
        //public void RegisterShortcode( string name, Func<string, IRockShortcode> shortcodeFactoryMethod )
        //{
        //    var shortcode = shortcodeFactoryMethod( name ) as IRockShortcode;

        //    RegisterShortcode( shortcode );
        //}

        //public void RegisterShortcode( IRockShortcode shortcode )
        //{
        //    if ( shortcode == null )
        //    {
        //        return;
        //    }

        //    var fluidTag = shortcode as ITag;

        //    if ( fluidTag == null )
        //    {
        //        throw new Exception( "Shortcode object is invalid. Shortcode must implement FluidEngine.ITag interface." );
        //    }

        //    //var tagName = "#" + shortcode.Name;

        //    if ( shortcode.ElementType == LavaElementTypeSpecifier.Inline )
        //    {
        //        LavaFluidTemplate.Factory.RegisterTag( shortcode.Name, fluidTag );
        //    }
        //    else
        //    {
        //        LavaFluidTemplate.Factory.RegisterBlock( shortcode.Name, fluidTag );
        //    }

        //}

        //public void RegisterBlock( string name, ITag tag )
        //{
        //    // Use Reflection to modify grammar.
        //    var parserFactory = FluidTemplate.Factory;

        //    var grammerInternalInfo = parserFactory.GetType().GetField( "_grammar", System.Reflection.BindingFlags.NonPublic );

        //    var _grammar = grammerInternalInfo.GetValue( parserFactory ) as FluidGrammar;

        //    lock ( _grammar )
        //    {
        //        _languageData = null;
        //        _blocks[name] = tag;

        //        _tags[name] = tag;

        //        // Configure the grammar to add support for the custom syntax

        //        var terminal = new NonTerminal( name )
        //        {
        //            Rule = _grammar.ToTerm( name ) + tag.GetSyntax( _grammar )
        //        };

        //        _grammar.KnownTags.Rule |= terminal;

        //        // Prevent the text from being added in the parsed tree.
        //        _grammar.MarkPunctuation( name );
        //    }

        //}

        //public void SetContextValue( string key, object value )
        //{
        //    throw new NotImplementedException();
        //}

        //public bool TryRender( string inputTemplate, out string output )
        //{
        //    throw new NotImplementedException();
        //}

        public override bool TryRender( string inputTemplate, out string output, ILavaContext context )
        {
            throw new NotImplementedException();
        }

        public override void UnregisterShortcode( string name )
        {
            throw new NotImplementedException();
        }
    }

    #region Member Access Strategy implementation

    /// <summary>
    /// An implementation of a MemberAccessStategy for the Fluid framework, modified to support anonymous types.
    /// This implementation closely resembles the default implementation supplied by the Fluid framework.
    /// </summary>
    public class DynamicMemberAccessStrategy : IMemberAccessStrategy
    {
        private Dictionary<Type, Dictionary<string, IMemberAccessor>> _map;
        private readonly IMemberAccessStrategy _parent;

        private DynamicMemberAccessor _dynamicMemberAccessor = new DynamicMemberAccessor();

        public DynamicMemberAccessStrategy()
        {
            _map = new Dictionary<Type, Dictionary<string, IMemberAccessor>>();
        }

        public DynamicMemberAccessStrategy( IMemberAccessStrategy parent ) : this()
        {
            _parent = parent;
            MemberNameStrategy = _parent.MemberNameStrategy;
        }

        public MemberNameStrategy MemberNameStrategy { get; set; } = MemberNameStrategies.Default;

        public bool IgnoreCasing { get; set; }

        public IMemberAccessor GetAccessor( Type type, string name )
        {
            IMemberAccessor accessor = null;

            /*
             * [2020-05-05] DL
             * To access the members of an anonymous Type, we need to use reflection.
             * We need to reimplement the entire MemberAccessStrategy to achieve this rather than simply register a new accessor via the Register() method,
             * because the Type being accessed is not known in advance.
             * This check for an anonymous type is fairly naive, but it works correctly for the .Net and Mono frameworks.
             * Refer https://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous.
             */
            if ( type.Name.Contains( "AnonymousType" ) || type.Name.Contains( "AnonType" ) )
            {
                return _dynamicMemberAccessor;
            }

            while ( type != typeof( object ) )
            {
                // Look for specific property map
                if ( _map.TryGetValue( type, out var typeMap ) )
                {
                    if ( typeMap.TryGetValue( name, out accessor ) || typeMap.TryGetValue( "*", out accessor ) )
                    {
                        return accessor;
                    }
                }

                accessor = accessor ?? _parent?.GetAccessor( type, name );

                if ( accessor != null )
                {
                    return accessor;
                }

                type = type.GetTypeInfo().BaseType;
            }

            return null;
        }

        public void Register( Type type, string name, IMemberAccessor getter )
        {
            if ( !_map.TryGetValue( type, out var typeMap ) )
            {
                typeMap = new Dictionary<string, IMemberAccessor>( IgnoreCasing
                    ? StringComparer.OrdinalIgnoreCase
                    : StringComparer.Ordinal );

                _map[type] = typeMap;
            }

            typeMap[name] = getter;
        }
    }

    /// <summary>
    /// A Fluid Engine Member Accessor that can retrieve the value of a member of an anonymously-typed object.
    /// </summary>
    public class DynamicMemberAccessor : IMemberAccessor
    {
        public object Get( object obj, string name, TemplateContext ctx )
        {
            return GetPropertyPathValue( obj, name );
        }

        private object GetPropertyPathValue( object obj, string propertyPath )
        {
            if ( string.IsNullOrWhiteSpace( propertyPath ) )
            {
                return obj;
            }

            return Rock.Common.ExtensionMethods.GetPropertyValue( obj, propertyPath );
        }
    }

    #endregion

}
