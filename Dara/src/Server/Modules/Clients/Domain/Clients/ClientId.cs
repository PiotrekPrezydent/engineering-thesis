namespace Dara.Server.Modules.Clients.Domain.Clients;

public class ClientId : IdOfType<Guid>
{
    public ClientId(Guid value) : base(value)
    {
    }
}