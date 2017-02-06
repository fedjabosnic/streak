using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Streak.V3.File
{
    public class Reader
    {
        private readonly IFileStream _stream;

        public Reader(IFileStream stream)
        {
            _stream = stream;
        }

        public void Read(byte[] array)
        {
            var read = 0;

            while ((read = _stream.Read(array, 0 + read, array.Length - read)) != array.Length)
            {
                //Thread.Sleep(cycle);
            }
        }
    }

    public class Writer
    {
        private readonly IFileStream _stream;

        private byte[] _buffer;
        private int _offset;

        public Writer(IFileStream stream, int buffer)
        {
            _stream = stream;

            _buffer = new byte[buffer];
            _offset = 0;
        }

        public void Write(byte[] array)
        {
            lock (_stream)
            {
                if (_offset + array.Length > _buffer.Length)
                {
                    Debug.WriteLine("Expanding data buffer for " + _stream.Name);

                    var size = _buffer.Length;

                    while (size - _offset < array.Length) size *= 2;

                    var buffer = new byte[size];

                    // Copy data to new buffer
                    Buffer.BlockCopy(_buffer, 0, buffer, 0, _buffer.Length);

                    // Reset buffer
                    _buffer = buffer;

                   // Debug.WriteLine("Buffer expanded to " + _buffer.Length);
                }

                Buffer.BlockCopy(array, 0, _buffer, _offset, array.Length);

                _offset += array.Length;
            }
        }
    }

    public interface IFileStream
    {
        string Name { get; }

        int Read(byte[] array, int offset, int count);
        
    }
}
