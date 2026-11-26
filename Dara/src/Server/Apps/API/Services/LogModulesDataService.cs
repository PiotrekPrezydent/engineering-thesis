using System.Text;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.GetAvaibleGroups;
using Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Orchestration.Application;
using Dara.Server.Modules.Orchestration.Application.Groups.GetGroupAgentSettings;
using Dara.Server.Modules.Orchestration.Application.Participants.GetParticipantFunctions;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Application.Plugins.GetOwnerPlugins;
using Dara.Server.Modules.Profiles.Application;
using Dara.Server.Modules.Profiles.Application.GetProfile;

namespace Dara.Server.Apps.API.Services;

public class LogModulesDataService
{
    private readonly ILogger<LogModulesDataService> _logger;
    private readonly IIdentityModule _identityModule;
    private readonly IProfilesModule _profilesModule;
    private readonly IGroupsModule _groupsModule;
    private readonly IPluginsModule _pluginsModule;
    private readonly IOrchestrationModule _orchestrationModule;

    public LogModulesDataService(ILogger<LogModulesDataService> logger, IIdentityModule identityModule, IProfilesModule profilesModule, IGroupsModule groupsModule, IPluginsModule pluginsModule, IOrchestrationModule orchestrationModule)
    {
        _logger = logger;
        _identityModule = identityModule;
        _profilesModule = profilesModule;
        _groupsModule = groupsModule;
        _pluginsModule = pluginsModule;
        _orchestrationModule = orchestrationModule;
    }
    
    
    public async Task LogDataAsync()
    {
        StringBuilder sb = new();
        Dictionary<Guid, (string Identifier, string ProfileName)> userDetails = new();
        var users = await _identityModule.ExecuteQueryAsync<GetAllUsersQuery, List<(Guid userId, string login)>>(new());
        
        sb.AppendLine("\nUSERS-PROFILES :::: \n");
        foreach (var user in users)
        {
            var profile = await _profilesModule.ExecuteQueryAsync<GetProfileQuery,ProfileDto>(new GetProfileQuery(user.userId));
            var plugins = await _pluginsModule.ExecuteQueryAsync<GetOwnerPluginsQuery,List<OwnerPluginDto>>(new GetOwnerPluginsQuery(user.userId));
            var functions = await _orchestrationModule.ExecuteQueryAsync<GetParticipantFunctionsQuery,List<ParticipantFunctionDto>>(new GetParticipantFunctionsQuery(user.userId));
            userDetails.Add(user.userId, ("LOGIN: " + user.login, "PROFILE-NAME: " + profile.Name));
            
            sb.AppendLine($"ID : {user.userId}");
            sb.AppendLine($"LOGIN : {user.login}");
            sb.AppendLine($"PROFILE-NAME : {profile.Name}");
            sb.AppendLine($"PLUGINS : {plugins.Count}");
            foreach (var plugin in plugins)
            {
                sb.AppendLine();
                sb.AppendLine($"PLUGIN NAME : {plugin.Name} DESCRIPTION : {plugin.Description} ENABLED : {plugin.IsEnabled} FUNCTIONS : {plugin.Functions.Count}");
                foreach (var function in plugin.Functions)
                {
                    sb.AppendLine($"\tFUNCTION NAME: {function.Description.Name} --- DESCRIPTION: {function.Description.Description} --- RETURN TYPE: {function.Description.Returns?.Kind.ToString() ?? "void"} --- ENABLED: {function.IsEnabled}");
                    foreach (var parameter in function.Description.Parameters)
                    {
                        sb.AppendLine(
                            $"\t\tPARAMETER NAME: {parameter.Name} --- DESCRIPTION: {parameter.Type.Description} --- TYPE: {parameter.Type.Kind}");
                    }
                    sb.AppendLine();
                }

                sb.AppendLine();
            }
            sb.AppendLine($"PARTICIPANT FUNCTIONS : {string.Join(", ", functions.Select(f => f.Name))}");
            sb.AppendLine();
            sb.AppendLine();
        }
        _logger.LogInformation(sb.ToString());
        sb.Clear();
        
        var groups = await _groupsModule.ExecuteQueryAsync<GetAvaibleGroupsQuery, List<AvaibleGroupDto>>(new());
        sb.AppendLine("\nGROUPS :::: \n");
        foreach (var group in groups)
        {
            var details =
                await _groupsModule.ExecuteQueryAsync<GetGroupDetailsQuery, GroupDetailsDto>(new(group.GroupId));
            
            sb.AppendLine($"ID : {details.GroupId}");
            sb.AppendLine($"OWNER : {details.OwnerId} --- {userDetails[details.OwnerId]}");
            sb.AppendLine($"NAME : {details.GroupName}");
            sb.AppendLine($"JOIN CODE : {details.JoinCode}");
            sb.AppendLine($"MEMBERS : {details.Members.Count}");
            var settings = await _orchestrationModule.ExecuteQueryAsync<GetGroupAgentSettingsQuery, GroupAgentSettingsDto?>(new GetGroupAgentSettingsQuery(details.GroupId, details.OwnerId));
            if (settings is not null)
                sb.AppendLine($"AGENT : PREFIX {settings.PromptMessagePrefix} --- PROVIDER {settings.ChatClientProviderName} --- MODEL {settings.ClientModelName} --- API KEY {(string.IsNullOrEmpty(settings.ApiKey) ? "empty" : "set")}");
            foreach (var member in details.Members)
            {
                sb.AppendLine($"\t{member} --- {userDetails[member]}");
            }
            sb.AppendLine();
            sb.AppendLine();
        }
        _logger.LogInformation(sb.ToString());
        
    }
}