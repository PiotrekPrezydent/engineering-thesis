using Dara.Shared.Testing.Plugins;

namespace Dara.Shared.Testing.Tests;

public static class CommonFunctionDatas
{
    public static FunctionData GetStockPrice => new FunctionData
    {
        Name = "GetStockPrice",
        Description = "Gets the current stock price for a ticker",
        Parameters = new List<ParameterData>
        {
            new() { Name = "ticker", Type = "string" }
        },
        ReturnType = "string"
    };

    public static FunctionData GetWeather => new FunctionData
    {
        Name = "GetWeather",
        Description = "Gets the current weather",
        Parameters = new List<ParameterData>
        {
            new() { Name = "city", Type = "string" }
        },
        ReturnType = "string"
    };

    public static FunctionData SaveFile => new FunctionData
    {
        Name = "SaveFile",
        Description = "Save content as file",
        Parameters = new List<ParameterData>()
        {
            new() { Name = "path", Type = "string" },
            new() { Name = "content", Type = "string" }
        },
    };

    public static FunctionData CameraCapture => new FunctionData
    {
        Name = "CameraCapture",
        Description = "Takes photo using camera and return content of created picture",
        Parameters = new List<ParameterData>(),
        ReturnType = "string"
    };

    public static FunctionData PhotosDirectory => new FunctionData
    {
        Name = "PhotosDirectory",
        Description = "Gets path to directory for storing photos",
        Parameters = new List<ParameterData>(),
        ReturnType = "string"
    };

}