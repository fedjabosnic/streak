using System;
using System.Diagnostics;

namespace Streak.V2.OS
{
    /// <summary>
    /// A dynamically resizing buffer that stores data in a single byte array and is resized when no longer large enough.
    /// To preempt and circumvent resizing, you can preallocate a larger initial apacity with the contructor argument.
    /// <remarks>
    /// The underlying buffer array is exposed publically via the Data property to allow copy-free access to the buffered data - be careful!
    /// </remarks>
    /// <remarks>
    /// This class is not thread safe.
    /// </remarks>
    /// </summary>
    internal class Buffer
    {
        private byte[] _buffer;
        private int _length;

        /// <summary>
        /// The buffer data.
        /// <remarks>
        /// Do not rely on the length of this array use the Buffer.Length property instead.
        /// </remarks>
        /// </summary>
        public byte[] Data => _buffer;

        /// <summary>
        /// The buffer length.
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Creates a new buffer with an initial capacity.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        public Buffer(int capacity = 4096)
        {
            _buffer = new byte[capacity];
            _length = 0;
        }

        /// <summary>
        /// Add data to the buffer.
        /// </summary>
        /// <param name="data">The data to add.</param>
        public void Add(byte[] data)
        {
            if (_buffer.Length < _length + data.Length)
            {
                var size = _buffer.Length;

                while (size - _length < data.Length) size *= 2;

                Array.Resize(ref _buffer, size);

                //var buffer = new byte[size];

                //// Copy data to new buffer
                //System.Buffer.BlockCopy(_buffer, 0, buffer, 0, _buffer.Length);

                //// Reset buffer
                //_buffer = buffer;

                Console.WriteLine("Buffer expanded to " + _buffer.Length);
            }

            System.Buffer.BlockCopy(data, 0, _buffer, _length, data.Length);

            _length += data.Length;
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        public void Clear()
        {
            _length = 0;

            // TODO: Should we zero out the contents?
        }
    }
}