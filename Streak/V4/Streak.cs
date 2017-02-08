using System;

namespace Streak.V4
{
    public struct Index
    {
        public string Name { get; set; }
        public long Position { get; set; }
    }

    public struct Entry
    {
        public long Position { get; set; }
        public byte[] Data { get; set; }
    }

    public class StreakInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public decimal Size { get; set; }
        public decimal Length { get; set; }

        public IndexInfo Index { get; set; }
        public JournalInfo Journal { get; set; }
    }

    public class IndexInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public decimal Size { get; set; }
    }

    public class JournalInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public decimal Size { get; set; }
    }

    public class Streak
    {
        private readonly IIndexer _indexer;
        private readonly IJournaler _journaler;
        private readonly ICommitter _committer;

        /// <summary>
        /// Instantiates a new streak with the provided indexer, journaler and committer.
        /// </summary>
        /// <param name="indexer">The indexer.</param>
        /// <param name="journaler">The journaler.</param>
        /// <param name="committer">The committer.</param>
        public Streak(IIndexer indexer, IJournaler journaler, ICommitter committer)
        {
            _indexer = indexer;
            _journaler = journaler;
            _committer = committer;

            _committer.Register(Commit);
        }

        /// <summary>
        /// Appends the entry to the streak.
        /// <remarks>
        /// The entry will not be available to readers until it is committed.
        /// </remarks>
        /// </summary>
        /// <param name="entry">The entry to append.</param>
        /// <returns>The position in the streak where this entry will be added.</returns>
        public long Append(Entry entry)
        {
            lock (_journaler)
            {
                var data = entry.Data;
                var location = _journaler.Append(data);
                var index = _indexer.Append(location);

                _committer.Update();

                return index.Position;
            }
        }

        /// <summary>
        /// Commits all uncommitted entries.
        /// </summary>
        public void Commit()
        {
            lock (_journaler)
            {
                _journaler.Commit();
                _indexer.Commit();
            }
        }
    }
}
