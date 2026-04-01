using LADR.SharedKernel.Domain.Models;

namespace LADR.Modules.Hub.Domain;

public class Hub : Entity
{
    public List<Guid> MembersIds { get; private set; }
    
    public HubConfiguration Configuration { get; private set; }
}