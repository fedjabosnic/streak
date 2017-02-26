using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Streaks.Core
{
    public class StreakWriter : IDisposable
    {
        private readonly FileStream _index;
        private readonly FileStream _items;

        private long _indexLength;
        private long _itemsLength;

        public StreakWriter(string path)
        {
            // Ensure directory exists
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            // Open or create the relevant files
            _index = new FileStream($@"{path}\main.ind", FileMode.Append, FileAccess.Write, FileShare.Read);
            _items = new FileStream($@"{path}\main.dat", FileMode.Append, FileAccess.Write, FileShare.Read);

            _indexLength = _index.Length;
            _itemsLength = _items.Length;
        }

        public void Dispose()
        {
            _index.Dispose();
            _items.Dispose();
        }

        public void Write(IEnumerable<Entry> events)
        {
            var all = events.ToList();

            if (all.Count < 10)
                WriteDirect(all);
            else
                WriteBuffered(all);
        }

        private void WriteDirect(List<Entry> events)
        {
            var position = _indexLength / 16;
            var offset = _itemsLength;

            foreach (var e in events)
            {
                // Should this be in user space?
                e.Position = e.Position != 0 ? e.Position : ++position;
                e.Timestamp = e.Timestamp != DateTime.MinValue ? e.Timestamp : DateTime.UtcNow;

                // Write data (consider a serializer abstraction)
                var length = e.SerializeTo(_items);

                // Write index (improve this)
                _index.Write(BitConverter.GetBytes(offset), 0, 8);
                _index.Write(BitConverter.GetBytes(offset += length), 0, 8);
            }

            // Flush files
            _items.Flush();
            _index.Flush();

            // Update offsets
            _itemsLength += offset;
            _indexLength += events.Count * 16;
        }

        private void WriteBuffered(List<Entry> events)
        {
            // Write all data in the batch to in memory streams
            using (var index = new MemoryStream())
            using (var items = new MemoryStream())
            {
                var position = _indexLength / 16;
                var offset = _itemsLength;

                foreach (var e in events)
                {
                    // Should this be in user space?
                    e.Position = e.Position != 0 ? e.Position : ++position;
                    e.Timestamp = e.Timestamp != DateTime.MinValue ? e.Timestamp : DateTime.UtcNow;

                    // Write data (consider a serializer abstraction)
                    var length = e.SerializeTo(items);

                    // Write index (improve this)
                    index.Write(BitConverter.GetBytes(offset), 0, 8);
                    index.Write(BitConverter.GetBytes(offset += length), 0, 8);

                    offset += length;
                }

                // Rewind in memory streams
                items.Position = 0;
                index.Position = 0;

                // Copy to files
                items.CopyTo(_items);
                index.CopyTo(_index);

                // Flush files
                _items.Flush();
                _index.Flush();

                // Update offsets
                _itemsLength += offset;
                _indexLength += events.Count * 16;
            }
        }
    }
}