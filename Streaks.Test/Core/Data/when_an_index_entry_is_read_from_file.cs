using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Streaks.Core.Data;
using Streaks.Core.IO;

namespace Streaks.Test.Core.Data
{
    [TestClass]
    public class when_an_index_entry_is_read_from_file
    {
        private Mock<IFileReader> file;
        private long position;
        private byte[] data;

        private IndexEntry entry;

        [TestInitialize]
        public void Setup()
        {
            // NOTE: The current implementation relies on a hardcoded 12 byte index length (8 bytes for the offset and 4 bytes for the position)

            file = new Mock<IFileReader>();
            // Binary data    |           100           |      25     |
            data = new byte[] { 100, 0, 0, 0, 0, 0, 0, 0, 25, 0, 0, 0 };
            // Currently at position 2
            //position = 24;

            file.Setup(x => x.Position).Returns(() => position);
            file.Setup(x => x.Read(It.IsAny<byte[]>(), 48)).Callback<byte[], long>((d, p) => data.CopyTo(d, 0));

            // Read an entry at position 5
            entry = file.Object.ReadIndex(5);
        }

        [TestMethod]
        public void the_correct_number_of_bytes_are_read_from_the_correct_place_in_the_file()
        {
            // The amount of data read should be 12 bytes, 8 for the offset and 4 for the length
            // The expected position of the data should be 12 x (n - 1), ie entry 5 should be located at 48 (12 x 4)
            file.Verify(x => x.Read(It.IsAny<byte[]>(), 48), Times.Once);
        }

        [TestMethod]
        public void the_binary_representation_of_the_offset_is_read_from_the_first_8_bytes()
        {
            // The offset should be 100
            entry.Offset.Should().Be(100);
        }

        [TestMethod]
        public void reads_the_binary_representation_of_the_length_is_read_from_the_last_4_bytes()
        {
            // The length should be 25
            entry.Length.Should().Be(25);
        }
    }
}