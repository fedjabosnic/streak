using System;

namespace Streaks.Utilities
{
    /// <summary> 
    /// A dynamically resizing buffer that stores data in a single byte array and is resized when no longer large enough. 
    /// To preempt and circumvent resizing, you can preallocate a larger initial capacity with the contructor argument. 
    /// To prevent and circumvent out of memory issues, you can declare the maximum allowed size that the buffer can grow to. 
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
        private int _max;

        /// <summary>
        /// The buffered data.
        /// <remarks>
        /// Do not rely on the length of this array use the Buffer.Length property instead.
        /// </remarks>
        /// </summary>
        internal byte[] Data => _buffer;

        /// <summary> The buffer length. </summary>
        internal int Length => _length;

        /// <summary> The buffer capacity. </summary>
        internal int Capacity => _buffer.Length;

        /// <summary>
        /// Creates a new buffer with optional initial and maximum capacities.
        /// </summary>
        /// <param name="capacity">The initial capacity of the buffer.</param>
        /// <param name="max">The maximum capacity of the buffer.</param>
        internal Buffer(int capacity = 4096, int max = int.MaxValue)
        {
            _buffer = new byte[capacity];
            _length = 0;
            _max = max;
        }

        /// <summary>
        /// Writes data to the buffer.
        /// </summary>
        /// <param name="data">The data to write.</param>
        internal void Write(byte[] data)
        {
            Write(data, 0, data.Length);
        }

        /// <summary>
        /// Writes data to the buffer.
        /// </summary>
        /// <param name="data">The data to write.</param>
        /// <param name="offset">The data offset to write.</param>
        /// <param name="length">The data length to write</param>
        internal void Write(byte[] data, int offset, int length)
        {
            EnsureCapacity(length);

            // TODO: Is this the most optimal way to do this? 
            System.Buffer.BlockCopy(data, offset, _buffer, _length, length);

            _length += length;
        }

        /// <summary>
        /// Clears the buffer.
        /// </summary>
        internal void Clear()
        {
            // TODO: Should we zero out the contents?

            _length = 0;
        }

        /// <summary>
        /// Ensures that the buffer has enough capacity for the additional data length. The buffer will be resized up to the maximum allowed capacity.
        /// </summary>
        /// <param name="length"></param>
        private void EnsureCapacity(int length)
        {
            if (_buffer.Length >= _length + length) return;

            var size = _buffer.Length;

            while (size - _length < length) size *= 2;

            if (size > _max) size = _max;

            // TODO: Deal with not enough capacity once we reach max (exception or some form of back-pressure?)

            // TODO: Is this the most optimal way to do this?
            Array.Resize(ref _buffer, size);
        }
    }
}