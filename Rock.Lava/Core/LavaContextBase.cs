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

using Rock.Common;

namespace Rock.Lava
{
    public abstract class LavaContextBase : ILavaContext
    {
        public object this[string key]
        {
            get
            {
                return GetMergeFieldValue( key, null );
            }
            set
            {
                SetMergeFieldValue( key, value );
            }
        }

        public abstract ILavaEngine LavaEngine { get; }

        public abstract List<string> GetEnabledCommands();
        public abstract IList<LavaDictionary> GetEnvironments();
        public abstract LavaDictionary GetMergeFieldsForLocalScope();
        public abstract IDictionary<string, object> GetMergeFieldsInContainerScope();
        public abstract IDictionary<string, object> GetMergeFieldsInScope();
        public abstract object GetMergeFieldValue( string key, object defaultValue );
        public abstract LavaDictionary GetMergeFieldValues();
        public abstract IList<LavaDictionary> GetScopes();
        //public abstract LavaDictionary Pop();
        //public abstract void Push( LavaDictionary newScope );
        public abstract string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects, string enabledLavaCommands = null, bool encodeStrings = false, bool throwExceptionOnErrors = false );
        public abstract void SetEnabledCommands( IEnumerable<string> commands );
        public void SetEnabledCommands( string commandList, string delimiter = "," )
        {
            var commands = commandList.SplitDelimitedValues( delimiter );

            SetEnabledCommands( commands );
        }

        public void SetMergeFieldValue( string key, object value )
        {
            SetMergeFieldValue( key, value, null );
        }

        public abstract void SetMergeFieldValue( string key, object value, string scopeSelector );

        public virtual void SetMergeFieldValues( LavaDictionary values )
        {
            if ( values == null )
            {
                return;
            }

            foreach ( var kvp in values )
            {
                SetMergeFieldValue( kvp.Key, kvp.Value );
            }
        }

        //public abstract void Stack( LavaDictionary newScope, Action callback );
        public abstract void Stack( Action callback );
    }
}