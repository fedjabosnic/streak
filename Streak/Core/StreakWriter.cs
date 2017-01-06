using System;
using System.Collections.Generic;
using System.IO;

namespace Streak.Core
{
    public class StreakWriter : IDisposable
    {
        private readonly FileStream _index;
        private readonly FileStream _items;

        public StreakWriter(string path)
        {
            // Ensure directory exists
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // Open or create the relevant files
            _index = new FileStream($@"{path}\main.ind", FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.None);
            _items = new FileStream($@"{path}\main.dat", FileMode.Append, FileAccess.Write, FileShare.Read, 4096, FileOptions.None);
        }

        public void Dispose()
        {
            _index.Dispose();
            _items.Dispose();
        }

        public void Write(IEnumerable<Event> events)
        {
            // TODO: Optimize for small batches as it might be more efficient to write directly to disk

            // Write all data in the batch to in memory streams
            using (var index = new MemoryStream())
            using (var items = new MemoryStream())
            {
                var position = _index.Length / 16;
                var offset = _items.Length;

                foreach (var e in events)
                {
                    // Shoul this be in user space?
                    e.Position = e.Position != 0 ? e.Position : ++position;
                    e.Timestamp = e.Timestamp != DateTime.MinValue ? e.Timestamp : DateTime.UtcNow;

                    // Write data (consider a serializer abstraction)
                    var length = e.SerializeTo(items);

                    // Write index (improve this)
                    index.Write(BitConverter.GetBytes(offset), 0, 8);
                    index.Write(BitConverter.GetBytes(offset + length), 0, 8);

                    offset += length;
                }

                // Rewind in memory streams
                items.Position = 0;
                index.Position = 0;

                // Copy to files
                items.CopyTo(_items, 4096);
                index.CopyTo(_index, 4096);

                // Flush files
                _items.Flush();
                _index.Flush();
            }
        }
    }
}