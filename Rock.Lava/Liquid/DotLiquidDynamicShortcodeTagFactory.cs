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

namespace Rock.Lava.DotLiquid
{
    public partial class DotLiquidEngine
    {
        //private static DotLiquidDynamicShortcodeTagFactory _dynamicShortcodeTag = new DotLiquidDynamicShortcodeTagFactory();

        /// <summary>
        /// Represents an implementation of a DotLiquid Tag that can be configured dynamically at runtime.
        /// </summary>
        /// <remarks>
        /// This class wraps a Lava Shortcode in a Tag Type that can be registered with the DotLiquid framework.
        /// </remarks>
        private class DotLiquidDynamicShortcodeTagFactory : Tag
        {
            public static Dictionary<string, Func<string, DynamicShortcodeDefinition>> TagFactoryMethods
            {
                get
                {
                    return _tagFactoryMethods;
                }
            }

            private static Dictionary<string, Func<string, DynamicShortcodeDefinition>> _tagFactoryMethods = new Dictionary<string, Func<string, DynamicShortcodeDefinition>>();

            public override void Initialize( string tagName, string markup, List<string> tokens )
            {
                var factoryMethod = _tagFactoryMethods[tagName];

                var shortcode = factoryMethod( tagName );

                base.Initialize( tagName, shortcode.TemplateMarkup, shortcode.Tokens );
            }

            public override void Render( Context context, TextWriter result )
            {
                base.Render( context, result );
            }
        }

    }
}
