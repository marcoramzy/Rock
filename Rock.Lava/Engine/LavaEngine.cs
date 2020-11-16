using System;
using System.Collections.Generic;
using Rock.Lava.DotLiquid;
using Rock.Lava.Blocks;
using System.IO;
using Rock.Lava.Fluid;

namespace Rock.Lava
{
    /// <summary>
    /// Provides access to core functions for the Rock Lava Engine.
    /// </summary>
    public static class LavaEngine
    {
        public static string ShortcodeNameSuffix = "_sc";

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

        public static void Initialize( LavaEngineTypeSpecifier? engineType, ILavaFileSystem fileSystem = null, IList<Type> filterImplementationTypes = null )
        {
            _liquidFramework = engineType ?? LavaEngineTypeSpecifier.DotLiquid;

            ILavaEngine engine;

            if ( _liquidFramework == LavaEngineTypeSpecifier.Fluid )
            {
                engine = new FluidEngine();
            }
            else
            {
                engine = new DotLiquidEngine();
            }

            engine.Initialize( fileSystem, filterImplementationTypes );

            _instance = engine;
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
}