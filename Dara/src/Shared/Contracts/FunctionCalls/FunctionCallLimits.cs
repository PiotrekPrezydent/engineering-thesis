namespace Dara.Shared.Contracts.FunctionCalls;

public static class FunctionCallLimits
{
    // 256 KB of json
    public const int MaxResultJsonLength = 262144;
    public const int MaxErrorLength = 2000;
}
