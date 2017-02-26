using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streaks.Utilities
{
    public interface IClock
    {
        /// <summary>
        /// The current universal date and time.
        /// </summary>
        DateTime Time { get; }
    }
}
