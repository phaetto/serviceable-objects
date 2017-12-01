namespace Serviceable.Objects.Remote.Dependencies
{
    using Newtonsoft.Json;
    using Objects.Dependencies;

    public static class ContainerExtensions
    {
        public static void FromJson(this Container container, string json)
        {
            var dependencyInjectionRegistrationTemplate = JsonConvert.DeserializeObject<DependencyInjectionRegistrationTemplate>(json);

            From(container, dependencyInjectionRegistrationTemplate);
        }

        public static void From(this Container container, DependencyInjectionRegistrationTemplate dependencyInjectionRegistrationTemplate)
        {
            if (dependencyInjectionRegistrationTemplate.Registrations == null)
            {
                return;
            }

            foreach (var dependencyInjectionRegistration in dependencyInjectionRegistrationTemplate.Registrations)
            {
                if (dependencyInjectionRegistration.WithDefaultInterface)
                {
                    container.RegisterWithDefaultInterface(Types.FindType(dependencyInjectionRegistration.Type));
                }
                else if (!string.IsNullOrWhiteSpace(dependencyInjectionRegistration.WithInstanceType))
                {
                    container.Register(Types.FindType(dependencyInjectionRegistration.Type), container.CreateObject(Types.FindType(dependencyInjectionRegistration.Type)));
                }
                else
                {
                    container.Register(Types.FindType(dependencyInjectionRegistration.Type), container.CreateObject(Types.FindType(dependencyInjectionRegistration.WithInstanceType)));
                }
            }
        }
    }
}