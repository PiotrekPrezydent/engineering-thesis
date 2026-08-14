using System.Text;
using Dara.Server.Modules.Groups.Application;
using Dara.Server.Modules.Groups.Application.Groups.GetAvaibleGroups;
using Dara.Server.Modules.Groups.Application.Groups.GetGroupDetails;
using Dara.Server.Modules.Identity.Application;
using Dara.Server.Modules.Identity.Application.GetUser;
using Dara.Server.Modules.Plugins.Application;
using Dara.Server.Modules.Plugins.Application.GetPlugins;
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
    
    public LogModulesDataService(ILogger<LogModulesDataService> logger, IIdentityModule identityModule, IProfilesModule profilesModule, IGroupsModule groupsModule, IPluginsModule pluginsModule)
    {
        _logger = logger;
        _identityModule = identityModule;
        _profilesModule = profilesModule;
        _groupsModule = groupsModule;
        _pluginsModule = pluginsModule;
    }
    
    
    public async Task LogDataAsync()
    {
        StringBuilder sb = new();
        Dictionary<Guid, (string Identifier, string ProfileName)> userDetails = new();
        var users = await _identityModule.ExecuteQueryAsync<GetAllUsersQuery, List<(Guid userId, string userIdentifier)>>(new());
        
        sb.AppendLine("\nUSERS-PROFILES :::: \n");
        foreach (var user in users)
        {
            var profile = await _profilesModule.ExecuteQueryAsync<GetProfileQuery,ProfileDto>(new GetProfileQuery(user.userId));
            var plugins = await _pluginsModule.ExecuteQueryAsync<GetPluginsQuery,List<PluginDto>>(new GetPluginsQuery(user.userId));
            userDetails.Add(user.userId, ("IDENTIFIER: " + user.userIdentifier, "PROFILE-NAME: " + profile.Name));
            
            sb.AppendLine($"ID : {user.userId}");
            sb.AppendLine($"IDENTIFIER : {user.userIdentifier}");
            sb.AppendLine($"PROFILE-NAME : {profile.Name}");
            sb.AppendLine($"PLUGINS : {plugins.Count}");
            foreach (var plugin in plugins)
            {
                sb.AppendLine();
                sb.AppendLine($"PLUGIN NAME : {plugin.PluginData.Name} DESCRIPTION : {plugin.PluginData.Description} FUNCTIONS : {plugin.Functions.Count}");
                foreach (var function in plugin.Functions)
                {
                    sb.AppendLine($"\tFUNCTION NAME: {function.FunctionData.Name} --- DESCRIPTION: {function.FunctionData.Description} --- RETURN TYPE: {function.FunctionData.ReturnTypeName}");
                    foreach (var parameter in function.Parameters)
                    {
                        sb.AppendLine(
                            $"\t\tPARAMETER NAME: {parameter.ParameterData.Name} --- DESCRIPTION: {parameter.ParameterData.Description} --- TYPE: {parameter.ParameterData.TypeName}");
                    }
                    sb.AppendLine();
                }

                sb.AppendLine();
            }
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