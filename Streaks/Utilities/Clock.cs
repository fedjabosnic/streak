using System;
using System.Runtime.InteropServices;

namespace Streaks.Utilities
{
    /// <summary>
    /// A clock which provides high resolution time if available in the current environment.
    /// </summary>
    /// <remarks>
    /// Adapted from https://manski.net/2014/07/high-resolution-clock-in-csharp/
    /// </remarks>
    public class Clock : IClock
    {
        [DllImport("Kernel32.dll", CallingConvention = CallingConvention.Winapi)]
        private static extern void GetSystemTimePreciseAsFileTime(out long filetime);

        public bool IsHighPrecision{ get; }

        public DateTime Time
        {
            get
            {
                if (!IsHighPrecision) return DateTime.UtcNow;

                long filetime;
                GetSystemTimePreciseAsFileTime(out filetime);

                return DateTime.FromFileTimeUtc(filetime);
            }
        }

        public Clock()
        {
            try
            {
                long _;
                GetSystemTimePreciseAsFileTime(out _); IsHighPrecision = true;
            }
            catch (Exception)
            {
                IsHighPrecision = false;
            }
        }
    }
}