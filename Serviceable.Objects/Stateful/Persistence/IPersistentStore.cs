namespace Serviceable.Objects.Stateful.Persistence
{
    using System;

    public interface IPersistentStore<T>
    {
        T New();
        bool Exists(T data);
        DateTimeOffset? GetLastSavedTime(T data);
        T Load(T data);
        void Save(T data);
    }
}