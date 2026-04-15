using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Exceptions;

namespace Dara.Modules.AccessManagment.Domain.User;

public class UserPassword : ValueObject
{
    public string HashPassword { get; }

    public UserPassword(string hash)
    {
        if (!IsValidPassword(hash))
            throw new NotValidPasswordException();
        
        HashPassword = hash;
    }
    
    //add more logic
    bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
        
        return true;
    }
}