using Dara.Modules.AccessManagment.Domain.Auth;

namespace Dara.Modules.AccessManagment.Infrastructure;

public class MockPasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return password.ToUpper() + "HASHED";
    }

    public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
    {
        if(HashPassword(providedPassword) == hashedPassword)
            return true;
        
        return false;
    }
}