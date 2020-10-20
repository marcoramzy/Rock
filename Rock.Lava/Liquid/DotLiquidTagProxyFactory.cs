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
    /// A factory to generate Lava Tags.
    /// </summary>
    internal static class DotLiquidTagProxyFactory
    {
        private static Dictionary<string, Func<string, IRockLavaTag>> _tagFactoryMethods = new Dictionary<string, Func<string, IRockLavaTag>>();

        public static void RegisterTag( string name, Func<string, IRockLavaTag> factoryMethod )
        {
            _tagFactoryMethods.Add( name, factoryMethod );
        }

        public static bool TryGetTagInstance( string tagName, out IRockLavaTag tagInstance )
        {
            if ( !_tagFactoryMethods.ContainsKey(tagName) )
            {
                tagInstance = null;
                return false;
            }

            var factoryMethod = _tagFactoryMethods[tagName];

            tagInstance = factoryMethod( tagName );

            return true;
        }

    }
}
