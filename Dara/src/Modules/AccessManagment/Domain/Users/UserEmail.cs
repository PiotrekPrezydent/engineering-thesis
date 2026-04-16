using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Users;

public class UserEmail : ValueObject
{
    public string Email { get; }
    
    public UserEmail(string email)
    {
        Email = email;
    }
}