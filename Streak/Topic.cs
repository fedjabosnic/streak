using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streak.V2.OS;

namespace Streak
{
    public class Streaks
    {
        public void Open(string topic)
        {
            throw new NotImplementedException();
        }
    }

    public class Topic
    {
        public Topic(string topic)
        {
            
        }

        public Reader Reader()
        {
            throw new NotImplementedException();
        }

        public Writer Writer()
        {
            throw new NotImplementedException();
        }
    }

    public class Writer : IDisposable
    {

        public Writer()
        {
            
        }

        public void Append(Entryx entry)
        {
            
        }

        public void Dispose()
        {
        }
    }

    public class Indexed
    {
        private readonly IFile _index;

        private readonly byte[] _buffer;
        private long _offset;

        public long Position { get; private set; }

        public Indexed()
        {
            _buffer = new byte[16];
            _offset = 0L;
        }

        public void Next()
        {
            //_index.Read(_buffer);
            _offset++;

            Position++;
        }
    }

    public class Reader : IDisposable
    {
        private readonly IFile _index;
        private readonly IFile _journal;

        private readonly long _offset;
        private readonly byte[] _buffer;

        public Reader(IFile index, IFile journal)
        {
            _index = index;
            _journal = journal;

            _offset = 0L;
            _buffer = new byte[16];
        }

        public Entryx Next()
        {
            // TODO: Does this always work?

            //_index.Read(_buffer);

            var data = new byte[BitConverter.ToInt64(_buffer, 0) - BitConverter.ToInt64(_buffer, 8)];

           // _journal.Read(data);

            return new Entryx
            {
                Position = _offset,
                Data = data
            };
        }

        public void Dispose()
        {
            _index.Dispose();
            _journal.Dispose();
        }
    }

    public struct Entryx
    {
        public long Position { get; internal set; }
        public byte[] Data { get; internal set; }

        public Entryx(long offset, long length)
        {
            Position = offset / 16;
            Data = new byte[length];
        }
    }

    public class Service
    {
        public Service()
        {
            var topic = new Topic("EURUSD");

            //topic.Seek(Position.Last);
            //topic.Seek(55);

            using (var writer = topic.Writer())
            {
                writer.Optimize(For.Consistency);

                writer.Append(new Entryx());
            }

            using (var reader = topic.Reader())
            {
                foreach (var entry in reader.Read(1, 1000))
                {
                    
                }
            }
        }
    }

    public static class Extensions
    {
        public static Writer Flush(this Writer topic)
        {
            throw new NotImplementedException();
        }

        public static Writer OptimizedFor<T>(this Writer topic) where T : WriteOptimization
        {
            throw new NotImplementedException();
        }

        public static Writer Optimize(this Writer topic, WriteOptimization optimization)
        {
            throw new NotImplementedException();
        }

        public static Reader Optimize(this Reader topic, ReadOptimization optimization)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Entryx> Read(this Reader reader, long from, long to)
        {
            //reader.Seek(from);

            for (var i = from; i <= to; i++)
            {
                yield return reader.Next();
            }
        }
    }

    [Flags]
    public enum Position : long
    {
        First = 1,
        Last = long.MaxValue
    }

    public class WriteOptimization
    {
    }

    /// <summary>
    /// Flushes to disk every append.
    /// </summary>
    public class Consistency : WriteOptimization
    {

    }

    /// <summary>
    /// Flushes to disk every 10 ms or 10 appends.
    /// </summary>
    public class Balanced : WriteOptimization
    {

    }


    /// <summary>
    /// Flushes to disk happen every 10ms.
    /// </summary>
    public class Throughput : WriteOptimization
    {

    }

    public static class For
    {
        public static Consistency Consistency { get; }
        public static Balanced Balance { get; }
        public static Throughput Throughput { get; }
    }

    public class ReadOptimization
    {
    }


}
