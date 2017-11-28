namespace Serviceable.Objects.Remote.Dependencies
{
    using System.Collections.Generic;

    public sealed class DependencyInjectionRegistrationTemplate
    {
        public IEnumerable<DependencyInjectionRegistration> Registrations { get; set; }
    }
}
