using System.IO.Pipes;
using Serviceable.Objects.Remote;
using Serviceable.Objects.Remote.Serialization.Streaming;

namespace Serviceable.Objects.IO.NamedPipes
{
    public sealed class NamedPipeClientContext: Context<NamedPipeClientContext>
    {
        private readonly string namedPipe;
        private readonly StreamSession streamSession = new StreamSession();

        public void Connect(IReproducible command)
        {
            using (var namedPipeClientStream = new NamedPipeClientStream(".", "testpipe", PipeDirection.InOut))
            {
                namedPipeClientStream.Connect(); // TODO: add timeout

                var specification = command.GetInstanceSpec();
                streamSession.Write(namedPipeClientStream, specification.SerializeToJson());

                namedPipeClientStream.WaitForPipeDrain();

                streamSession.Read(namedPipeClientStream);

                while (streamSession.CommandsTextReadyToBeParsedQueue.TryDequeue(out string commandString))
                {
                    // Deserialize
                }
            }
        }
    }
}
