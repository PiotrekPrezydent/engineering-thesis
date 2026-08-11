using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Profiles.Domain;

namespace Dara.Server.Modules.Profiles.Application.ChangeProfileName;

public class ChangeProfileNameCommandHandler : ICommandHandler<ChangeProfileNameCommand>
{
    private readonly IProfileRepository _profileRepository;

    public ChangeProfileNameCommandHandler(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }


    public async Task HandleAsync(ChangeProfileNameCommand command)
    {
        var profile = await _profileRepository.GetByIdAsync(new ProfileId(command.ProfileId));
        profile.UpdateName(command.NewName);
    }
}