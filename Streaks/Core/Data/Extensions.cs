using System;
using System.Threading;
using Streaks.Core.IO;

namespace Streaks.Core.Data
{
    public static unsafe class Extensions
    {
        /// <summary>
        /// A buffer used for reading/writing index entries to avoid temporary allocations and garbage collection
        /// </summary>
        /// <remarks>
        /// Should match the size of a serialized index entry (or be bigger)
        /// </remarks>
        internal static ThreadLocal<byte[]> IndexBuffer = new ThreadLocal<byte[]>(() => new byte[12]);

        public static LogEntry ReadLog(this IFileReader file, IndexEntry index)
        {
            var buffer = new byte[index.Length];

            file.Read(buffer, index.Offset);

            return new LogEntry { Data = buffer };
        }

        public static IndexEntry ReadIndex(this IFileReader file, long position)
        {
            var buffer = IndexBuffer.Value;

            file.Read(buffer, (position - 1) * 12);

            return new IndexEntry
            {
                Offset = BitConverter.ToInt64(buffer, 0),
                Length = BitConverter.ToInt32(buffer, 8)
            };
        }

        public static void Write(this IFileWriter file, LogEntry log)
        {
            file.Write(log.Data);
        }

        public static void Write(this IFileWriter file, IndexEntry index)
        {
            fixed (byte* b = IndexBuffer.Value)
            {
                *(long*)(b + 00) = index.Offset;
                *(long*)(b + 08) = index.Length;
            }

            file.Write(IndexBuffer.Value);
        }
    }
}