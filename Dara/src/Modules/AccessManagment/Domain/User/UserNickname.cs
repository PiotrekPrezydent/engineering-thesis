using Dara.Core.Domain.Business;
using Dara.Modules.AccessManagment.Domain.Exceptions;

namespace Dara.Modules.AccessManagment.Domain.User;

public class UserNickname : ValueObject
{
    internal string Nickname { get; }
    
    internal UserNickname(string nickname)
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