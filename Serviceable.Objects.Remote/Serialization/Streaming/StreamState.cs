using System.Collections.Concurrent;

namespace Serviceable.Objects.Remote.Serialization.Streaming
{
    public sealed class StreamState
    {
        public bool HasBegunParsingCommand;
        public ConcurrentQueue<string> CommandsTextReadyToBeParsedQueue = new ConcurrentQueue<string>();
        public string ParsedCommandBuffer = string.Empty;
    }
}
