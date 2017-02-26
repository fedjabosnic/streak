using System;
using System.IO;
using System.Text;

namespace Streaks.Core
{
    public class Entry
    {
        /// <summary> The position. </summary>
        public long Position { get; set; }
        /// <summary> The timestamp. </summary>
        public DateTime Timestamp { get; set; }
        /// <summary> The event data. </summary>
        public string Data { get; set; }

        public long SerializeTo(Stream stream)
        {
            var start = stream.Position;

            using (var sw = new BinaryWriter(stream, new UTF8Encoding(), true))
            {
                sw.Write(Position);
                sw.Write(Timestamp.Ticks);
                sw.Write(Data);
            }

            return stream.Position - start;
        }

        public void DeserializeFrom(Stream stream)
        {
            using (var sw = new BinaryReader(stream, new UTF8Encoding(), true))
            {
                Position = sw.ReadInt64();
                Timestamp = new DateTime(sw.ReadInt64(), DateTimeKind.Utc);
                Data = sw.ReadString();
            }
        }
    }
}
