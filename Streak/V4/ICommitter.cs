using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Streak.V2.OS;
using Buffer = Streak.V2.OS.Buffer;

namespace Streak.V4
{
    public interface ICommitter
    {
        void Register(Action commit);
        void Update();
    }

    public class Committer : ICommitter
    {
        //private readonly List<ICommittable> _committables;

        private readonly List<Action> _committables;
        private readonly int _rate;
        private readonly int _time;

        private int _updates;

        public Committer(int rate, int time)
        {
            _committables = new List<Action>();
            _rate = rate;
            _time = time;
        }

        public void Register(Action commit)
        {
            _committables.Add(commit);
        }

        public void Update()
        {
            _updates++;

            if (_updates < _rate) return;

            foreach (var commit in _committables)
            {
                commit();
            }

            _updates = 0;
        }
    }

    public class ImmediateFileCommitter
    {
        public ImmediateFileCommitter(IFileWriter index, IFileWriter journal)
        {

        }
    }

    public class BufferingCommitter
    {
        private readonly object _lock = new object();
        private StreakFiles _primary = new StreakFiles();
        private StreakFiles _secondary = new StreakFiles();

        public BufferingCommitter()
        {
        }

        public void Append(byte[] index, byte[] journal)
        {
            //_primary.Append(index, journal);
        }

        public void Commit()
        {
            lock (_lock)
            {
                _secondary = Interlocked.Exchange(ref _primary, _secondary);

            }
        }
    }

    internal class StreakFiles
    {
        public Buffer Index { get; set; } = new Buffer();
        public Buffer Journal { get; set; } = new Buffer();
    }

    public interface IWritableStream
    {
        long Position { get; }
        long Length { get; }

        void Append(byte[] data);
        void Append(byte[] data, int offset, int length);
        void Flush();
    }

    public interface IWritableIndex : IWritableStream
    {

    }

    public interface IWritableJournal : IWritableStream
    {

    }

    public interface IWritableStreak
    {
        long Position { get; }

        void Append(Entry entry);
        void Commit();
    }

    public class WritableFile : IWritableStream
    {
        internal FileStream File { get; set; }

        public long Position => File.Position;
        public long Length => File.Length;

        public WritableFile(string path)
        {
            File = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.Read, 1, FileOptions.SequentialScan);
        }

        public void Append(byte[] data)
        {
            File.Write(data, 0, data.Length);
        }

        public void Append(byte[] data, int offset, int length)
        {
            File.Write(data, offset, length);
        }

