using System;
using System.Collections.Generic;
using Rock.Lava.Shortcodes;

namespace Rock.Lava
{
    //TODO: Implement IRockStartup.
    public abstract class LavaEngineBase : ILavaEngine // ,IRockStartup
    {
        public abstract void Initialize( ILavaFileSystem fileSystem, IList<Type> filterImplementationTypes = null );

        public abstract string EngineName { get; }

        public abstract ILavaContext NewContext();

        public abstract LavaEngineTypeSpecifier EngineType { get; }

        public abstract Type GetShortcodeType( string name );

        public abstract void RegisterSafeType( Type type, string[] allowedMembers = null );

        public void RegisterStaticShortcode( Func<string, IRockShortcode> shortcodeFactoryMethod )
        {
            var instance = shortcodeFactoryMethod( "default" );

            if ( instance == null )
            {
                throw new Exception( "Shortcode factory could not provide a valid instance for \"default\"." );
            }

            RegisterStaticShortcode( instance.SourceElementName, shortcodeFactoryMethod );
        }

        public void RegisterStaticShortcode( string name, Func<string, IRockShortcode> shortcodeFactoryMethod )
        {
            var instance = shortcodeFactoryMethod( name );

            if ( instance == null )
            {
                throw new Exception( $"Shortcode factory could not provide a valid instance for \"{name}\" ." );
            }

            // Get a registration name for the shortcode that will not collide with an existing tag name.
            var registrationKey = GetShortcodeRegistrationKey( name );

            if ( instance.ElementType == LavaShortcodeTypeSpecifier.Inline )
            {
                var tagFactoryMethod = shortcodeFactoryMethod as Func<string, IRockLavaTag>;

                this.RegisterTag( registrationKey, tagFactoryMethod );
            }
            else
            {
                this.RegisterBlock( registrationKey,  ( blockName ) =>
                {
                    // Get a shortcode instance using the provided shortcut factory.
                    var shortcode = shortcodeFactoryMethod( registrationKey );

                    // Return the shortcode instance as a RockLavaBlock
                    return shortcode as IRockLavaBlock;
                } );
                ;
            }
        }

        private string GetShortcodeRegistrationKey( string shortcodeName )
        {
            var internalName = shortcodeName + LavaEngine.ShortcodeNameSuffix;

            return internalName.Trim().ToLower();
        }

        //public abstract void RegisterStaticShortcode( string name, Func<string, IRockShortcode> shortcodeFactoryMethod );


        //public abstract void RegisterDynamicShortcode( string name, Func<string, DynamicShortcodeDefinition> shortcodeFactoryMethod );

        public void RegisterDynamicShortcode( string name, Func<string, DynamicShortcodeDefinition> shortcodeFactoryMethod )
        {
            var instance = shortcodeFactoryMethod( name );

            if ( instance == null )
            {
                throw new Exception( $"Shortcode factory could not provide a valid instance for \"{name}\" ." );
            }

            if ( instance.ElementType == LavaShortcodeTypeSpecifier.Inline )
            {
                // Create a new factory method that returns an initialized Shortcode Tag element.
                
                Func<string, IRockLavaTag> tagFactoryMethod = ( tagName ) =>
                {
                    var shortcodeInstance = GetShortcodeFromFactory<DynamicShortcodeTag>( tagName, shortcodeFactoryMethod );

                    return shortcodeInstance;

                    // Call the factory method we have been passed to retrieve the definition of the shortcode.
                    // The definition may change at runtime, so we need to execute the factory method every time we create a new shortcode instance.
                    //var shortcodeDefinition = shortcodeFactoryMethod( tagName );

                    //var shortcodeInstance = new DynamicShortcodeTag( shortcodeDefinition );

                    //return shortcodeInstance;
                };

                // Get a registration name for the shortcode that will not collide with an existing tag name.
                var registrationKey = GetShortcodeRegistrationKey( name );

                this.RegisterTag( registrationKey, tagFactoryMethod );
            }
            else
            {
                // Create a new factory method that returns an initialized Shortcode Block element.

                Func<string, IRockLavaBlock> blockFactoryMethod = ( blockName ) =>
                {
                    // Call the factory method we have been passed to retrieve the definition of the shortcode.
                    // The definition may change at runtime, so we need to execute the factory method every time we create a new shortcode instance.
                    var shortCodeName = blockName;

                    if ( shortCodeName.EndsWith( LavaEngine.ShortcodeNameSuffix ) )
                    {
                        shortCodeName = shortCodeName.Remove( shortCodeName.Length - LavaEngine.ShortcodeNameSuffix.Length );
                    }

                    var shortcodeDefinition = shortcodeFactoryMethod( shortCodeName );

                    var shortcodeInstance = new DynamicShortcodeBlock( shortcodeDefinition );

                    return shortcodeInstance;
                };

                // Get a registration name for the shortcode that will not collide with an existing tag name.
                var registrationKey = GetShortcodeRegistrationKey( name );

                this.RegisterBlock( registrationKey, blockFactoryMethod );



                //this.RegisterBlock( registrationKey, ( blockName ) =>
                //{
                //    // Get a shortcode instance using the provided shortcut factory.
                //    var shortcode = shortcodeFactoryMethod( registrationKey );

                //    // Return the shortcode instance as a RockLavaBlock
                //    return shortcode as IRockLavaBlock;
                //} );

            }
        }

