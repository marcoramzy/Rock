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
using DotLiquid;
using DotLiquid.NamingConventions;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Rock.Lava.DotLiquid
{
    /// <summary>
    /// A Lava wrapper for the DotLiquid implementation of a Liquid Template.
    /// </summary>
    public class DotLiquidLavaTemplate : ILavaTemplate
    {
        #region Constructors

        private Template _templateContext;

        public DotLiquidLavaTemplate(Template template)
        {
            _templateContext = template;
        }
                        
        public DotLiquidLavaTemplate()
        {
            // Initialize the engine and create a new context.
            //FluidEngine.Initialize();

            //_templateContext = new LiquidTemplateContext();

            // Bypass standard Liquid casing and preserve PascalCase for method names. 
            //_templateContext.MemberRenamer = ( member => member.Name );

            // Set the template culture to the current culture.
            //_templateContext.PushCulture( CultureInfo.CurrentCulture );

            //ReplaceBuiltInFunctions();

            //AddFiltersFromClass( typeof( TemplateFilters ) );
            //AddFiltersFromClass( typeof( ScribanFilters ) );

            InitializeDotLiquid();
        }

        /// <summary>
        /// Initializes Rock's Lava system (which uses DotLiquid)
        /// Doing this in startup will force the static Liquid class to get instantiated
        /// so that the standard filters are loaded prior to the custom RockFilter.
        /// This is to allow the custom 'Date' filter to replace the standard Date filter.
        /// </summary>
        private void InitializeDotLiquid()
        {
            // DotLiquid uses a RubyDateFormat by default,
            // but since we aren't using Ruby, we want to disable that
            Liquid.UseRubyDateFormat = false;

            /* 2020-05-20 MDP (actually this comment was here a long time ago)
                NOTE: This means that all the built in template filters,
                and the RockFilters, will use CSharpNamingConvention.
            
                For example the dotliquid documentation says to do this for formatting dates: 
                {{ some_date_value | date:"MMM dd, yyyy" }}
           
                However, if CSharpNamingConvention is enabled, it needs to be: 
                {{ some_date_value | Date:"MMM dd, yyyy" }}
            */

            Template.NamingConvention = new CSharpNamingConvention();

            //Template.FileSystem = new LavaFileSystem();
            Template.RegisterSafeType( typeof( Enum ), o => o.ToString() );
            Template.RegisterSafeType( typeof( DBNull ), o => null );

            Template.RegisterFilter( typeof( Rock.Lava.Filters.TemplateFilters ) );
            Template.RegisterFilter( typeof( Rock.Lava.DotLiquid.DotLiquidFilters ) );
        }

        private void ReplaceBuiltInFunctions()
        {
            //var function = _templateContext.BuiltinObject["join"];

            //_templateContext.BuiltinObject.Remove( "join" );
            //_templateContext.BuiltinObject.Add( "Join", function );            
        }

        #endregion

        private Hash _Context = new Hash();

        public ILavaEngine LavaEngine
        {
            get
            {
                return new DotLiquidEngineManager();
            }
        }

        public void SetContextValue( string key, object value )
        {
            // DotLiquid does not process ExpandoObjects, so replace with Dictionary<string, object>
            //value = ConvertExpandoObjectsToDictionaries( value );

            //var converter = new ExpandoObjectConverter();

            //var serialized = JsonConvert.SerializeObject( value ); // .Serialize( Object<List<ExpandoObject>>( json, converter );

            //var newValue = JsonConvert.DeserializeObject( serialized, new JsonSerializerSettings { Converters = new List<JsonConverter> { new ExpandoConverter() } } );

            //object input = null;
            //input = JsonConvert.DeserializeObject<List<ExpandoObject>>( json, converter );
            //var output = ( List<object> ) StandardFilters.Sort( input, "StartDateTime", "desc" );

            var newValue = ConvertExpandoObjectsToDictionaries( value );

            _Context.AddOrReplace( key, newValue );
        }

        public bool TryRender( string inputTemplate, out string output )
        {
            var template = Template.Parse( inputTemplate );

            output = template.Render( _Context );

            return true;
        }

        /// <summary>
        /// This method is specifically used to convert an <see cref="ExpandoObject"/> with a Tree structure to a <see cref="Dictionary{string, object}"/>.
        /// </summary>
        /// <param name="expando">The <see cref="ExpandoObject"/> to convert</param>
        /// <returns>The fully converted <see cref="ExpandoObject"/></returns>
        private object ConvertExpandoObjectsToDictionaries( object input )
        {
            // Replace ExpandoObject collection.
            var expandoCollection = input as IEnumerable<ExpandoObject>;

            if ( expandoCollection != null )
            {
                // Replace with a list of Dictionary<string, object>
                var newList = new List<Dictionary<string, object>>();
                
                foreach (var expandoElement in expandoCollection)
                {
                    var newDictionary = ConvertExpandoObjectsToDictionaries( expandoElement ) as Dictionary<string, object>;

                    newList.Add( newDictionary );
                }

                return newList;
            }

            // Replace ExpandoObject.
            var expando = input as ExpandoObject;

            if ( expando != null )
            {
                // Replace with a list of Dictionary<string, object>
                var newValue = new Dictionary<string, object>( expando );

                return ConvertExpandoObjectsToDictionaries( newValue );
            }
            
            var dictionary = input as IDictionary<string, object>;

            if ( dictionary != null )
            {
                var replacementItems = new Dictionary<string, object>();

                foreach ( var kvp in dictionary )
                {
                    var newValue = ConvertExpandoObjectsToDictionaries( kvp.Value );

                    if ( newValue != kvp.Value )
                    {
                        replacementItems.Add( kvp.Key, newValue );
                    }
                }

                if ( replacementItems.Any() )
                {
                    foreach (var kvp in replacementItems )
                    {
                        dictionary.AddOrReplace( kvp.Key, kvp.Value );
                    }
                }

                return dictionary;
            }

            //var list = input as IList;

            //if ( list != null )
            //{
            //    var replacementList = new List<object>();

            //    for ( int i = 0; i < list.Count; i++ )
            //    {
            //        var newItem = ConvertExpandoObjectsToDictionaries( list[i] );

            //        if ( newItem != list[i] )
            //        {
            //            replacementList.Add( newItem );
            //        }

            //        list[i] = ConvertExpandoObjectsToDictionaries( list[i] );
            //    }

            //    if ( replacementList)
            //    return list;
            //}

            //var collection = input as ICollection;

            //if ( collection != null )
            //{
            //    var addElements = new List<object>();
            //    var removeElements = new List<object>();

            //    foreach ( var element in collection )
            //    {
            //        var newElement = ConvertExpandoObjectsToDictionaries( element );
            //        //collection.Add( list[i] = ConvertExpandoObjectsToDictionaries( list[i] );
            //    }
            //    return list;
            //}

            return input;

            //return RecursivelyConvertIDictToDict( expando );
        }

        /// <summary>
        /// This method takes an <see cref="IDictionary{string, object}"/> and recursively converts it to a <see cref="Dictionary{string, object}"/>. 
        /// The idea is that every <see cref="IDictionary{string, object}"/> in the tree will be of type <see cref="Dictionary{string, object}"/> instead of some other implementation like <see cref="ExpandoObject"/>.
        /// </summary>
        /// <param name="value">The <see cref="IDictionary{string, object}"/> to convert</param>
        /// <returns>The fully converted <see cref="Dictionary{string, object}"/></returns>
        private Dictionary<string, object> RecursivelyConvertIDictToDict( IDictionary<string, object> value )
        {
            var newDictionary = value.ToDictionary(
                keySelector => keySelector.Key,
                elementSelector =>
                {
                    // if it's another IDict just go through it recursively
                    if ( elementSelector.Value is IDictionary<string, object> dict )
                    {
                        return RecursivelyConvertIDictToDict( dict );
                    }

                    // if it's an IEnumerable check each element
                    if ( elementSelector.Value is IEnumerable<object> list )
                    {
                        // go through all objects in the list
                        // if the object is an IDict -> convert it
                        // if not keep it as is
                        return list
                                    .Select( o => o is IDictionary<string, object>
                                 ? RecursivelyConvertIDictToDict( ( IDictionary<string, object> ) o )
                                 : o
                                    );
                    }

                    // neither an IDict nor an IEnumerable -> it's fine to just return the value it has
                    return elementSelector.Value;
                }
            );

            return newDictionary;
        }

        /// <summary>
        /// Add a set of filters to this template context by extracting the implementing methods from the supplied class.
        /// Filter methods must be marked as Public and Static.
        /// </summary>
        /// <param name="implementingClass"></param>
        //public void AddFiltersFromClass( Type implementingClass )
        //{
        //    Template.RegisterFilter( implementingClass );

        //    //var scriptObject1 = new ScriptObject();
        //    //// Declare a function `myfunc` returning the string `Yes`
        //    //scriptObject1.Import( implementingClass, renamer: member => member.Name );

        //    //_templateContext.PushGlobal( scriptObject1 );
        //}

        public void Dispose()
        {
            //
        }
        public void RegisterSafeType( Type type, string[] allowedMembers = null )
        {
            if ( allowedMembers == null )
            {
                allowedMembers = type.GetProperties().Select( x => x.Name )
                    .Union( type.GetFields().Where(x => x.IsPublic ).Select( x => x.Name ) )
                    .ToArray();
            }

            Template.RegisterSafeType( type, allowedMembers );
        }

        public bool TryRender( IDictionary<string, object> values, out string output )
        {
            throw new NotImplementedException();
        }

        public bool TryRender( LavaRenderParameters parameters, out string output )
        {
            throw new NotImplementedException();
        }

        public bool TryRender( IDictionary<string, object> values, out string output, out IList<Exception> errors )
        {
            throw new NotImplementedException();
        }

        public bool TryRender( LavaRenderParameters parameters, out string output, out IList<Exception> errors )
        {
            throw new NotImplementedException();
        }

        //private class ExpandoConverter : JsonConverter
        //{
        //    public override bool CanConvert( Type objectType )
        //    {
        //        return ( objectType == typeof( ExpandoObject ) );
        //    }

        //    public override object ReadJson( JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer )
        //    {
        //        var dictionary = new Dictionary<string, object>( ( Dictionary<string, object> ) existingValue );

        //        return dictionary;
        //    }

        //    public override bool CanWrite
        //    {
        //        get { return false; }
        //    }

        //    public override void WriteJson( JsonWriter writer, object value, JsonSerializer serializer )
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}
