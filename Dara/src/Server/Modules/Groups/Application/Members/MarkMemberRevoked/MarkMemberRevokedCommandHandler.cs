using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Application.Members.MarkMemberRevoked;

public class MarkMemberRevokedCommandHandler : ICommandHandler<MarkMemberRevokedCommand>
{
    private readonly IMemberRepository _memberRepository;

    public MarkMemberRevokedCommandHandler(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task HandleAsync(MarkMemberRevokedCommand command)
    {
        var member = await _memberRepository.GetByIdAsync(new MemberId(command.MemberId));
        member.MarkRevoked();
    }
}
