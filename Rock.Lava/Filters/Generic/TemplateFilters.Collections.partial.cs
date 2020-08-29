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
using System.Linq;

namespace Rock.Lava.Filters
{
    public static partial class TemplateFilters
    {
        static Random _randomNumberGenerator = new Random();

        /// <summary>
        /// Determines whether the input collection contains the specified value.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="containValue">The search value.</param>
        /// <returns>
        ///   <c>true</c> if the input collection contains the specified search value; otherwise, <c>false</c>.
        /// </returns>
        public static bool Contains( object input, object containValue )
        {
            var inputList = ( input as IList );

            if ( inputList != null )
            {
                return inputList.Contains( containValue );
            }

            return false;
        }

        /// <summary>
        /// Extracts a single item from an array.
        /// </summary>
        /// <param name="input">The input object to extract one element from.</param>
        /// <param name="index">The index number of the object to extract.</param>
        /// <returns>The single object from the array or null if not found.</returns>
        public static object Index( object input, object index )
        {
            if ( input == null || index == null )
            {
                return input;
            }

            if ( !( input is IList ) )
            {
                return input;
            }

            var inputList = input as IList;
            var indexInt = index.ToString().AsIntegerOrNull();
            if ( !indexInt.HasValue || indexInt.Value < 0 || indexInt.Value >= inputList.Count )
            {
                return null;
            }

            return inputList[indexInt.Value];
        }

        /// <summary>
        /// Orders a collection of elements by the specified property (or properties)
        /// and returns a new collection in that order.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="property">The property or properties to order the collection by.</param>
        /// <returns>A new collection sorted in the requested order.</returns>
        /// <example>
        ///     {% assign members = 287635 | GroupById | Property:'Members' | OrderBy:'GroupRole.IsLeader desc,Person.FullNameReversed' %}
        ///    <ul>
        ///    {% for member in members %}
        ///        <li>{{ member.Person.FullName }} - {{ member.GroupRole.Name }}</li>
        ///    {% endfor %}
        ///    </ul>
        /// </example>
        public static IEnumerable OrderBy( object input, string property )
        {
            IEnumerable<object> e = input is IEnumerable<object> ? input as IEnumerable<object> : new List<object> { input };

            if ( !e.Any() || string.IsNullOrWhiteSpace( property ) )
            {
                return e;
            }

            //
            // Create a list of order by objects for the field to order by
            // and the ascending/descending flag.
            //
            var orderBy = property
                .Split( ',' )
                .Select( s => s.Split( new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ) )
                .Select( a => new { Property = a[0], Descending = a.Length >= 2 && "desc" == a[1].ToLower() } )
                .ToList();

            //
            // Do initial ordering of first property requested.
            //
            IOrderedQueryable<object> qry;
            if ( orderBy[0].Descending )
            {
                qry = e.Cast<object>().AsQueryable().OrderByDescending( d => ExtensionMethods.GetPropertyValue( d, orderBy[0].Property ) );
            }
            else
            {
                qry = e.Cast<object>().AsQueryable().OrderBy( d => ExtensionMethods.GetPropertyValue( d, orderBy[0].Property ) );
            }

            //
            // For the rest use ThenBy and ThenByDescending.
            //
            for ( int i = 1; i < orderBy.Count; i++ )
            {
                var propertyName = orderBy[i].Property; // This can't be inlined. -dsh

                if ( orderBy[i].Descending )
                {
                    qry = qry.ThenByDescending( d => ExtensionMethods.GetPropertyValue( d, propertyName ) );
                }
                else
                {
                    qry = qry.ThenBy( d => ExtensionMethods.GetPropertyValue( d, propertyName ) );
                }
            }

            return qry.ToList();
        }

        /// <summary>
        /// Selects the set of values of a named property from the items in a collection.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="selectKey">The select key.</param>
        /// <returns></returns>
        public static object Select( object input, string selectKey )
        {
            if ( input == null )
            {
                return input;
            }

            var enumerableInput = input as IEnumerable;

            if ( enumerableInput == null )
            {
                return input;
            }

            var result = new List<object>();

            foreach ( var value in enumerableInput )
            {
                // TODO: Find a cross-framework solution to support ILiquidizable objects.

                //if ( value is ILiquidizable )
                //{
                //    var liquidObject = value as ILiquidizable;
                //    if ( liquidObject.ContainsKey( selectKey ) )
                //    {
                //        result.Add( liquidObject[selectKey] );
                //    }
                //}
                //else
                if ( value is IDictionary<string, object> )
                {
                    var dictionaryObject = value as IDictionary<string, object>;
                    if ( dictionaryObject.ContainsKey( selectKey ) )
                    {
                        result.Add( dictionaryObject[selectKey] );
                    }
                }
                else
                {
                    result.Add( value.GetPropertyValue( selectKey ) );
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the size of an array or the length of a string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int Size( object input )
        {
            if ( input is string )
            {
                return ( ( string ) input ).Length;
            }

            if ( input is IEnumerable )
            {
                return ( ( IEnumerable ) input ).Cast<object>().Count();
            }

            return 0;
        }

        /// <summary>
        /// Rearranges an array in a random order
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static object Shuffle( object input )
        {
            if ( input == null )
            {
                return input;
            }

            if ( !( input is IList ) )
            {
                return input;
            }

            var inputList = input as IList;
            int n = inputList.Count;
            while ( n > 1 )
            {
                n--;
                int k = _randomNumberGenerator.Next( n + 1 );
                var value = inputList[k];
                inputList[k] = inputList[n];
                inputList[n] = value;
            }

            return inputList;
        }
    }
}
