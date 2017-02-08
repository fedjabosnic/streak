using Streak.V2.OS;

namespace Streak.V4
{
    public class Journaler : IJournaler
    {
        private readonly IFileWriter _file;

        public Journaler(IFileWriter file)
        {
            _file = file;
        }

        public FileSegmentInfo Append(byte[] data)
        {
            return _file.Write(data);
        }

        public void Commit()
        {
            _file.Flush();
        }
    }
}