using System.IO;
using System.Text;
using Serviceable.Objects.Remote.Serialization.Streaming;
using Xunit;

namespace Serviceable.Objects.Remote.Tests
{
    public class StreamSessionTests
    {
        [Fact]
        public void Read_WhenStreamSessionReadsProtocolText_ThenItCorrectlyParsesTheInput()
        {
            const string streamString = "SOPv11\nSOPv12\nSOPv13\nSOPv1\\n\n\n\nSOPv14\n";
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(streamString));

            var streamSession = new StreamSession();
            while (streamSession.Read(memoryStream));

            Assert.Equal(5, streamSession.CommandsTextReadyToBeParsedQueue.Count);
        }

        [Fact]
        public void Read_WhenStreamSessionReadsProtocolTextInChunks_ThenItCorrectlyParsesTheInput()
        {
            const string streamString = "SOPv11\nSOPv12\nSOPv13\nSOPv1\\n\n\n\nSOPv14\n";
            var memoryStream = new MemoryStream(Encoding.ASCII.GetBytes(streamString));

            var streamSession = new StreamSession(2);
            while(streamSession.Read(memoryStream));

            Assert.Equal(5, streamSession.CommandsTextReadyToBeParsedQueue.Count);
        }
    }
}
