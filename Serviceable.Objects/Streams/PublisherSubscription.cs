namespace Serviceable.Objects.Streams
{
    using System.Threading;

    public sealed class PublisherSubscription
    {
        public SubscriberStream SubscriberStream { get; set; }

        public CancellationToken CancellationToken { get; set; }
    }
}
