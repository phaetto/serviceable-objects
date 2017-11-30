namespace Serviceable.Objects.Streams
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;

    public sealed class SubscriberStream : IDisposable
    {
        private readonly BlockingCollection<object> blockingColection;

        private bool isDisposed;

        public SubscriberStream(BlockingCollection<object> blockingColection = null)
        {
            this.blockingColection = blockingColection ?? new BlockingCollection<object>(new ConcurrentQueue<object>());
        }

        public IEnumerable<object> AsStream()
        {
            return AsStream(CancellationToken.None);
        }

        public IEnumerable<object> AsStream(CancellationToken cancellationToken)
        {
            return AsStream(cancellationToken, null);
        }

        public IEnumerable<TType> AsStream<TType>()
        {
            return AsStream<TType>(CancellationToken.None);
        }

        public IEnumerable<TType> AsStream<TType>(CancellationToken cancellationToken)
        {
            // This is wrong
            return AsStream(cancellationToken, typeof(TType)).Cast<TType>();
        }

        public void PublishToCollection(object action)
        {
            if (isDisposed)
            {
                return;
            }

            blockingColection.Add(action);
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            blockingColection.CompleteAdding();
            blockingColection.Dispose();
        }

        private IEnumerable<object> AsStream(CancellationToken cancellationToken, Type requestedSpecificType)
        {
            IEnumerable<object> enumeration;

            try
            {
                enumeration = blockingColection.GetConsumingEnumerable(cancellationToken);
            }
            catch (ObjectDisposedException)
            {
                yield break;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                yield break;
            }

            var hasAnyMore = false;
            var iterator = enumeration.GetEnumerator();
            do
            {
                try
                {
                    hasAnyMore = iterator.MoveNext();
                }
                catch (ObjectDisposedException)
                {
                    // From BlockingCollection
                    yield break;
                }
                catch (ArgumentNullException)
                {
                    // From mscorlib
                    yield break;
                }
                catch (OperationCanceledException)
                {
                    // From task library
                    yield break;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    yield break;
                }

                var item = iterator.Current;

                if (requestedSpecificType != null && !SupportsType(item.GetType(), requestedSpecificType))
                {
                    continue;
                }

                yield return item;

            } while (hasAnyMore);
        }

        private bool SupportsType(Type itemType, Type requestedType)
        {
            return itemType == requestedType
                   || itemType.GetTypeInfo().IsSubclassOf(requestedType)
                   || itemType.GetTypeInfo().ImplementedInterfaces.Any(x => x.AssemblyQualifiedName == requestedType.AssemblyQualifiedName);
        }
    }
}
