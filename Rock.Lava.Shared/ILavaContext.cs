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
        /// <summary>
        /// The set of Lava Commands that are specifically enabled in this context.
        /// </summary>
        IList<string> EnabledCommands { get; }

        /// <summary>
        /// Registers are user-defined variables in the current context that are internally available to custom filters and tags.
        /// </summary>
        /// <remarks>
        /// Usages: Stores the EnabledCommands setting.
        /// </remarks>
        LavaDictionary Registers { get; }

        /// <summary>
        /// ???
        /// </summary>
        IList<LavaDictionary> Environments { get; }

        /// <summary>
        /// ???
        /// </summary>
        IList<LavaDictionary> Scopes { get; }

        //IDictionary<string, object> GetInternalMergeFields();

        /// <summary>
        /// Gets the set of merge fields in the current Lava source markup.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetMergeFieldsInScope();

        /// <summary>
        /// Gets the dictionary of values that are active in the local scope.
        /// Values are defined by the outermost container first, and overridden by values defined in a contained scope.
        /// </summary>
        /// <returns></returns>
        LavaDictionary GetMergeFieldsForLocalScope();

        /// <summary>
        /// Gets the set of merge fields in the current Lava block or container hierarchy.
        /// </summary>
        /// <returns></returns>
        IDictionary<string, object> GetMergeFieldsInContainerScope();

        string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects, string enabledLavaCommands, bool encodeStrings = false, bool throwExceptionOnErrors = false );
        string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects );

        object this[string key] { get; set; }

        object GetValue( string key, object defaultValue );
        void SetValue( string key, object value );

        ILavaEngine LavaEngine { get; }


        /// <summary>
        /// pushes a new local scope on the stack, pops it at the end of the block
        /// 
        /// Example:
        /// 
        /// context.stack do
        /// context['var'] = 'hi'
        /// end
        /// context['var] #=> nil
        /// </summary>
        /// <param name="newScope"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        void Stack( LavaDictionary newScope, Action callback );

        void Stack( Action callback );

        /// <summary>
        /// Push new local scope on the stack. use <tt>Context#stack</tt> instead
        /// </summary>
        /// <param name="newScope"></param>
        void Push( LavaDictionary newScope );

        /// <summary>
        /// Pop from the stack. use <tt>Context#stack</tt> instead
        /// </summary>
        LavaDictionary Pop();

    }
}
