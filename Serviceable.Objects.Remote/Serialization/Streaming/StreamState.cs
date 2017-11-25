using System.Collections.Generic;

namespace Serviceable.Objects.Remote.Serialization.Streaming
{
    public sealed class StreamState
    {
        public bool HasBegunParsingCommand;
        public List<string> CommandsTextReadyToBeParsed = new List<string>(5);
        public string ParsedCommandBuffer = string.Empty;
    }
}
