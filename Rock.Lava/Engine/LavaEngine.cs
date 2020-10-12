using System;
using System.Collections.Generic;
using Rock.Lava.DotLiquid;
using Rock.Lava.Blocks;
using System.IO;

namespace Rock.Lava
{
    /// <summary>
    /// Provides access to core functions for the Rock Lava Engine.
    /// </summary>
    public static class LavaEngine
    {
        private static ILavaEngine _instance = null;
        private static LavaEngineTypeSpecifier _liquidFramework = LavaEngineTypeSpecifier.DotLiquid;

        public static LavaEngineTypeSpecifier LiquidFramework
        {
            get
            {
                return _liquidFramework;
            }
            set
            {
                if ( _liquidFramework != value )
                {
                    _liquidFramework = value;

                    // Reset the existing instance so it can be re-created on next access.
                    _instance = null;
                }
            }
        }

        public static void InitializeDotLiquidFramework( ILavaFileSystem fileSystem = null, IList<Type> filterImplementationTypes = null )
        {
            _liquidFramework = LavaEngineTypeSpecifier.DotLiquid;

            var liquidEngine = new DotLiquidEngine();

            liquidEngine.Initialize( fileSystem, filterImplementationTypes );

            _instance = liquidEngine;
        }
        
        public static ILavaEngine Instance
        {
            get
            {
                if ( _instance == null )
                {
                    if ( _liquidFramework == LavaEngineTypeSpecifier.DotLiquid )
                    {
                        InitializeDotLiquidFramework();
                    }
                    else
                    {
                        throw new Exception( "Liquid Framework not implemented." );
                        // TODO: Add option to instantiate Fluid engine.
                    }                    
                }

                return _instance;
            }
        }
    }

    //TODO: Implement IRockStartup.
    public abstract class LavaEngineBase : ILavaEngine // ,IRockStartup
    {
        public abstract string EngineName { get; }

        public abstract ILavaContext NewContext();

        public abstract LavaEngineTypeSpecifier EngineType { get; }

        public abstract Type GetShortcodeType( string name );

        public abstract void RegisterSafeType( Type type, string[] allowedMembers = null );

        public abstract void RegisterStaticShortcode( string name, Func<string, IRockShortcode> shortcodeFactoryMethod );
        public abstract void RegisterDynamicShortcode( string name, Func<string, DynamicShortcodeDefinition> shortcodeFactoryMethod );

        public abstract void RegisterShortcode( IRockShortcode shortcode );
        public abstract void RegisterShortcode<T>( string name )
            where T : IRockShortcode;

        public abstract void SetContextValue( string key, object value );

        public bool TryRender( string inputTemplate, out string output )
        {
            return TryRender( inputTemplate, out output, context: null );
        }

        public bool TryRender( string inputTemplate, out string output, LavaDictionary mergeValues )
        {
            var context = NewContext();

            context.SetMergeFieldValues( mergeValues );

            return TryRender( inputTemplate, out output, context );
        }

        public abstract bool TryRender( string inputTemplate, out string output, ILavaContext context );

    public abstract void UnregisterShortcode( string name );

        public abstract bool AreEqualValue( object left, object right );
        //

        /// <summary>
        /// Method that will be run at Rock startup
        /// </summary>
        public void OnStartup()
        {
            RegisterLavaShortcodeBlocks();
        }

        private void RegisterLavaShortcodeBlocks()
        {
            // get all the block dynamic shortcodes and register them
            //var blockShortCodes = LavaShortcodeCache.All().Where( s => s.TagType == TagType.Block );

            //foreach ( var shortcode in blockShortCodes )
            //{
            //    // register this shortcode
            //    Template.RegisterShortcode<DynamicShortcodeBlock>( shortcode.TagName );
            //}

        }

        private void RegisterLavaShortCodeInlineTags()
        {
            // get all the block dynamic shortcodes and register them
            //var blockShortCodes = LavaShortcodeCache.All().Where( s => s.TagType == TagType.in.Block );

            //foreach ( var shortcode in blockShortCodes )
            //{
            //    // register this shortcode
            //    Template.RegisterShortcode<DynamicShortcodeBlock>( shortcode.TagName );
            //}
        }

        public bool TryParseTemplate( string inputTemplate, out ILavaTemplate template )
        {
            try
            {
                template = this.ParseTemplate( inputTemplate );
                return true;
            }
            catch
            {
                template = null;
                return false;
            }
        }

        public abstract ILavaTemplate ParseTemplate( string inputTemplate );

        public abstract void RegisterTag( string name, Func<string, IRockLavaTag> factoryMethod );

        public abstract void RegisterBlock( string name, Func<string, IRockLavaBlock> factoryMethod );

        public abstract Dictionary<string, ILavaTagInfo> GetRegisteredTags();

        public abstract void RenderTag( IRockLavaBlock tag, ILavaContext context, TextWriter result );
    }
}