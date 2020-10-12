using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Lava
{
    /// <summary>
    /// Specifies that this object can provide a LavaDataObject.
    /// </summary>
    public interface ILavaDataObjectSource
    {
        ILavaDataObject GetLavaDataObject();
    }

    /// <summary>
    /// Specifies that this object can be made accessible in a Lava template.
    /// </summary>
    public interface ILavaDataObject
    {
        object GetValue( object key );

        bool ContainsKey( object key );

        /// <summary>
        /// Gets the available keys (for debugging info).
        /// </summary>
        /// <value>
        /// The available keys.
        /// </value>
        List<string> AvailableKeys { get; }
    }
}
