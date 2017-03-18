using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Streaks.Core.Data;
using Streaks.Core.IO;

namespace Streaks.Test.Core.Data
{
    [TestClass]
    public class when_a_log_entry_is_written_to_file
    {
        private Mock<IFileWriter> file;
        private LogEntry entry;

        private byte[] data;

        [TestInitialize]
        public void Setup()
        {
            file = new Mock<IFileWriter>();
            entry = new LogEntry { Data = new byte[] { 1, 2, 3, 4, 5 } };

            file.Setup(x => x.Write(It.IsAny<byte[]>())).Callback<byte[]>(d => data = d);

            // Write the entry to the file
            file.Object.Write(entry);
        }

        [TestMethod]
        public void the_correct_number_of_bytes_are_written_to_the_file()
        {
            // 5 bytes should have been written to the file
            file.Verify(x => x.Write(It.IsAny<byte[]>()), Times.Once);
        }

        [TestMethod]
        public void the_binary_data_is_written_to_the_file()
        {
            // The written data should match what was in the log index data
            data.Should().Equal(new byte[] { 1, 2, 3, 4, 5 });
        }
    }
}