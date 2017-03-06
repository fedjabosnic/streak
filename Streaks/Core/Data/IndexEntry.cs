using System;

namespace Streaks.Core.Data
{
    public struct IndexEntry
    {
        public DateTime Timestamp { get; set; }
        public long Offset { get; set; }
        public int Length { get; set; }
    }
}