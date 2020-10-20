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
    /// Defines the properties of a Dynamic Shortcode.
    /// A dynamic shortcode creates an element in a Lava document from a parameterized template that is defined at runtime.
    /// </summary>
    public class DynamicShortcodeDefinition
    {
        public string Name { get; set; }

        public string TemplateMarkup { get; set; }

        public List<string> Tokens { get; set; }

        public LavaShortcodeTypeSpecifier ElementType { get; set; }
    }
}