using Streak.V2.OS;

namespace Streak.V4
{
    public class Indexer : IIndexer
    {
        private readonly IFileWriter _file;
        private readonly byte[] _buffer;

        public Indexer(IFileWriter file)
        {
            _file = file;
            _buffer = new byte[FileSegmentInfo.Size];
        }

        public unsafe Index Append(FileSegmentInfo location)
        {
            fixed (byte* b = _buffer) *(long*)(b + 0) = location.Length;
            fixed (byte* b = _buffer) *(long*)(b + 8) = location.Offset;

            var info = _file.Write(_buffer);

            return new Index
            {
                Position = (info.Offset + info.Length) / 16
            };
        }

        public void Commit()
        {
            _file.Flush();
        }
    }
}