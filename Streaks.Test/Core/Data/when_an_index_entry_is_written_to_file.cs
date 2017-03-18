using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Streaks.Core.Data;
using Streaks.Core.IO;

namespace Streaks.Test.Core.Data
{
    [TestClass]
    public class when_an_index_entry_is_written_to_file
    {
        private Mock<IFileWriter> file;
        private IndexEntry entry;

        private byte[] data;

        [TestInitialize]
        public void Setup()
        {
            // NOTE: The current implementation relies on a hardcoded 12 byte index length (8 bytes for the offset and 4 bytes for the position)

            file = new Mock<IFileWriter>();
            entry = new IndexEntry { Offset = 100, Length = 25 };

            file.Setup(x => x.Write(It.IsAny<byte[]>())).Callback<byte[]>(d => data = d);

            // Write the entry to the file
            file.Object.Write(entry);
        }

        [TestMethod]
        public void the_correct_number_of_bytes_are_written_to_the_file()
        {
            // 12 bytes should have been written to the file
            file.Verify(x => x.Write(It.IsAny<byte[]>()), Times.Once);
        }

        [TestMethod]
        public void the_binary_representation_of_the_offset_is_written_to_the_first_8_bytes()
        {
            // The first 8 bytes should be the binary representation of the offset (100)
            data.Skip(0).Take(8).Should().Equal(new byte[] { 100, 0, 0, 0, 0, 0, 0, 0 });
        }

        [TestMethod]
        public void the_binary_representation_of_the_length_is_written_to_the_last_4_bytes()
        {
            // The last 4 bytes should be the binary representation of the length (25)
            data.Skip(8).Take(4).Should().Equal(new byte[] { 25, 0, 0, 0 });
        }
    }
}