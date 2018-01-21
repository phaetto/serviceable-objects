namespace Serviceable.Objects.Composition.Graph.Stages.Initialization
{
    using System.Threading;

    public interface IInitializationStageSynchronization
    {
        ReaderWriterLockSlim ReaderWriterLockSlim { get; set; }
    }
}
