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

namespace Rock.Lava
{
    /// <summary>
    /// Represents a specific environment and configuration in which a Lava template is resolved at runtime by the Lava Engine.
    /// </summary>
    public interface ILavaContext
    {
        //IList<IDictionary<string, object>> Environments { get; }
        IList<IDictionary<string, object>> Scopes { get; }

        //IDictionary<string, object> GetInternalMergeFields();

        /// <summary>
        /// Gets the set of merge fields in the current Lava source markup.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetMergeFieldsInScope();

        /// <summary>
        /// Gets the set of merge fields in the current Lava block or container hierarchy.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetMergeFieldsInEnvironment();

        string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects, string enabledLavaCommands, bool encodeStrings = false, bool throwExceptionOnErrors = false );
        string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects );

        object GetValue( string key );
        void SetValue( string key, object value );

        ILavaEngine LavaEngine { get; }
    }

    /// <summary>
    /// Represents a Lava Template.
    /// </summary>
    public interface ILavaEngine
    {
        /// <summary>
        /// The descriptive name of the templating framework on which Lava is currently operating.
        /// </summary>
        string FrameworkName { get; }

        /// <summary>
        /// Registers a shortcode with a factory method.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="shortcodeFactoryMethod"></param>
        void RegisterShortcode( string name, Func<string, IRockShortcode> shortcodeFactoryMethod );

        void RegisterShortcode( IRockShortcode shortcode );

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allowedMembers"></param>
        void RegisterSafeType( Type type, string[] allowedMembers = null );
    }

}
