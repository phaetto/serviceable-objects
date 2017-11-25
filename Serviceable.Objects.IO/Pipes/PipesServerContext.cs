using System.Linq;
using Newtonsoft.Json;
using Serviceable.Objects.Composition.Events;
using Serviceable.Objects.Remote.Serialization;

namespace Serviceable.Objects.IO.Pipes
{
    public sealed class PipesServerContext : Context<PipesServerContext>
    {
        private string PushDataAsCommand(string data)
        {
            var spec = DeserializableSpecification<ExecutableCommandSpecification>.DeserializeFromJson(data);
            var command = spec.CreateFromSpec();

            var eventResults =
                PublishCommandEventAndGetResults(new GraphFlowEventPushControlApplyCommandInsteadOfEvent(command))
                    .Where(x => x.ResultObject != null).ToList();

            if (eventResults.Count > 0)
            {
                var results = eventResults.Select(x => x.ResultObject);
                return JsonConvert.SerializeObject(results);
            }

            return null;
        }
    }
}
