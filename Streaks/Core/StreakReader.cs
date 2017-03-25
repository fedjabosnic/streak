using System;
using Streaks.Core.Data;
using Streaks.Core.IO;

namespace Streaks.Core
{
    public class StreakReader : IStreakReader
    {
        internal IFileReader Log { get; }
        internal IFileReader Index { get; }

        // TODO: Remove magic numbers
        public long Size => Log.Length + Index.Length;
        public long Count => Index.Length / 12;

        internal StreakReader(IFileReader log, IFileReader index)
        {
            Log = log;
            Index = index;
        }

        public byte[] Read(long position)
        {
            var index = Index.ReadIndex(position);
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