        private T GetShortcodeFromFactory<T>( string shortcodeInternalName, Func<string, DynamicShortcodeDefinition> shortcodeFactoryMethod )
            where T : DynamicShortcode, new()
        {
            // Call the factory method we have been passed to retrieve the definition of the shortcode.
            // The definition may change at runtime, so we need to execute the factory method every time we create a new shortcode instance.
            var shortCodeName = shortcodeInternalName;

            if ( shortCodeName.EndsWith( LavaEngine.ShortcodeNameSuffix ) )
            {
                shortCodeName = shortCodeName.Remove( shortCodeName.Length - LavaEngine.ShortcodeNameSuffix.Length );
            }

            var shortcodeDefinition = shortcodeFactoryMethod( shortCodeName );

            var shortcodeInstance = new T();

            shortcodeInstance.Initialize( shortcodeDefinition );

            return shortcodeInstance;
        }


        //public abstract void RegisterShortcode( IRockShortcode shortcode );

        //public abstract void RegisterShortcode<T>( string name )
        //  where T : IRockShortcode;

        //public abstract void SetContextValue( string key, object value );

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

        public Dictionary<string, ILavaElementInfo> GetRegisteredElements()
        {
            var tags = new Dictionary<string, ILavaElementInfo>();

            foreach ( var tagWrapper in _lavaElements )
            {
                var info = new LavaTagInfo();

                info.Name = tagWrapper.Key;

                info.SystemTypeName = tagWrapper.Value.SystemTypeName;

                tags.Add( info.Name, info );
            }

            return tags;
        }    

        #region Tags

        private static Dictionary<string, ILavaElementInfo> _lavaElements = new Dictionary<string, ILavaElementInfo>( StringComparer.OrdinalIgnoreCase );

        public virtual void RegisterTag( string name, Func<string, IRockLavaTag> factoryMethod )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
            {
                throw new ArgumentException( "Name must be specified." );
            }

            name = name.Trim().ToLower();

            var tagInstance = factoryMethod( name );

            var tagInfo = new LavaTagInfo();

            tagInfo.Name = name;
            tagInfo.FactoryMethod = factoryMethod;

            tagInfo.IsAvailable = ( tagInstance != null );

            if ( tagInstance != null )
            {
                tagInfo.SystemTypeName = tagInstance.GetType().FullName;
            }

            _lavaElements[name] = tagInfo;
        }

        public virtual void RegisterBlock( string name, Func<string, IRockLavaBlock> factoryMethod )
        {
            if ( string.IsNullOrWhiteSpace( name ) )
            {
                throw new ArgumentException( "Name must be specified." );
            }

            name = name.Trim().ToLower();

            var blockInstance = factoryMethod( name );

            var blockInfo = new LavaBlockInfo();

            blockInfo.Name = name;
            blockInfo.FactoryMethod = factoryMethod;

            blockInfo.IsAvailable = ( blockInstance != null );

            if ( blockInstance != null )
            {
                blockInfo.SystemTypeName = blockInstance.GetType().FullName;
            }

            _lavaElements[name] = blockInfo;
        }

        public bool TryGetTagInstance( string tagName, out IRockLavaTag tagInstance )
        {
            tagInstance = null;

            if ( !_lavaElements.ContainsKey( tagName ) )
            {
                return false;
            }

            var tag = _lavaElements[tagName] as LavaTagInfo;

            if ( tag == null )
            {
                return false;
            }

            var factoryMethod = tag.FactoryMethod;

            tagInstance = factoryMethod( tagName );

            return true;
        }

        #endregion

        protected void ProcessException( Exception ex )
        {
            if ( this.ThrowExceptions )
            {
                throw ex;
            }
        }

        public bool ThrowExceptions { get; set; } = true;
    }
}