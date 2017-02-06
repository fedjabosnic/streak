using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Streak.V2.OS;

namespace Streak.V3.Journal
{
    public class Writer : IDisposable
    {
        private readonly IFileWriter _file;
        public long Position { get; }

        public Writer(IFileWriter file)
        {
            _file = file;
        }

        public void Append(Entry entry)
        {
            // TODO: Append to file

            _file.Write(entry.Data);

        }

        public void Commit()
        {
            // TODO: Commit to disk
        }

        public void Dispose()
        {
        }
    }
}
