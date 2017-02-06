using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Streak.V2.Reader
{
    internal class Index
    {
        private long _offset;
        private readonly byte[] _buffer;
        private readonly FileStream _file;

        internal Entry Current { get; private set; }

        public Index(string path)
        {
            // Open file and create buffer
            _file = new FileStream(path + @"\main.ind", FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 512000, FileOptions.SequentialScan);
            _buffer = new byte[16];
        }

        internal void Skip(long count)
        {
            // Ensure that the request is valid
            if (count <= 0) throw new Exception();
            if (_file.Length < (_offset + count) * 16) throw new Exception();

            // Set offset
            _offset += count - 1;

            // Move file to offset
            _file.Seek(_offset * 16, SeekOrigin.Current);

            // Move to current
            Next();
        }

        internal Entry Next()
        {
            var read = 0;
            while ((read = _file.Read(_buffer, 0 + read, _buffer.Length - read)) != _buffer.Length)
            {
            }

            _offset++;

            // Set and return the next entry
            return Current = new Entry
            {
                Position = _offset,
                Start = BitConverter.ToInt64(_buffer, 0),
                End = BitConverter.ToInt64(_buffer, 8)
            };
        }

        internal struct Entry
        {
            internal long Position { get; set; }
            internal long Start { get; set; }
            internal long End { get; set; }
        }
    }

    //internal class Index
    //{
    //    private long _offset;
    //    private readonly byte[] _buffer;
    //    private readonly FileStream _file;

    //    internal Entry Current { get; private set; }

    //    public Index(string path)
    //    {
    //        // Open file and create buffer
    //        _file = new FileStream(path + @"\main.ind", FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 512000, FileOptions.SequentialScan);
    //        _buffer = new byte[8];
    //    }

    //    internal void Skip(long count)
    //    {
    //        // Ensure that the request is valid
    //        if (count <= 0) throw new Exception();
    //        if (_file.Length < (_offset + count) * 8) throw new Exception();

    //        // Set offset
    //        _offset += count - 2;

    //        // Move file to offset
    //        _file.Seek(_offset * 8, SeekOrigin.Current);

    //        // Move to current
    //        Next();
    //        Next();
    //    }

    //    internal Entry Next()
    //    {
    //        while (_file.Read(_buffer, 0, 8) != 8)
    //        {
    //            //Debug.WriteLine("Bugger");
    //            Thread.Sleep(100);
    //        }

    //        _offset++;

    //        // Set and return the next entry
    //        return Current = new Entry
    //        {
    //            Position = _offset,
    //            Start = Current.End,
    //            End = BitConverter.ToInt64(_buffer, 0)
    //        };
    //    }

    //    internal struct Entry
    //    {
    //        internal long Position { get; set; }
    //        internal long Start { get; set; }
    //        internal long End { get; set; }
    //    }
    //}
}
