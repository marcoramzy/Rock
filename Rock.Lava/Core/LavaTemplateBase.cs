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

namespace Rock.Lava.DotLiquid
{
    /// <summary>
    /// Provides base functionality for a Lava template.
    /// </summary>
    public abstract class LavaTemplateBase : ILavaTemplate
    {
        public abstract ILavaEngine LavaEngine { get; }

        /// <summary>
        /// The set of Lava commands permitted for this template.
        /// </summary>
        public IList<string> EnabledCommands { get; set; }

        public bool TryRender( LavaRenderParameters parameters, out string output, out IList<Exception> errors )
        {
            return TryRender( parameters as IDictionary<string, object>, out output, out errors );
        }

        public string Render( IDictionary<string, object> values )
        {
            string output;
            IList<Exception> errors;

            var isValid = TryRender( values, out output, out errors );

            if ( !isValid )
            {
                // Append error messages to the output.
                foreach ( var error in errors )
                {
                    output += "\nLiquid Error: " + error.Message;
                }
            }

            return output;           
        }

        public abstract void SetContextValue( string key, object value );
        public bool TryRender( IDictionary<string, object> values, out string output, out IList<Exception> errors )
        {
            if ( values == null )
            {
                values = new Dictionary<string, object>();
            }

            // Add the EnabledCommands setting to the context.
            values["EnabledCommands"] = this.EnabledCommands;

            return OnTryRender( values, out output, out errors );
        }

        protected abstract bool OnTryRender( IDictionary<string, object> values, out string output, out IList<Exception> errors );
    }
}
