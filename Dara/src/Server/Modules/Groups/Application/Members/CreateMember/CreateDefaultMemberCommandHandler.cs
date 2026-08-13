using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Application.Members.CreateMember;

public class CreateDefaultMemberCommandHandler : ICommandHandler<CreateDefaultMemberCommand>
{
    private readonly IMemberRepository _memberRepository;

    public CreateDefaultMemberCommandHandler(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task HandleAsync(CreateDefaultMemberCommand command)
    {
        var member = Member.CreateDefault(new(command.MemberId));
        await _memberRepository.AddAsync(member);
    }
}