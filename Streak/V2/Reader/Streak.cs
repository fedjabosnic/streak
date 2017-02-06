using System;
using System.IO;

namespace Streak.V2.Reader
{
    public class Streak
    {
        private long _offset;
        private readonly Index _index;
        private readonly FileStream _file;

        public Streak(string path)
        {
            // Open file and create index
            _index = new Index(path);
            _file = new FileStream(path + @"\main.dat", FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 512000, FileOptions.SequentialScan);
        }

        public void Skip(long count)
        {
            // Skip on the index
            _index.Skip(count);

            // Calculate delta
            var delta = _index.Current.End - _offset;

            // Move file and offset
            _file.Seek(delta, SeekOrigin.Current);
            _offset += delta;
        }

        private readonly byte[] temp = new byte[16000000];

        public Entry Next()
        {
            var index = _index.Next();
            var data = new byte[index.End - index.Start];

            // Read data from the file
            var read = 0;
            while ((read = _file.Read(data, 0 + read, data.Length - read)) != data.Length)
            {
            }

            _offset += data.Length;

            // Create and return entry
            return new Entry
            {
                Index = index,
                Data = data
            };
        }

        public struct Entry
        {
            internal Index.Entry Index { get; set; }

            public byte[] Data { get; set; }
            public long Position => Index.Position;
        }
    }
}
