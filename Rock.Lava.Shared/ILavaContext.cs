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
        /// The set of Lava Commands that are specifically enabled for this context.
        /// </summary>
        List<string> GetEnabledCommands();

        void SetEnabledCommands( IEnumerable<string> commands );

        void SetEnabledCommands( string commandList, string delimiter = "," );

        /// <summary>
        /// Gets the value of a field that is accessible for merging into a template.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        object GetMergeFieldValue( string key, object defaultValue );

        /// <summary>
        /// Gets the collection of user-defined variables in the current context that are internally available to custom filters and tags.
        /// </summary>
        LavaDictionary GetMergeFieldValues();

        /// <summary>
        /// Sets the value of a field that is accessible for merging into a template.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        void SetMergeFieldValue( string key, object value );

        /// <summary>
        /// Sets the value of a field that is accessible for merging into a template.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <param name="scopeSelector>"root|parent|current", or the index number of a scope in the current stack.</param>
        /// <returns></returns>
        void SetMergeFieldValue( string key, object value, string scopeSelector );

        /// <summary>
        /// Sets a collection of user-defined variables in the current context that are internally available to custom filters and tags.
        /// </summary>
        /// <param name="values"></param>
        void SetMergeFieldValues( LavaDictionary values );

        /// <summary>
        /// Get or set the value of a field that is accessible for merging into a template.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object this[string key] { get; set; }

        ILavaEngine LavaEngine { get; }

        /// <summary>
        /// Retrieves a nested stack of Environments, with the current environment first.
        /// An environment holds the variables that have been defined by the container in which a Lava template is resolved.
        /// </summary>
        IList<LavaDictionary> GetEnvironments();

        /// <summary>
        /// Retrieves a nested stack of Variables, with the current context first.
        /// A scope holds the variables that have been created and assigned in the process of resolving a Lava template.
        /// </summary>
        IList<LavaDictionary> GetScopes();

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
        //void Stack( LavaDictionary newScope, Action callback );

        void Stack( Action callback );

        /// <summary>
        /// Push new local scope on the stack. use <tt>Context#stack</tt> instead
        /// </summary>
        /// <param name="newScope"></param>
        //void Push( LavaDictionary newScope );

        /// <summary>
        /// Pop from the stack. use <tt>Context#stack</tt> instead
        /// </summary>
        //LavaDictionary Pop();

    }
}