        public void Flush()
        {
            File.Flush();
        }


    }

    public class WritableIndex : IWritableStream
    {
        internal IWritableStream Stream { get; set; }

        public long Position => Stream.Position;
        public long Length => Stream.Length;

        public WritableIndex(IWritableStream stream)
        {
            Stream = stream;
        }

        public void Append(byte[] data)
        {
            Stream.Append(data);
        }

        public void Append(byte[] data, int offset, int length)
        {
            Stream.Append(data, offset, length);
        }

        public void Flush()
        {
            Stream.Flush();
        }
    }

    public class WritableJournal : IWritableStream
    {
        internal IWritableStream Stream { get; set; }

        public long Position => Stream.Position;
        public long Length => Stream.Length;

        public WritableJournal(IWritableStream stream)
        {
            Stream = stream;
        }

        public void Append(byte[] data)
        {
            Stream.Append(data);
        }

        public void Append(byte[] data, int offset, int length)
        {
            Stream.Append(data, offset, length);
        }

        public void Flush()
        {
            Stream.Flush();
        }
    }

    public interface IStreamCommitter
    {
        void Append(long x, byte[] data);
        void Append(byte[] index, byte[] data);
        void Commit();
    }

    public class StreamCommitter : IStreamCommitter
    {
        internal IWritableStream Index { get; }
        internal IWritableStream Journal { get; }

        public StreamCommitter(IWritableStream index, IWritableStream journal)
        {
            Index = index;
            Journal = journal;
        }

        public void Append(byte[] index, byte[] data)
        {
            Journal.Append(data);
            Index.Append(index);
        }

        byte[] _buffer = new byte[8];

        public unsafe void Append(long x, byte[] data)
        {
            fixed (byte* b = _buffer) *(long*)(b + 0) = x;

            Journal.Append(data);
            Index.Append(_buffer);
        }

        public void Commit()
        {
            Journal.Flush();
            Index.Flush();
        }
    }

    public class BufferedStreamCommitter : IStreamCommitter
    {
        private readonly Buffer _indexBuffer;
        private readonly Buffer _journalBuffer;

        internal IWritableStream Index { get; }
        internal IWritableStream Journal { get; }

        public BufferedStreamCommitter(IWritableStream index, IWritableStream journal)
        {
            _indexBuffer = new Buffer(8388608);
            _journalBuffer = new Buffer(8388608);

            Index = index;
            Journal = journal;
        }

        public void Append(byte[] index, byte[] data)
        {
            _journalBuffer.Add(data);
            _indexBuffer.Add(index);
        }

        byte[] _buffer = new byte[8];

        public unsafe void Append(long x, byte[] data)
        {
            fixed (byte* b = _buffer) *(long*)(b + 0) = x;

            _journalBuffer.Add(data);
            _indexBuffer.Add(_buffer);
        }

        public void Commit()
        {
            Journal.Append(_journalBuffer.Data, 0, _journalBuffer.Length);
            Index.Append(_indexBuffer.Data, 0, _indexBuffer.Length);

            Journal.Flush();
            Index.Flush();

            _journalBuffer.Clear();
            _indexBuffer.Clear();
        }
    }

    internal class BufferThing
    {
        public int writing;
        public int written;
        public Buffer A;
        public Buffer B;
    }

    public class DoubleBufferedStreamCommitter : IStreamCommitter
    {
        private BufferThing _primary;
        private BufferThing _secondary;

        internal IWritableStream Index { get; }
        internal IWritableStream Journal { get; }

        public DoubleBufferedStreamCommitter(IWritableStream index, IWritableStream journal)
        {
            _primary = new BufferThing { A = new Buffer(8388608), B = new Buffer(8388608) };
            _secondary = new BufferThing { A = new Buffer(8388608), B = new Buffer(8388608) };

            Index = index;
            Journal = journal;

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1);
                    Commit();
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void Append(byte[] index, byte[] data)
        {
            var buffer = _primary;

            Interlocked.Increment(ref buffer.writing);
            buffer.A.Add(data);
            buffer.B.Add(index);
            Interlocked.Decrement(ref buffer.writing);
        }

        byte[] _buffer = new byte[8];

        public unsafe void Append(long x, byte[] data)
        {
            var buffer = _primary;

            Interlocked.Increment(ref buffer.writing);

            fixed (byte* b = _buffer) *(long*)(b + 0) = x;
            buffer.A.Add(data);
            buffer.B.Add(_buffer);

            Interlocked.Decrement(ref buffer.writing);
        }

        public void Commit()
        {
            lock (_buffer)
            {
                //Stopwatch sw = new Stopwatch();

                //sw.Start();

                _secondary = Interlocked.Exchange(ref _primary, _secondary);

                var waited = 0;
                while (Thread.VolatileRead(ref _secondary.writing) > 0)
                {
                    // Wait until any currently running writer finishes
                    waited++;
                }

               // var amount = _secondary.A.Length;
                Journal.Append(_secondary.A.Data, 0, _secondary.A.Length);
                Index.Append(_secondary.B.Data, 0, _secondary.B.Length);

                Journal.Flush();
                Index.Flush();

                _secondary.A.Clear();
                _secondary.B.Clear();

                //sw.Stop();

                //if (amount != 0) Console.WriteLine($"Commit time: {((double) sw.ElapsedTicks / Stopwatch.Frequency) * 1000000}ms ({amount})");
                //if (waited != 0) Console.WriteLine($"     waited: {waited} iterations");
            }
        }
    }

    public class Appender
    {
        internal IStreamCommitter Committer { get; }

        public Appender(IStreamCommitter committer)
        {
            Committer = committer;
        }

        public unsafe void Append(Entry entry)
        {
            //var index = new byte[16];

            //fixed (byte* b = index) *(long*)(b + 0) = 15;
            //fixed (byte* b = index) *(long*)(b + 8) = 474545;

            Committer.Append(15, entry.Data);
        }

        public void Commit()
        {
            Committer.Commit();
        }
    }
}