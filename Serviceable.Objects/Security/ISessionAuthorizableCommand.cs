namespace Serviceable.Objects.Security
{
    public interface ISessionAuthorizableCommand
    {
        string Session { get; set; }
    }
}
