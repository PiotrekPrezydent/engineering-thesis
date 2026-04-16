using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Auth.Exceptions;

namespace Dara.Modules.AccessManagment.Domain.Users;

public class UserPassword : ValueObject
{
    public string HashedPassword { get; }

    public UserPassword(string hashed)
    {
        if (!IsValidPassword(hashed))
            throw new WrongPasswordException();
        
        //maybe check if password is hashed 
        
        HashedPassword = hashed;
    }
    
    //add more logic
    bool IsValidPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return false;
        
        return true;
    }
    

}