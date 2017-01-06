using System;
using System.IO;
using System.Text;

namespace Streak.Core
{
    public class Event
    {
        /// <summary> The position. </summary>
        public long Position { get; set; }
        /// <summary> The timestamp. </summary>
        public DateTime Timestamp { get; set; }

        /// <summary> The event type. </summary>
        public string Type { get; set; }
        /// <summary> The event data. </summary>
        public string Data { get; set; }
        /// <summary> The event meta data. </summary>
        public string Meta { get; set; }

        public long SerializeTo(Stream stream)
        {
            var start = stream.Position;

            using (var sw = new BinaryWriter(stream, new UTF8Encoding(), true))
            {
                sw.Write(Position);
                sw.Write(Timestamp.Ticks);
                sw.Write(Type);
                sw.Write(Data);
                sw.Write(Meta);
            }

            return stream.Position - start;
        }

        public void DeserializeFrom(Stream stream)
        {
            using (var sw = new BinaryReader(stream, new UTF8Encoding(), true))
            {
                Position = sw.ReadInt64();
                Timestamp = new DateTime(sw.ReadInt64(), DateTimeKind.Utc);
                Type = sw.ReadString();
                Data = sw.ReadString();
                Meta = sw.ReadString();
            }
        }
    }
}
