using Dara.BuildingBlocks.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Exceptions;

namespace Dara.Modules.AccessManagment.Domain.User;

public class UserNickname : ValueObject
{
    internal string Nickname { get; }
    
    public UserNickname(string nickname)
    {
        if (!IsValidNickname(nickname))
            throw new NotValidNicknameException();
        
        Nickname = nickname;
    }
    
    //add more logic
    bool IsValidNickname(string nickname)
    {
        if(string.IsNullOrWhiteSpace(nickname))
            return false;
        
        return true;
    }
}