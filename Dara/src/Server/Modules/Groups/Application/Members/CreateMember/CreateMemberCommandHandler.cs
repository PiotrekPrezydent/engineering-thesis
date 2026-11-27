using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Application.Members.CreateMember;

public class CreateMemberCommandHandler : ICommandHandler<CreateMemberCommand>
{
    private readonly IMemberRepository _memberRepository;

    public CreateMemberCommandHandler(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task HandleAsync(CreateMemberCommand command)
    {
        var member = Member.Create(
            new MemberId(command.MemberId),
            command.UserId,
            command.Name);
        await _memberRepository.AddAsync(member);
    }
}
