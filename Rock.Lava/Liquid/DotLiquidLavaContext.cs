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
using System.Collections.ObjectModel;
using System.Linq;
using DotLiquid;
using DotLiquid.Exceptions;

using Rock.Common;

namespace Rock.Lava.DotLiquid
{
    public class DotLiquidLavaContext : ILavaContext
    {
        private Context _context;

        public DotLiquidLavaContext( Context context )
        {
            _context = context;
        }

        public Context DotLiquidContext
        {
            get
            {
                return _context;
            }
        }

        public object this[string key]
        {
            get
            {
                return _context[key];
            }
            set
            {
                _context[key] = value;
            }
        }

        public IList<LavaDictionary> Environments
        {
            get
            {
                var environments = new List<LavaDictionary>();

                foreach ( var hash in _context.Environments )
                {
                    environments.Add( new LavaDictionary( hash ) );
                }

                return environments;
            }
        }

        public IList<LavaDictionary> Scopes
        {
            get
            {
                var environments = new List<LavaDictionary>();

                foreach ( var hash in _context.Scopes )
                {
                    environments.Add( new LavaDictionary( hash ) );
                }

                return environments;
            }

        }

        public ILavaEngine LavaEngine
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        private List<string> _enabledCommands = new List<string>();

        public IList<string> EnabledCommands
        {
            get
            {
                return _enabledCommands;
                //_enabledCommands
                //if ( _context.Registers?.ContainsKey( "EnabledCommands" ) == true )
                //{
                //    return _context.Registers["EnabledCommands"].ToString().Split( ',' ).ToList();
                //}

                //return new List<string>();
            }
            set
            {
                if ( value == null )
                {
                    _enabledCommands = new List<string>();
                }
                else
                {
                    _enabledCommands = value.ToList();
                }
            }
        }

        public LavaDictionary Registers
        {
            get
            {
                return LavaDictionary.FromDictionary( _context.Registers );
            }
        }


        public IDictionary<string, object> GetMergeFieldsInContainerScope()
        {
            // get merge fields loaded by the block or container
            var internalMergeFields = new Dictionary<string, object>();

            if ( _context.Environments.Count > 0 )
            {
                foreach ( var item in _context.Environments[0] )
                {
                    internalMergeFields.AddOrReplace( item.Key, item.Value );
                }
            }

            return internalMergeFields;
        }

        public IDictionary<string, object> GetMergeFieldsInScope()
        {
            var internalMergeFields = new Dictionary<string, object>();

            // get variables defined in the lava source
            foreach ( var scope in _context.Scopes )
            {
                foreach ( var item in scope )
                {
                    internalMergeFields.AddOrReplace( item.Key, item.Value );
                }
            }

            return internalMergeFields;
        }

        /// <summary>
        /// Gets the dictionary of values that are active in the local scope.
        /// Values are defined by the outermost container first, and overridden by values defined in a contained scope.
        /// </summary>
        /// <returns></returns>
        public LavaDictionary GetMergeFieldsForLocalScope()
        {
            var fields = new LavaDictionary();

            // First, get all of the variables defined in the local lava context
            foreach ( var scope in _context.Scopes )
            {
                foreach ( var item in scope )
                {
                    fields.AddOrReplace( item.Key, item.Value );
                }
            }

            // Second, apply overrides defined by the block or container
            foreach ( var environment in _context.Environments )
            {
                foreach ( var item in environment )
                {
                    fields.AddOrReplace( item.Key, item.Value );
                }
            }

            // TODO: Verify that this order is correct? It is the same order that is used in numerous places throughout Rock, but it seems to be inverted?
            // Shouldn't the local scope override the container scope?

            return fields;
        }

        public object GetValue( string key, object defaultValue )
        {
            if ( !_context.HasKey(key) )
            {
                return defaultValue;
            }

            return _context[key];
        }

        public string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects, string enabledLavaCommands, bool encodeStrings = false, bool throwExceptionOnErrors = false )
        {
            try
            {
                // 7-9-2020 JME / NA
                // We decided to remove the check for lava merge fields here as this method is specifically
                // made to resolve them. The performance increase for text without lava is acceptable as in
                // a vast majority of cases the string will have lava (that's what this method is for). In
                // these cases there is a performance tax (though small) on the vast majority of calls.

                // If there have not been any EnabledLavaCommands explicitly set, then use the global defaults.
                if ( enabledLavaCommands == null )
                {
                    // TODO:    
                    //enabledLavaCommands = GlobalAttributesCache.Value( "DefaultEnabledLavaCommands" );
                }

                Template template = GetTemplate( content );
                template.Registers.AddOrReplace( "EnabledCommands", enabledLavaCommands );
                //template.InstanceAssigns.AddOrReplace( "CurrentPerson", currentPersonOverride );
                return template.Render( Hash.FromDictionary( mergeObjects ) );
            }
            catch ( Exception ex )
            {
                throw ex;
                //ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
                //return "Error resolving Lava merge fields: " + ex.Message;
            }
        }

