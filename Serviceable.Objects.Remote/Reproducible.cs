namespace Serviceable.Objects.Remote
{
    using Serialization;

    public abstract class Reproducible : IReproducible
    {
        public virtual CommandSpecification GetInstanceSpec()
        {
            var commandSpecificationService = new CommandSpecificationService();
            return commandSpecificationService.CreateSpecificationForCommand(this);
        }
    }
}