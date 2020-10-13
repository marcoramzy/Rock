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
using DotLiquid;
using Rock.Lava.Blocks;

namespace Rock.Lava.DotLiquid
{
    /// <summary>
    /// Represents an implementation of a Lava Data Object that can be used by the DotLiquid Templating Framework.
    /// </summary>
    internal class DotLiquidLavaDataObjectProxy : ILiquidizable, IIndexable, ILavaDataObject //, IDictionary<string, object>
    {
        private ILavaDataObject _dataObject = null;

        public DotLiquidLavaDataObjectProxy( ILavaDataObject dataObject )
        {
            _dataObject = dataObject;
        }

        public object this[object key]
        {
            get
            {
                if ( _dataObject == null )
                {
                    return null;
                }

                return _dataObject.GetValue( key );
            }
        }

        public List<string> AvailableKeys
        {
            get
            {
                if ( _dataObject == null )
                {
                    return new List<string>();
                }

                return _dataObject.AvailableKeys;
            }
        }

        public bool ContainsKey( object key )
        {
            if ( _dataObject == null )
            {
                return false;
            }

            return _dataObject.ContainsKey( key );
        }

        public object GetValue( object key )
        {
            if ( _dataObject == null )
            {
                return null;
            }

            return _dataObject.GetValue( key );
        }

        public object ToLiquid()
        {
            return this;
        }

        /// <summary>
        /// Return a string representation of the object.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // If we are wrapping an object instance, return the ToString() for the object.
            if ( _dataObject != null )
            {
                return _dataObject.ToString();
            }
            else
            {
                return base.ToString();
            }
        }
    }
}
