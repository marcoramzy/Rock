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

namespace Rock.Lava
{
    public interface ILavaTagInfo
    {
        string Name { get; }
        string SystemTypeName { get; }
        string ToString();
    }

    public class LavaTagInfo : ILavaTagInfo
    {
        public string Name { get; set; }

        public string SystemTypeName { get; set; }

        public override string ToString()
        {
            return string.Format( "{0} [{1}]", Name, SystemTypeName );
        }
    }

    public enum LavaEngineTypeSpecifier
    {
        // DotLiquid is an open-source implementation of the Liquid templating language. [https://github.com/dotliquid/dotliquid]
        DotLiquid = 1,
        // Fluid is an open-source implementation of the Liquid templating language. [https://github.com/sebastienros/fluid]
        Fluid = 2
    }

    /// <summary>
    /// Represents a Lava Template.
    /// </summary>
    public interface ILavaEngine
    {
        /// <summary>
        /// The descriptive name of the templating framework on which Lava is currently operating.
        /// </summary>
        string EngineName { get; }

        /// <summary>
        /// The Liquid template framework used to parse and render Lava templates.
        /// </summary>
        LavaEngineTypeSpecifier EngineType { get; }

        /// <summary>
        /// Get a new context instance.
        /// </summary>
        /// <returns></returns>
        ILavaContext NewContext();

        void RegisterTag( string name, Func<string, IRockLavaTag> factoryMethod );
        void RegisterBlock( string name, Func<string, IRockLavaBlock> factoryMethod );
        //void RegisterShortcode( IRockShortcode shortcode );

        /// <summary>
        /// Registers a shortcode with a factory method that provides the definition of the shortcode dynamically.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="factoryMethod"></param>
        void RegisterDynamicShortcode( string name, Func<string, DynamicShortcodeDefinition> factoryMethod );

        void RegisterStaticShortcode( string name, Func<string, IRockShortcode> factoryMethod );

        //bool TagIsRegistered( string name );

        Dictionary<string, ILavaTagInfo> GetRegisteredTags();

        //void RegisterShortcode<T>( string name )
        //    where T : IRockShortcode;
        //where T : Tag, new();
        //{
        //    Shortcodes[name] = typeof( T );
        //}

        void UnregisterShortcode( string name );
        //{
        //    if ( Shortcodes.ContainsKey( name ) )
        //    {
        //        Shortcodes.Remove( name );
        //    }
        //}

        Type GetShortcodeType( string name );
        //{
        //    Type result;
        //    Shortcodes.TryGetValue( name, out result );
        //    return result;
        //}

        /// <summary>
        /// Set a value that will be used when rendering this template.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetContextValue( string key, object value );


        /// <summary>
        /// Try to render the provided template
        /// </summary>
        /// <param name="inputTemplate"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool TryRender( string inputTemplate, out string output );

        bool TryRender( string inputTemplate, out string output, LavaDictionary mergeValues );

        bool TryRender( string inputTemplate, out string output, ILavaContext context );
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allowedMembers"></param>
        void RegisterSafeType( Type type, string[] allowedMembers = null );

        bool TryParseTemplate( string inputTemplate, out ILavaTemplate template );

        ILavaTemplate ParseTemplate( string inputTemplate );

        bool AreEqualValue( object left, object right );

        void RenderTag( IRockLavaBlock tag, ILavaContext context, TextWriter result );
    }

}
