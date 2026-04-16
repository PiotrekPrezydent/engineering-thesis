using Dara.BuildingBlocks.Domain.Business;

namespace Dara.Modules.AccessManagment.Domain.Users;

public class UserNickname : ValueObject
{
    public string Nickname { get; }

    public UserNickname(string nickname)
    {
        Nickname = nickname;
    }
}