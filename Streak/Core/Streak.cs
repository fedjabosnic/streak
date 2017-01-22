using System;
using System.Collections.Generic;
using System.IO;

namespace Streak.Core
{
    public class Streak : IStreak
    {
        private readonly string _path;

        private readonly StreakReader _reader;
        private readonly StreakWriter _writer;

        public long Length => new FileInfo($@"{_path}\main.ind").Length / 16;

        public Streak(string path, bool reader = true, bool writer = false)
        {
            _path = path;

            _reader = reader ? new StreakReader(path) : null;
            _writer = writer ? new StreakWriter(path) : null;
        }

        public void Dispose()
        {
            _reader?.Dispose();
            _writer?.Dispose();
        }

        public void Save(Entry entry)
        {
            if (_writer == null) throw new NotSupportedException();

            _writer.Write(entry);
        }

        public void Save(IEnumerable<Entry> events)
        {
            if (_writer == null) throw new NotSupportedException();

            _writer.Write(events);
        }

        public IEnumerable<Entry> Get(long @from = 1, long to = long.MaxValue, bool continuous = false)
        {
            if (_reader == null) throw new NotSupportedException();

            return _reader.Read(from, to, continuous);
        }
    }
}