using System;
using System.Collections.Generic;
using System.IO;

namespace Streak.Store
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
            _index = File.Open($@"{path}\main.ind", FileMode.Append, FileAccess.Write, FileShare.Read);
            _items = File.Open($@"{path}\main.dat", FileMode.Append, FileAccess.Write, FileShare.Read);
        }

        public void Dispose()
        {
            _index.Dispose();
            _items.Dispose();
        }

        public void Write(IEnumerable<Event> events)
        {
            // TODO: Optimize for small batches (it is more efficient to write directly to disk)
            // TODO: Thread safety (shouldn't call write more than once at a time)

            // Write all data in the batch to temporary streams
            using (var index = new MemoryStream())
            using (var items = new MemoryStream())
            {
                var position = _index.Length / 16;
                var offset = _items.Length;

                foreach (var e in events)
                {
                    e.Position = e.Position != 0 ? e.Position : ++position;
                    e.Timestamp = e.Timestamp != DateTime.MinValue ? e.Timestamp : DateTime.UtcNow;

                    // Write data
                    var length = e.SerializeTo(items);

                    // Write index (improve this)
                    index.Write(BitConverter.GetBytes(offset), 0, 8);
                    index.Write(BitConverter.GetBytes(offset + length), 0, 8);

                    offset += length;
                }

                items.Position = 0;
                index.Position = 0;

                // Update real files
                items.CopyTo(_items);
                index.CopyTo(_index);

                // Flush files
                _items.Flush();
                _index.Flush();
            }
        }
    }
}