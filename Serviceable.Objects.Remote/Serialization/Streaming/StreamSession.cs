namespace Serviceable.Objects.Remote.Serialization.Streaming
{
    using System;
    using System.Collections.Concurrent;
    using System.IO;
    using System.Linq;
    using System.Text;
    using Newtonsoft.Json;

    public sealed class StreamSession
    {
        private readonly int bufferSize;
        private readonly byte[] buffer;
        private const string StartOfStream = "SOPv1";
        private const string EndOfStream = "\n";
        private readonly StreamState streamState = new StreamState();

        public bool IsCommandBufferWaitingForCompletion => streamState.HasBegunParsingCommand;

        public ConcurrentQueue<string> CommandsTextReadyToBeParsedQueue => streamState.CommandsTextReadyToBeParsedQueue;

        public StreamSession(int bufferSize = 256)
        {
            this.bufferSize = bufferSize;
            buffer = new byte[bufferSize];
        }

        public bool Read(Stream stream)
        {
            var bytesRead = stream.Read(buffer, 0, bufferSize);
            if (bytesRead == 0)
            {
                return false;
            }

            var stringRead = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            if (stringRead.IndexOf(EndOfStream, StringComparison.Ordinal) == -1)
            {
                // No command string was completed
                streamState.HasBegunParsingCommand = true;
                streamState.ParsedCommandBuffer += stringRead;
                return true;
            }

            // One command completed
            var stringChuncks = stringRead.Split(new[] { EndOfStream }, StringSplitOptions.None);

            if (stringChuncks.Length == 0)
            {
                return true;
            }

            var startingIndex = 0;
            if (streamState.HasBegunParsingCommand)
            {
                streamState.HasBegunParsingCommand = false;
                streamState.ParsedCommandBuffer += stringChuncks[0];
                if (!VerifyProtocolHeader(streamState.ParsedCommandBuffer))
                {
                    // Replace with protocol error
                    WriteException(new InvalidOperationException("Protocol error. The format was not correct."));
                }
                else
                {
                    streamState.CommandsTextReadyToBeParsedQueue.Enqueue(streamState.ParsedCommandBuffer.Substring(StartOfStream.Length));
                }
                streamState.ParsedCommandBuffer = string.Empty;
                startingIndex = 1;
            }

            // Check if there are intermidiate commands
            for (var i = startingIndex; i < stringChuncks.Length - 1; ++i)
            {
                if (string.IsNullOrWhiteSpace(stringChuncks[i]))
                {
                    continue;
                }

                streamState.ParsedCommandBuffer = string.Empty;
                if (!VerifyProtocolHeader(stringChuncks[i]))
                {
                    // Replace with protocol error
                    WriteException(new InvalidOperationException("Protocol error. The format was not correct."));
                }
                else
                {
                    streamState.CommandsTextReadyToBeParsedQueue.Enqueue(stringChuncks[i].Substring(StartOfStream.Length));
                }
            }
            
            // Add the last string left to the buffer:
            if (!string.IsNullOrWhiteSpace(stringChuncks.Last()))
            {
                streamState.ParsedCommandBuffer = stringChuncks.Last();
                if (VerifyProtocolHeader(streamState.ParsedCommandBuffer))
                {
                    var isFinalisedCommand = stringRead.LastIndexOf(EndOfStream, StringComparison.Ordinal) == stringRead.Length - EndOfStream.Length;
                    streamState.HasBegunParsingCommand = !isFinalisedCommand;
                    if (isFinalisedCommand)
                    {
                        streamState.CommandsTextReadyToBeParsedQueue.Enqueue(streamState.ParsedCommandBuffer.Substring(StartOfStream.Length));
                        streamState.ParsedCommandBuffer = string.Empty;
                    }
                }
            }

            return true;
        }

        public void Write(Stream stream, string data)
        {
            var bytes = Encoding.ASCII.GetBytes(StartOfStream + data + EndOfStream);
            stream.Write(bytes, 0, bytes.Length);
        }

        private bool VerifyProtocolHeader(string commandBuffer)
        {
            return commandBuffer.StartsWith(StartOfStream);
        }

        private void WriteException(Exception exception)
        {
            streamState.HasBegunParsingCommand = false;
            streamState.ParsedCommandBuffer = string.Empty;

            streamState.CommandsTextReadyToBeParsedQueue.Enqueue(JsonConvert.SerializeObject(new CommandResultSpecification
            {
                ResultDataAsJson = JsonConvert.SerializeObject(exception),
                CommandType = null,
                ContainsError = true,
            }));
        }
    }
}
