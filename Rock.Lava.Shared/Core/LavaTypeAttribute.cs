using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Lava
{
        /// <summary>
        /// Specifies the type is safe to be rendered by DotLiquid.
        /// </summary>
        [AttributeUsage( AttributeTargets.Class )]
        public class LavaTypeAttribute : Attribute
        {
            /// <summary>
            /// An array of property and method names that are allowed to be called on the object.
            /// </summary>
            public string[] AllowedMembers { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="allowedMembers">An array of property and method names that are allowed to be called on the object.</param>
            public LavaTypeAttribute( params string[] allowedMembers )
            {
                AllowedMembers = allowedMembers;
            }
        }
}
