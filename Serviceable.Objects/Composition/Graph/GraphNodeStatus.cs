namespace Serviceable.Objects.Composition.Graph
{
    public enum GraphNodeStatus
    {
        Unconfigured = 0,
        Configuring,
        Configured,
        SetupStarted,
        SetupFinished,
        Initializing,
        Initialized,
        Deinitializing,
        DismantlingSetup,
        Deconfiguring
    }
}