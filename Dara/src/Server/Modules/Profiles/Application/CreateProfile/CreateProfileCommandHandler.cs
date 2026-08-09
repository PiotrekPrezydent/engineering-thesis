using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Profiles.Domain;

namespace Dara.Server.Modules.Profiles.Application.CreateProfile;

public class CreateProfileCommandHandler : ICommandHandler<CreateProfileCommand>
{
    private readonly IProfileRepository _profileRepository;

    public CreateProfileCommandHandler(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task HandleAsync(CreateProfileCommand command)
    {
        var profileId = new ProfileId(command.ProfileId);
        await _profileRepository.AddAsync(Profile.Create(profileId,command.Name));
    }
}