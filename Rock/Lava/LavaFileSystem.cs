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
using System.Web;

using Rock.Web.Cache;

namespace Rock.Lava
{
    /// <summary>
    /// The file system used to retrieve Lava templates referenced by an include tag.
    /// </summary>
    public class LavaFileSystem : LavaFileSystemBase
    {
        /// <summary>
        /// Resolves the absolute path for the input template.
        /// </summary>
        /// <param name="templatePath">The template path.</param>
        /// <returns></returns>
        protected override string OnResolveTemplatePath( string templatePath )
        {
            if ( templatePath == null )
            {
                throw new Exception( string.Format( "LavaFileSystem Illegal Template Name. (\"{0}\") ", templatePath ) );
            }

            /*
	            07/07/2020 - MSB 

                We need to be careful here because if this is being run from a job or workflow the HttpContext.Current will
                most likely be null. We shouldn't be relying on HttpContext.Current for information here unless we know it will not be null.
	
	            Reason: Update Persisted Datasets Job with Lava includes.
            */

            if ( HttpContext.Current != null )
            {
                if ( templatePath.StartsWith( "~~" ) &&
                    HttpContext.Current.Items != null &&
                    HttpContext.Current.Items.Contains( "Rock:PageId" ) )
                {
                    var rockPage = PageCache.Get( HttpContext.Current.Items["Rock:PageId"].ToString().AsInteger() );
                    if ( rockPage != null &&
                        rockPage.Layout != null &&
                        rockPage.Layout.Site != null )
                    {
                        templatePath = "~/Themes/" + rockPage.Layout.Site.Theme + ( templatePath.Length > 2 ? templatePath.Substring( 2 ) : string.Empty );
                    }
                }

                return HttpContext.Current.Server.MapPath( templatePath );
            }

            return Path.Combine( AppDomain.CurrentDomain.BaseDirectory, templatePath.Replace( "~~", "Themes/Rock" ).Replace( "~/", "" ) );
        }
    }
}