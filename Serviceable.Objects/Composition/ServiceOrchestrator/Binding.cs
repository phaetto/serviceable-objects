namespace Serviceable.Objects.Composition.ServiceOrchestrator
{
    using System.Collections.Generic;

    public class Binding : Dictionary<string, string>
    {
        public string Host
        {
            get => this["Host"];
            set => this["Host"] = value;
        }

        public string Port
        {
            get => this["Port"];
            set => this["Port"] = value;
        }
    }
}