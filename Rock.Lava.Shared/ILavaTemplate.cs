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
    /// Represents a Lava Template.
    /// </summary>
    public interface ILavaTemplate
    {
        /// <summary>
        /// Set a value that will be used when rendering this template.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void SetContextValue( string key, object value );

        /// <summary>
        /// Try to render the template.
        /// Errors will be included in the rendered output.
        /// </summary>
        /// <returns></returns>        
        string Render();

        /// <summary>
        /// Try to render the template using the provided context values.
        /// Errors will be included in the rendered output.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>        
        string Render( IDictionary<string, object> values );

        /// <summary>
        /// Try to render the template using the provided context values.
        /// </summary>
        /// <param name="inputTemplate"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        bool TryRender( IDictionary<string, object> values, out string output, out IList<Exception> errors );

        bool TryRender( LavaRenderParameters parameters, out string output, out IList<Exception> errors );

        /// <summary>
        /// The set of Lava commands permitted for this template.
        /// </summary>
        IList<string> EnabledCommands { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="allowedMembers"></param>
        //void RegisterSafeType( Type type, string[] allowedMembers = null );

        ILavaEngine LavaEngine { get;  }
        //ILavaContext Context { get; }
    }

    public class LavaRenderParameters
    {
        public LavaRenderParameters()
        {
            EnabledCommands = new List<string>();
            Registers = new Dictionary<string, object>();
            InstanceAssigns = new Dictionary<string, object>();
            LocalVariables = new Dictionary<string, object>();
            ValueTypeTransformers = new Dictionary<Type, Func<object, object>>();
        }

        /// <summary>
        /// The set of Lava commands permitted for this rendering of the template.
        /// </summary>
        public List<string> EnabledCommands { get; set; }

        /// <summary>
        /// Private variable assignments that are shared with other instances of this template but are not accessible to the source template.
        /// </summary>
        public IDictionary<string, object> Registers { get; set; }
       
        /// <summary>
        /// Local variable assignments made while resolving this template.
        /// </summary>
        [Obsolete("Not sure if this is used?")]
        public IDictionary<string, object> InstanceAssigns { get; set; }

        /// <summary>
        /// Local variable assignments used to resolve this template.
        /// </summary>
        public IDictionary<string, object> LocalVariables { get; set; }

        /// <summary>
        /// A set of functions that transform the values supplied to the template for specific Types.
        /// </summary>
        public IDictionary<Type, Func<object, object>> ValueTypeTransformers { get; set; }
    }
}
