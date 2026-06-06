namespace Dara.BuildingBlocks.Infrastructure;

public static class LogsIdsRanges
{
    public const int ServerStart = 10000;
    public const int ServerEnd = 19999;
    
    public const int CoreStart = 20000;
    public const int CoreEnd = 29999;
    
    public const int ModulesStart = 30000;
    public const int ModulesEnd = 999999;
}

public enum LogCategory
{
    DOTNET = 0,
    SERVER = 1,
    CORE = 2,
    MODULE = 3,
    NONE = 99999,
}
