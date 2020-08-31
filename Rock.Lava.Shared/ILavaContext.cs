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

}
