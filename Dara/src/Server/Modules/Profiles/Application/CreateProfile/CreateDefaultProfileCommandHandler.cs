using Dara.Server.BuildingBlocks.Application.Commands;
using Dara.Server.Modules.Profiles.Domain;

namespace Dara.Server.Modules.Profiles.Application.CreateProfile;

public class CreateDefaultProfileCommandHandler : ICommandHandler<CreateDefaultProfileCommand>
{
    private readonly IProfileRepository _profileRepository;

    public CreateDefaultProfileCommandHandler(IProfileRepository profileRepository)
    {
        _profileRepository = profileRepository;
    }

    public async Task HandleAsync(CreateDefaultProfileCommand command)
    {
        Console.WriteLine("CREATE PRO");
        var profile = Profile.CreateDefault(new(command.ProfileId));
        await _profileRepository.AddAsync(profile);
    }
}