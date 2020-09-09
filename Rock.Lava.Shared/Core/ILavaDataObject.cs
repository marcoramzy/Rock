using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Lava
{
    /// <summary>
    /// Specifies the type is safe to be rendered by Lava.
    /// </summary>
    public interface ILavaDataObject
    {
        object ToLiquid();
    }
}
