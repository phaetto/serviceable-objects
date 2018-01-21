namespace Serviceable.Objects.Composition.Graph.Stages.Initialization
{
    using System.Threading;

    public interface IInitializeStageFactoryWithDeinitSynchronization : IInitializeStageFactory
    {
        ReaderWriterLockSlim ReaderWriterLockSlim { get; set; }
    }
}
