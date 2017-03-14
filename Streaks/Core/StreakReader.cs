using System;
using Streaks.Core.Data;
using Streaks.Core.IO;
using Streaks.Utilities;

namespace Streaks.Core
{
    public class StreakReader : IStreakReader
    {
        internal IClock Clock { get; }
        internal IFileReader Log { get; }
        internal IFileReader Index { get; }

        // TODO: Remove magic numbers
        public long Count => Index.Length / 12;

        internal StreakReader(IClock clock, IFileReader log, IFileReader index)
        {
            Clock = clock;

            Log = log;
            Index = index;
        }

        public byte[] Read(long position)
        {
            var index = Index.ReadIndex(position - 1);
            var log = Log.ReadLog(index);

            return log.Data;
        }

        public void Dispose()
        {
            Log?.Dispose();
            Index?.Dispose();
        }
    }
}