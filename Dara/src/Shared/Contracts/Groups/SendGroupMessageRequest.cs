namespace Dara.Shared.Contracts.Groups;

// a prompt starts with the prefix of the group, e.g. "/prompt take a photo"
public record SendGroupMessageRequest(string Content);
