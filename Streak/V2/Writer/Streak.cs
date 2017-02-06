using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Streak.V2.OS;

namespace Streak.V2.Writer
{
    public class Streak
    {
        private long _offset;
        private readonly Index _index;
        private readonly IFileWriter _file;

        private int _dirty;

        public Streak(string path)
        {
            // Open file and create index
            _index = new Index(path);
            _file = new FileWriter(path + @"\main.dat");

            // Set offset
            _offset = _file.Length;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);

                    lock(_file)
                    {
                        if (_dirty > 0) Flush();
                    }
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Append(Entry entry)
        {
            lock (_file)
            {
                // Write data
                _file.Write(entry.Data);//, 0, entry.Data.Length);
                _dirty++;

                //if (_dirty % 100 == 0) Flush();

                // Update index
                _index.Append(_offset, _offset + entry.Data.Length);

                // Update offset
                _offset += entry.Data.Length;
            }
        }

        public void Flush()
        {
            lock (_file)
            {
                _file.Flush();
                _index.Flush();
                _dirty = 0;
                //_index.Flush();
            }
        }

        public struct Entry
        {
            internal Index.Entry Index { get; set; }

            public byte[] Data { get; set; }
            public long Position => Index.Position;
        }
    }
}
