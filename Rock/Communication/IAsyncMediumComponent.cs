using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rock.Communication
{
    /// <summary>
    /// In order to take advantage of async your Medium Component should implement this interface.
    /// </summary>
    public interface IAsyncMediumComponent
    {
        /// <summary>
        /// Sends the asynchronous.
        /// </summary>
        /// <param name="communication">The communication.</param>
        /// <returns></returns>
        Task SendAsync( Model.Communication communication );

    }
}
