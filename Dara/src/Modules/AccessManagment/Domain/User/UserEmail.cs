using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Exceptions;

namespace Dara.Modules.AccessManagment.Domain.User;

public class UserEmail : ValueObject
{
    internal string Email { get; }
    
    public UserEmail(string email)
    {
        if (!IsValidEmail(email))
            throw new NotValidEmailException();
        
        Email = email;
    }

    //add more logic
    bool IsValidEmail(string email)
    {
        if(string.IsNullOrWhiteSpace(email))
            return false;
        
        return true;
    }
}