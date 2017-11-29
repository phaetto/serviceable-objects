namespace Serviceable.Objects.IO.NamedPipes.Client
{
    using System.IO.Pipes;
    using Newtonsoft.Json;
    using Remote;
    using Remote.Serialization.Streaming;

    public sealed class NamedPipeClientContext: Context<NamedPipeClientContext>
    {
        // TODO: make client a real proxy content
        private readonly string namedPipe;

        private readonly int timeoutInMilliseconds;
        private readonly StreamSession streamSession = new StreamSession();

        public NamedPipeClientContext(string namedPipe, int timeoutInMilliseconds)
        {
            this.namedPipe = namedPipe;
            this.timeoutInMilliseconds = timeoutInMilliseconds;
        }

        public object Send(IReproducible command)
        {
            using (var namedPipeClientStream = new NamedPipeClientStream(".", namedPipe, PipeDirection.InOut))
            {
                namedPipeClientStream.Connect(timeoutInMilliseconds);

                var specification = command.GetInstanceSpec();
                streamSession.Write(namedPipeClientStream, specification.SerializeToJson());

                namedPipeClientStream.WaitForPipeDrain();

                streamSession.Read(namedPipeClientStream);

                if (streamSession.CommandsTextReadyToBeParsedQueue.TryDequeue(out var replyString))
                {
                    if (!string.IsNullOrWhiteSpace(replyString))
                    {
                        return JsonConvert.DeserializeObject(replyString);
                    }
                }

                return null;
            }
        }
    }
}
