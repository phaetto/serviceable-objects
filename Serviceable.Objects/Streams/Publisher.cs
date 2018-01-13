namespace Serviceable.Objects.Streams
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class Publisher : IDisposable
    {
        private readonly List<PublisherSubscription> subscriptions = new List<PublisherSubscription>();

        public IEnumerable<object> SubscribeToInfiniteStream()
        {
            return SubscribeToInfiniteStream(CancellationToken.None);
        }

        public IEnumerable<object> SubscribeToInfiniteStream(CancellationToken cancellationToken)
        {
            var newSubscription = new PublisherSubscription
            {
                    CancellationToken = cancellationToken,
                    SubscriberStream = new SubscriberStream()
                };

            lock (subscriptions)
            {
                subscriptions.Add(newSubscription);
            }

            return newSubscription.SubscriberStream.AsStream(cancellationToken);
        }


        public IEnumerable<TType> SubscribeToInfiniteStream<TType>()
        {
            return SubscribeToInfiniteStream<TType>(CancellationToken.None);
        }

        public IEnumerable<TType> SubscribeToInfiniteStream<TType>(CancellationToken cancellationToken)
        {
            var newSubscription = new PublisherSubscription
            {
                CancellationToken = cancellationToken,
                SubscriberStream = new SubscriberStream()
            };

            lock (subscriptions)
            {
                subscriptions.Add(newSubscription);
            }

            return newSubscription.SubscriberStream.AsStream<TType>(cancellationToken);
        }

        public void Publish(object action)
        {
            var subscriptionsCancelled = new List<PublisherSubscription>();

            lock (subscriptions)
            {
                foreach (var publisherSubscription in subscriptions)
                {
                    if (!publisherSubscription.CancellationToken.IsCancellationRequested)
                    {
                        publisherSubscription.SubscriberStream.PublishToCollection(action);
                    }
                    else
                    {
                        subscriptionsCancelled.Add(publisherSubscription);
                    }
                }
            }

            lock (subscriptions)
            {
                foreach (var subscriptionCancelled in subscriptionsCancelled)
                {
                    subscriptionCancelled.SubscriberStream.Dispose();
                    subscriptions.Remove(subscriptionCancelled);
                }
            }
        }

        public virtual void Dispose()
        {
            lock (subscriptions)
            {
                foreach (var publisherSubscription in subscriptions)
                {
                    publisherSubscription.SubscriberStream.Dispose();
                }
            }
        }
    }
}
