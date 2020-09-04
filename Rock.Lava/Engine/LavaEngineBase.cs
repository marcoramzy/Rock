using System;
using System.Collections.Generic;
using System.Text;
using Rock.Lava.Shortcodes;
using Rock.Lava.DotLiquid;
using Rock.Lava.Blocks;

namespace Rock.Lava
{
    /// <summary>
    /// Provides access to core functions for the Rock Lava Engine.
    /// </summary>
    public static class LavaEngine
    {
        private static ILavaEngine _instance = null;

        public static ILavaEngine Instance
        {
            get
            {
                if ( _instance == null )
                {
                    // TODO: Add option to instantiate Fluid engine.
                    _instance = new DotLiquidEngineManager();
                }

                return _instance;
            }
        }
    }

    //TODO: Implement IRockStartup.
    public abstract class LavaEngineBase : ILavaEngine // ,IRockStartup
    {
        public abstract string FrameworkName { get; }

        public abstract Type GetShortcodeType( string name );

        public abstract void RegisterSafeType( Type type, string[] allowedMembers = null );

        public abstract void RegisterShortcode( string name, Func<string, IRockShortcode> shortcodeFactoryMethod );

        public abstract void RegisterShortcode( IRockShortcode shortcode );

        public abstract void SetContextValue( string key, object value );

        public abstract bool TryRender( string inputTemplate, out string output );

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
            catch ( Exception ex )
            {
                template = null;
                return false;
            }
        }

        public abstract ILavaTemplate ParseTemplate( string inputTemplate );

        public void RegisterTag( string name, Func<string, IRockLavaTag> factoryMethod )
        {
            throw new NotImplementedException();
        }

        public void RegisterBlock( string name, Func<string, IRockLavaBlock> factoryMethod )
        {
            throw new NotImplementedException();
        }

        public abstract Dictionary<string, ILavaTagInfo> GetRegisteredTags();
    }
}