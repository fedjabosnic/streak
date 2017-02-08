namespace Streak.V2.OS
{
    /// <summary>
    /// Describes a file segment, including an offset and length.
    /// </summary>
    public struct FileSegmentInfo
    {
        public static long Size => 16;

        /// <summary>
        /// The offset from the beginning of the file.
        /// </summary>
        public long Offset { get; set; }

        /// <summary>
        /// The length from the offset.
        /// </summary>
        public long Length { get; set; }

        public unsafe byte[] ToBytes()
        {
            var data = new byte[Size];

            fixed (byte* b = data) *(long*)(b + 0) = Offset;
            fixed (byte* b = data) *(long*)(b + 8) = Length;

            return data;
        }
    }
}