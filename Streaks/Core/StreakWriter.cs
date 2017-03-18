using System;
using Streaks.Core.Data;
using Streaks.Core.IO;

namespace Streaks.Core
{
    public class StreakWriter : IStreakWriter
    {
        internal IFileWriter Log { get; }
        internal IFileWriter Index { get; }

        internal StreakWriter(IFileWriter log, IFileWriter index)
        {
            Log = log;
            Index = index;
        }

        public void Write(byte[] data)
        {
            var log = new LogEntry { Data = data };
            var index = new IndexEntry { Offset = Log.Position, Length = data.Length };

            Log.Write(log);
            Index.Write(index);
        }

        public void Commit()
        {
            Log.Flush();
            Index.Flush();
        }

        public void Discard()
        {
            Log.Discard();
            Index.Discard();
        }

        public void Dispose()
        {
            Log?.Dispose();
            Index?.Dispose();
        }
    }
}