        public string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects )
        {
            var enabledCommands = string.Empty; // GlobalAttributesCache.Get().GetValue( "DefaultEnabledLavaCommands" );
            return ResolveMergeFields( content, mergeObjects, enabledCommands );
        }

        public void SetValue( string key, object value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Uses Lava to resolve any merge codes within the content using the values in the merge objects.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="mergeObjects">The merge objects.</param>
        /// <param name="enabledLavaCommands">The enabled lava commands.</param>
        /// <param name="encodeStrings">if set to <c>true</c> [encode strings].</param>
        /// <param name="throwExceptionOnErrors">if set to <c>true</c> [throw exception on errors].</param>
        /// <returns></returns>
        //public string ResolveMergeFields( this string content, IDictionary<string, object> mergeObjects, string enabledLavaCommands, bool encodeStrings = false, bool throwExceptionOnErrors = false )
        //{
        //    try
        //    {
        //        if ( !content.HasMergeFields() )
        //        {
        //            return content ?? string.Empty;
        //        }

        //        if ( GlobalAttributesCache.Get().LavaSupportLevel == Lava.LavaSupportLevel.LegacyWithWarning && mergeObjects.ContainsKey( "GlobalAttribute" ) )
        //        {
        //            if ( hasLegacyGlobalAttributeLavaMergeFields.IsMatch( content ) )
        //            {
        //                Rock.Model.ExceptionLogService.LogException( new Rock.Lava.LegacyLavaSyntaxDetectedException( "GlobalAttribute", "" ), System.Web.HttpContext.Current );
        //            }
        //        }

        //        Template template = GetTemplate( content );
        //        template.Registers.AddOrReplace( "EnabledCommands", enabledLavaCommands );

        //        string result;

        //        if ( encodeStrings )
        //        {
        //            // if encodeStrings = true, we want any string values to be XML Encoded ( 
        //            RenderParameters renderParameters = new RenderParameters();
        //            renderParameters.LocalVariables = Hash.FromDictionary( mergeObjects );
        //            renderParameters.ValueTypeTransformers = new Dictionary<Type, Func<object, object>>();
        //            renderParameters.ValueTypeTransformers[typeof( string )] = EncodeStringTransformer;
        //            result = template.Render( renderParameters );
        //        }
        //        else
        //        {
        //            result = template.Render( Hash.FromDictionary( mergeObjects ) );
        //        }

        //        if ( throwExceptionOnErrors && template.Errors.Any() )
        //        {
        //            if ( template.Errors.Count == 1 )
        //            {
        //                throw template.Errors[0];
        //            }
        //            else
        //            {
        //                throw new AggregateException( template.Errors );
        //            }
        //        }

        //        return result;
        //    }
        //    catch ( System.Threading.ThreadAbortException )
        //    {
        //        // Do nothing...it's just a Lava PageRedirect that just happened.
        //        return string.Empty;
        //    }
        //    catch ( Exception ex )
        //    {
        //        if ( throwExceptionOnErrors )
        //        {
        //            throw;
        //        }
        //        else
        //        {
        //            //ExceptionLogService.LogException( ex, System.Web.HttpContext.Current );
        //            return "Error resolving Lava merge fields: " + ex.Message;
        //        }
        //    }
        //}

        /// <summary>
        /// Looks for a parsed template in cache (if the content is 100 characters or less).
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns></returns>
        private Template GetTemplate( string content )
        {
            // Do not cache any content over 100 characters in length
            //if ( content?.Length > 100 )
            //{
            return Template.Parse( content );
            //}

            // Get template from cache
            //var template = LavaTemplateCache.Get( content ).Template;

            // Clear any previous errors
            //template.Errors.Clear();

            //return template;
        }

        /// <summary>
        /// Push new local scope on the stack. use <tt>Context#stack</tt> instead
        /// </summary>
        /// <param name="newScope"></param>
        public void Push( LavaDictionary newScope )
        {
            if ( Scopes.Count > 80 )
            {
                throw new StackLevelException( "ContextStackException" );
            }

            Scopes.Insert( 0, newScope );
        }

        /// <summary>
        /// Pop from the stack. use <tt>Context#stack</tt> instead
        /// </summary>
        public LavaDictionary Pop()
        {
            if ( Scopes.Count == 1 )
            {
                throw new ContextException();
            }

            var result = Scopes[0];

            Scopes.RemoveAt( 0 );

            return result;
        }

        /// <summary>
        /// pushes a new local scope on the stack, pops it at the end of the block
        /// 
        /// Example:
        /// 
        /// context.stack do
        /// context['var'] = 'hi'
        /// end
        /// context['var] #=> nil
        /// </summary>
        /// <param name="newScope"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public void Stack( LavaDictionary newScope, Action callback )
        {
            Push( newScope );
            try
            {
                callback();
            }
            finally
            {
                Pop();
            }
        }

        public void Stack( Action callback )
        {
            Stack( new LavaDictionary(), callback );

        }
    }
}