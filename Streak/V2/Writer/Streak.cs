using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Streak.V2.Writer
{
    public class Streak
    {
        private long _offset;
        private readonly Index _index;
        private readonly FileStream _file;

        private int _dirty;

        public Streak(string path)
        {
            // Open file and create index
            _index = new Index(path);
            _file = new FileStream(path + @"\main.dat", FileMode.Append, FileAccess.Write, FileShare.Read, 512000, FileOptions.SequentialScan);

            // Set offset
            _offset = _file.Length;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(100);

                    lock(_file)
                    {
                        if (_dirty > 0) _file.Flush();
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Append(Entry entry)
        {
            lock (_file)
            {
                // Write data
                _file.Write(entry.Data, 0, entry.Data.Length);
                _dirty++;

                if (_dirty % 100 == 0) _file.Flush();

                // Update index
                _index.Append(_offset, _offset + entry.Data.Length);

                // Update offset
                _offset += entry.Data.Length;
            }
        }

        public void Flush()
        {
            //_file.Flush();
            //_index.Flush();
        }

        public struct Entry
        {
            internal Index.Entry Index { get; set; }

            public byte[] Data { get; set; }
            public long Position => Index.Position;
        }
    }
}
