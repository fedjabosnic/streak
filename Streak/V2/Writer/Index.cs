using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Streak.V2.OS;

namespace Streak.V2.Writer
{
    internal class Index
    {
        private readonly string _path;
        private readonly byte[] _buffer;
        private readonly IFileWriter _file;

        private long _position = 0;
        private int _dirty;

        public Index(string path)
        {
            _path = path;
            // Open file and create buffer
            _file = new FileWriter(path + @"\main.ind");
            _buffer = new byte[16];

            _position = _file.Length / 16;

            //Task.Factory.StartNew(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1);

            //        lock (_file)
            //        {
            //            if (_dirty > 0) Flush();
            //        }
            //    }
            //}, TaskCreationOptions.LongRunning);
        }

        internal unsafe void Append(long start, long end)
        {
            lock (_file)
            {
                // Update buffer
                fixed (byte* b = _buffer) *(long*)(b + 0) = start;
                fixed (byte* b = _buffer) *(long*)(b + 8) = end;

                // Update file
                _file.Write(_buffer);//, 0, 16);
                _dirty++;

                //if (_dirty % 100 == 0) Flush();
                _position++;
            }
        }

        internal void Flush()
        {
            lock (_file)
            {
                _file.Flush();
                _dirty = 0;
            }
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
    //    private readonly byte[] _buffer;
    //    private readonly FileStream _file;

    //    private int _dirty;

    //    public Index(string path)
    //    {
    //        // Open file and create buffer
    //        _file = new FileStream(path + @"\main.ind", FileMode.Append, FileAccess.Write, FileShare.Read, 512000, FileOptions.SequentialScan);
    //        _buffer = new byte[8];
    //    }

    //    internal unsafe void Append(long start, long end)
    //    {
    //        // Update buffer
    //        //fixed (byte* b = _buffer) *(long*)(b + 0) = start;
    //        fixed (byte* b = _buffer) *(long*)(b + 0) = end;

    //        // Update file
    //        _file.Write(_buffer, 0, 8);
    //        _dirty++;

    //        if(_dirty % 100 == 0) _file.Flush();
    //    }

    //    internal void Flush()
    //    {
    //        //_file.Flush();
    //    }

    //    internal struct Entry
    //    {
    //        internal long Position { get; set; }
    //        internal long Start { get; set; }
    //        internal long End { get; set; }
    //    }
    //}
}
