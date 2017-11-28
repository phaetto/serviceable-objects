namespace Serviceable.Objects.Remote.Dependencies
{
    public sealed class DependencyInjectionRegistration
    {
        public string Type { get; set; }

        public bool WithDefaultInterface { get; set; }

        public string WithInstanceType { get; set; }
    }
}
