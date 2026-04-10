namespace Dara.Modules.AccessManagment.Domain.Auth;

public interface IPasswordHasher
{
    public string HashPassword(string password);
    
    public bool VerifyHashedPassword(string hashedPassword, string providedPassword);
}