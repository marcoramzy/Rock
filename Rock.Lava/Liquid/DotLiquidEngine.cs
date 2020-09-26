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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DotLiquid;
using Rock.Lava.Filters;

namespace Rock.Lava.DotLiquid
{
    /// <summary>
    /// Initialization class for the DotLiquid Templating Engine.
    /// </summary>
    public class DotLiquidEngine : LavaEngineBase
    {
        public bool ThrowExceptions { get; set; } = true;

        public override string EngineName
        {
            get
            {
                return "DotLiquid";
            }
        }

        public override LavaEngineTypeSpecifier EngineType
        {
            get
            {
                return LavaEngineTypeSpecifier.DotLiquid;
            }
        }

        /// <summary>
        /// Initializes Rock's Lava system (which uses DotLiquid)
        /// Doing this in startup will force the static Liquid class to get instantiated
        /// so that the standard filters are loaded prior to the custom RockFilter.
        /// This is to allow the custom 'Date' filter to replace the standard Date filter.
        /// </summary>
        public void Initialize( ILavaFileSystem fileSystem, IList<Type> filterImplementationTypes = null )
        {
            // DotLiquid uses a RubyDateFormat by default,
            // but since we aren't using Ruby, we want to disable that
            Liquid.UseRubyDateFormat = false;

            /* 2020-05-20 MDP (actually this comment was here a long time ago)
                NOTE: This means that all the built in template filters,
                and the RockFilters, will use CSharpNamingConvention.

                For example the dotliquid documentation says to do this for formatting dates: 
                {{ some_date_value | date:"MMM dd, yyyy" }}

                However, if CSharpNamingConvention is enabled, it needs to be: 
                {{ some_date_value | Date:"MMM dd, yyyy" }}
            */

            Template.NamingConvention = new global::DotLiquid.NamingConventions.CSharpNamingConvention();

            Template.FileSystem = new DotLiquidLavaFileSystem( fileSystem );

            Template.RegisterSafeType( typeof( Enum ), o => o.ToString() );
            Template.RegisterSafeType( typeof( DBNull ), o => null );

            Template.RegisterFilter( typeof( TemplateFilters ) );
            Template.RegisterFilter( typeof( DotLiquidFilters ) );

            Template.FilterContextParameterType = typeof( ILavaContext );
            Template.FilterContextParameterTransformer = ( context ) =>
            {
                // Wrap the DotLiquid context in a framework-agnostic Lava context.
                return new DotLiquidLavaContext( context as Context );
            };

            // Register custom filters last, so they can override built-in filters of the same name.
            if ( filterImplementationTypes != null )
            {
                foreach ( var filterImplementationType in filterImplementationTypes )
                {
                    Template.RegisterFilter( filterImplementationType );
                }
            }

            Template.RegisterSafeType( typeof( Rock.Lava.ILiquidizable ), ( x ) => { return ( (Rock.Lava.ILiquidizable)x ).ToLiquid(); } );
            Template.RegisterSafeType( typeof( Rock.Lava.ILavaDataObject ), ( x ) => { return ( (ILavaDataObject)x ).ToLiquid(); } );
        }

        private void RegisterFilterInternal( Type filterImplementationType )
        {
        }

        public override Type GetShortcodeType( string name )
        {
            throw new NotImplementedException();
        }

        public override void RegisterSafeType( Type type, string[] allowedMembers = null )
        {
            if ( type is Rock.Lava.ILiquidizable )
            {
                Template.RegisterSafeType( type, ( x ) => { return ( (Rock.Lava.ILiquidizable)x ).ToLiquid(); } );
            }
            else if ( type is Rock.Lava.ILiquidizable )
            {
                Template.RegisterSafeType( typeof( Rock.Lava.ILavaDataObject ), ( x ) => { return ( (ILavaDataObject)x ).ToLiquid(); } );
            }
            else
            {
                // Wrap the object in a RockDynamic proxy.
                Template.RegisterSafeType( type, ( x ) => { return new RockDynamic( x ).ToLiquid(); } );
            }
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

        public override void RegisterShortcode<T>( string name )
        {
            var shortcodesCollectionInfo = typeof( Template ).GetProperty( "Shortcodes", BindingFlags.Static | BindingFlags.NonPublic );

            var shortcodesCollection = shortcodesCollectionInfo.GetValue( null ) as Dictionary<string, Type>;

            shortcodesCollection.Add( name, typeof( T ) );
        }

        public override void SetContextValue( string key, object value )
        {
            throw new NotImplementedException();
        }

        private object GetDotLiquidCompatibleValue( object value )
        {
            if ( value == null
                 || value is string
                 || value is IEnumerable
                 || value is decimal
                 || value is DateTime
                 || value is DateTimeOffset
                 || value is TimeSpan
                 || value is Guid
                 || value is Enum
                 || value is KeyValuePair<string, object>
                 || value.GetType().IsPrimitive
                 )
            {
                return value;
            }

            var safeTypeTransformer = Template.GetSafeTypeTransformer( value.GetType() );

            if ( safeTypeTransformer != null )
            {
                return safeTypeTransformer( value );
            }

            return value;
        }

        public override bool TryRender( string inputTemplate, out string output, LavaDictionary mergeValues )
        {
            try
            {
                var template = CreateNewDotLiquidTemplate( inputTemplate );

                if ( mergeValues != null )
                {
                    object fieldValue;

                    // We want to avoid DotLiquid wrapping any values as a DropProxy object,
                    // because we want to avoid any framework-specific Types being passed to our framework-agnostic filters.
                    foreach ( var key in mergeValues.Keys.ToList() )
                    {
                        fieldValue = GetDotLiquidCompatibleValue( mergeValues[key] );

                        mergeValues[key] = fieldValue;
                    }

                    output = template.Render( Hash.FromDictionary( mergeValues ) );
                }
                else
                {
                    output = template.Render();
                }

                return true;
            }
            catch ( Exception ex )
            {
                ProcessException( ex );

                output = null;
                return false;
            }
        }

        private void ProcessException( Exception ex )
        {
            if ( this.ThrowExceptions )
            {
                throw ex;
            }
        }

        public override void UnregisterShortcode( string name )
        {
            throw new NotImplementedException();
        }

        private Template CreateNewDotLiquidTemplate( string inputTemplate )
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

            return template;
        }

        public override ILavaTemplate ParseTemplate( string inputTemplate )
        {
            return new DotLiquidLavaTemplate( CreateNewDotLiquidTemplate( inputTemplate ) );
        }

        public override bool AreEqualValue( object left, object right )
        {
            var condition = global::DotLiquid.Condition.Operators["=="];

            return condition( left, right );
        }

        public override Dictionary<string, ILavaTagInfo> GetRegisteredTags()
        {
            return Template.Tags.ToDictionary( k => k.Key, v => (ILavaTagInfo)( new LavaTagInfo { Name = v.Key, SystemTypeName = v.Value.Name } ) );
        }

    }
}
