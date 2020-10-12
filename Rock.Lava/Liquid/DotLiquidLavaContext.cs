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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DotLiquid;
using DotLiquid.Exceptions;

using Rock.Common;

namespace Rock.Lava.DotLiquid
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
        public abstract string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects, string enabledLavaCommands, bool encodeStrings = false, bool throwExceptionOnErrors = false );
        public abstract string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects );
        public abstract void SetEnabledCommands( IEnumerable<string> commands );
        public void SetEnabledCommands( string commandList, string delimiter = "," )
        {
            var commands = commandList.SplitDelimitedValues( delimiter );

            SetEnabledCommands( commands );
        }

        public abstract void SetMergeFieldValue( string key, object value );
        public abstract void SetMergeFieldValue( string key, object value, string scopeSelector );
        public abstract void SetMergeFieldValues( LavaDictionary values );
        //public abstract void Stack( LavaDictionary newScope, Action callback );
        public abstract void Stack( Action callback );
    }

    public class DotLiquidLavaContext : LavaContextBase
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

        //public object this[string key]
        //{
        //    get
        //    {
        //        return _context[key];
        //    }
        //    set
        //    {
        //        _context[key] = value;
        //    }
        //}


        public override IList<LavaDictionary> GetEnvironments()
        {
            var environments = new List<LavaDictionary>();

            foreach ( var hash in _context.Environments )
            {
                environments.Add( new LavaDictionary( hash ) );
            }

            return environments;
        }

        public override IList<LavaDictionary> GetScopes()
        {
            var environments = new List<LavaDictionary>();

            foreach ( var hash in _context.Scopes )
            {
                environments.Add( new LavaDictionary( hash ) );
            }

            return environments;
        }

        public override ILavaEngine LavaEngine
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        //private List<string> _enabledCommands = new List<string>();

        public override void SetEnabledCommands( IEnumerable<string> commands )
        {
            if ( commands == null )
            {
                _context.Registers["EnabledCommands"] = string.Empty;
            }
            else
            {
                _context.Registers["EnabledCommands"] = commands.JoinStrings( "," );
            }
        }

        public override List<string> GetEnabledCommands()
        {
            // The set of enabled Lava Commands is stored in the DotLiquid Registers collection.
            if ( _context.Registers?.ContainsKey( "EnabledCommands" ) == true )
            {
                return _context.Registers["EnabledCommands"].ToString().Split( ',' ).ToList();
            }

            return new List<string>();
        }

        /// <summary>
        /// Gets the dictionary of values that are active in the local scope.
        /// Values are defined by the outermost container first, and overridden by values defined in a contained scope.
        /// </summary>
        /// <returns></returns>
        public override LavaDictionary GetMergeFieldsForLocalScope()
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

            // Second, apply overrides defined by the block or container in which the template is being resolved.
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

        /// <summary>
        /// Get the value of a field that is accessible for merging into a template.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public override object GetMergeFieldValue( string key, object defaultValue )
        {
            if ( !_context.HasKey(key) )
            {
                return defaultValue;
            }

            return _context[key];
        }

        /// <summary>
        /// Get a dictionary of field values that are accessible for merging in to a template.
        /// </summary>
        public override LavaDictionary GetMergeFieldValues()
        {
            return LavaDictionary.FromDictionary( _context.Registers );
        }

        public override string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects, string enabledLavaCommands, bool encodeStrings = false, bool throwExceptionOnErrors = false )
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

        public override string ResolveMergeFields( string content, IDictionary<string, object> mergeObjects )
        {
            var enabledCommands = string.Empty; // GlobalAttributesCache.Get().GetValue( "DefaultEnabledLavaCommands" );
            return ResolveMergeFields( content, mergeObjects, enabledCommands );
        }

        public override void SetMergeFieldValue( string key, object value )
        {
            var fieldValue = GetDotLiquidCompatibleValue( value );

            // Use the default implementation to set a variable in the current scope.
            _context[key] = fieldValue;
        }

        public override void SetMergeFieldValue( string key, object value, string scopeReference )
        {
            int? scopeIndex;

            if ( string.IsNullOrWhiteSpace( scopeReference ) )
            {
                scopeIndex = 0;
            }
            else
            { 
                scopeReference = scopeReference.Trim().ToLower();

                if ( scopeReference == "root" )
                {
                    scopeIndex = _context.Scopes.Count - 1;
                }
                else if ( scopeReference == "parent" )
                {
                    scopeIndex = 1;
                }
                else if ( scopeReference == "current" )
                {
                    scopeIndex = 0;
                }
                else
                {
                    scopeIndex = scopeReference.AsIntegerOrNull();
                }
            }

            if ( scopeIndex == null )
            {
                throw new Exception( $"SetMergeFieldValue failed. Scope reference \"{ scopeReference }\" is invalid." );
            }

            var fieldValue = GetDotLiquidCompatibleValue( value );

            // Set the variable in the specified scope.
            _context.Scopes[scopeIndex.Value][key] = fieldValue;
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
        //public override void Push( LavaDictionary newScope )
        //{
        //    if ( _context.Scopes.Count > 80 )
        //    {
        //        throw new StackLevelException( "ContextStackException" );
        //    }

        //    _context.Scopes.Insert( 0, newScope. Hash.FromDictionary( newScope ) );
        //}

        /// <summary>
        /// Pop from the stack. use <tt>Context#stack</tt> instead
        /// </summary>
        //public override LavaDictionary Pop()
        //{
        //    if ( _context.Scopes.Count == 1 )
        //    {
        //        throw new ContextException();
        //    }

        //    var result = LavaDictionary.FromDictionary( _context.Scopes[0] );

        //    _context.Scopes.RemoveAt( 0 );

        //    return result;
        //}

        //public override void Stack( LavaDictionary newScope, Action callback )
        //{
        //    Push( newScope );
        //    try
        //    {
        //        callback();
        //    }
        //    finally
        //    {
        //        Pop();
        //    }
        //}

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
        public override void Stack( Action callback )
        {
            //Stack( new LavaDictionary(), callback );

            // Push a new scope onto the stack.
            if ( _context.Scopes.Count > 80 )
            {
                throw new StackLevelException( "ContextStackException" );
            }

            var newScope = new Hash();

            _context.Scopes.Insert( 0, newScope );

            try
            {
                callback();
            }
            finally
            {
                if ( _context.Scopes.Count == 1 )
                {
                    throw new ContextException();
                }

                //var result = LavaDictionary.FromDictionary( _context.Scopes[0] );

                _context.Scopes.RemoveAt( 0 );

                //return result;


                //                Pop();
            }

        }

        public void ClearValues()
        {
            _context.ClearInstanceAssigns();
        }

        public override void SetMergeFieldValues( LavaDictionary values )
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

        private object GetDotLiquidCompatibleValue( object value )
        {
            if ( value == null
                 || value is string
                 || value is IEnumerable
                 || value is decimal
                 || value is DateTime
                 || value is DateTimeOffset
                 || value is TimeSpan
                 || value is Guid
                 || value is Enum
                 || value is KeyValuePair<string, object>
                 || value.GetType().IsPrimitive
                 )
            {
                return value;
            }

            var safeTypeTransformer = Template.GetSafeTypeTransformer( value.GetType() );

            if ( safeTypeTransformer != null )
            {
                return safeTypeTransformer( value );
            }

            return value;
        }


        #region Unused???

        //TODO: Remove this?
        public override IDictionary<string, object> GetMergeFieldsInContainerScope()
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

        // TODO: Remove this?
        public override IDictionary<string, object> GetMergeFieldsInScope()
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

        #endregion


    }
}