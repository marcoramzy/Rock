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
using System.IO;

using DotLiquid;
using DotLiquid.Exceptions;
using DotLiquid.FileSystems;

namespace Rock.Lava
{
    /// <summary>
    /// Lava's <seealso cref="IFileSystem"/>. This is used when Lava templates retrieve other Lava templates when using the include tag.
    /// </summary>
    public class DotLiquidLavaFileSystem : LavaFileSystem, IFileSystem
    {
        /// <summary>
        /// Called by Liquid to retrieve a template file
        /// </summary>
        /// <param name="context"></param>
        /// <param name="templateName"></param>
        /// <returns></returns>
        /// <exception cref="FileSystemException">LavaFileSystem Template Not Found</exception>
        string IFileSystem.ReadTemplateFile( Context context, string templateName )
        {
            string templatePath = ( string ) context[templateName];

            // Try to find exact file specified
            var file = new FileInfo( FullPath( templatePath ) );
            if ( file.Exists )
            {
                return File.ReadAllText( file.FullName );
            }

            // If requested template file does not include an extension
            if ( string.IsNullOrWhiteSpace( file.Extension ) )
            {
                // Try to find file with .lava extension
                string filePath = file.FullName + ".lava";
                if ( File.Exists( filePath ) )
                {
                    return File.ReadAllText( filePath );
                }

                // Try to find file with .liquid extension
                filePath = file.FullName + ".liquid";
                if ( File.Exists( filePath ) )
                {
                    return File.ReadAllText( filePath );
                }

                // If file still not found, try prefixing filename with an underscore
                if ( !file.Name.StartsWith( "_" ) )
                {
                    filePath = Path.Combine( file.DirectoryName, string.Format( "_{0}.lava", file.Name ) );
                    if ( File.Exists( filePath ) )
                    {
                        return File.ReadAllText( filePath );
                    }
                    filePath = Path.Combine( file.DirectoryName, string.Format( "_{0}.liquid", file.Name ) );
                    if ( File.Exists( filePath ) )
                    {
                        return File.ReadAllText( filePath );
                    }
                }
            }

            throw new FileSystemException( "LavaFileSystem Template Not Found", templatePath );
        }
    }
}