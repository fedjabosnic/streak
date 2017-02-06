using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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

    public class Streak
    {
        private readonly IIndexer _indexer;
        private readonly IJournaler _journaler;
        private readonly ICommitter _committer;

        public Streak(IIndexer indexer, IJournaler journaler, ICommitter committer)
        {
            _indexer = indexer;
            _journaler = journaler;
            _committer = committer;

            _committer.Register(Commit);
        }

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
