using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Streaks.Core.Data;
using Streaks.Core.IO;

namespace Streaks.Test.Core.Data
{
    [TestClass]
    public class when_a_log_entry_is_read_from_file
    {
        private Mock<IFileReader> file;
        private long position;
        private byte[] data;

        private LogEntry entry;

        [TestInitialize]
        public void Setup()
        {
            file = new Mock<IFileReader>();
            data = new byte[] { 1, 2, 3, 4, 5 };

            file.Setup(x => x.Read(It.IsAny<byte[]>(), 60)).Callback<byte[], long>((d, p) => data.CopyTo(d, 0));

            // Read an entry at position 5
            entry = file.Object.ReadLog(new IndexEntry { Offset = 60, Length = 5 });
        }

        [TestMethod]
        public void the_correct_number_of_bytes_are_read_from_the_correct_place_in_the_file()
        {
            // The amount of data read should be 5 bytes, ie the length specified in the index
            // The expected position of the data should be 60, ie, the offset specified in the index
            file.Verify(x => x.Read(It.Is<byte[]>(d => d.Length == 5), 60), Times.Once);
        }

        [TestMethod]
        public void the_data_should_be_as_expected()
        {
            // The offset should be 100
            entry.Data.Should().Equal(new byte[] { 1, 2, 3, 4, 5 });
        }
    }
}