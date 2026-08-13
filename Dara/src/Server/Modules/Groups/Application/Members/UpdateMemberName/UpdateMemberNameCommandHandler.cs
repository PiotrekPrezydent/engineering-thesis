using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Groups.Domain.Members;

namespace Dara.Server.Modules.Groups.Application.Members.UpdateMemberName;

public class UpdateMemberNameCommandHandler : ICommandHandler<UpdateMemberNameCommand>
{
    private readonly IMemberRepository _memberRepository;

    public UpdateMemberNameCommandHandler(IMemberRepository memberRepository)
    {
        _memberRepository = memberRepository;
    }

    public async Task HandleAsync(UpdateMemberNameCommand command)
    {
        var member = await _memberRepository.GetByIdAsync(new MemberId(command.MemberId));
        member.UpdateName(command.NewName);
    }
}