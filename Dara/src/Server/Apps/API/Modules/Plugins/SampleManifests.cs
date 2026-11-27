using System.Security.Cryptography;
using System.Text;
using Dara.Shared.Contracts.Functions;
using Dara.Shared.Contracts.Plugins;

namespace Dara.Server.Apps.API.Modules.Plugins;

// sample plugins of seeded clients as a client registers them, the hash stands in for the hash of a plugin .dll
public static class SampleManifests
{
    // client name -> the full set of plugins the client registers
    public static IReadOnlyDictionary<string, IReadOnlyList<PluginManifest>> ByClient { get; } = new Dictionary<string, IReadOnlyList<PluginManifest>>
    {
        ["Bob-Phone"] = [Camera(), FileManagement(), Display(), Notifications(), Location()],
        ["Bob-Laptop"] = [Camera(), FileManagement(), Display(), Notifications(), Location(), SystemControl()],
        ["Bob-Smartwatch"] = [Display(), Notifications(), Location()],
        ["Room-101-Computer-01"] = [Display(), Notifications(), SystemControl()],
        ["Room-101-Computer-02"] = [Display(), Notifications(), SystemControl()]
    };

    public static PluginManifest Camera()
    {
        return Manifest("Camera plugin", "Device camera capabilities",
            new FunctionDescription("CameraCapture", "Takes photo using device camera and return content of created picture", [],
                Text("Content of the picture", contentEncoding: "base64")));
    }

    public static PluginManifest FileManagement()
    {
        return Manifest("File management plugin", "File and directory management capabilities",
            new FunctionDescription("SaveFile", "Saves content as a file",
            [
                new PropertyDescription("path", Text("Path where the file will be saved"), true),
                new PropertyDescription("content", Text("Content that will be saved"), true),
                new PropertyDescription("overwrite", new TypeDescription(TypeKind.Boolean, Description: "Whether an existing file is replaced", Default: "false"), false)
            ], null),
            new FunctionDescription("ReadFile", "Reads content of a file from the specified path",
                [new PropertyDescription("path", Text("Path of the file to read"), true)],
                Text("Content of the file")),
            new FunctionDescription("FileExists", "Checks whether a file exists at the specified path",
                [new PropertyDescription("path", Text("Path of the file to check"), true)],
                new TypeDescription(TypeKind.Boolean)),
            new FunctionDescription("PhotosDirectory", "Gets the path to the directory used for storing photos", [],
                Text("Path of the directory")));
    }

    public static PluginManifest Storage()
    {
        return Manifest("Storage plugin", "Device storage information and management",
            new FunctionDescription("GetFreeSpace", "Returns the amount of free storage space available on the device in bytes", [],
                new TypeDescription(TypeKind.Integer, Minimum: 0)),
            new FunctionDescription("GetTotalSpace", "Returns the total storage capacity of the device in bytes", [],
                new TypeDescription(TypeKind.Integer, Minimum: 0)));
    }

    public static PluginManifest DeviceInformation()
    {
        return Manifest("Device information plugin", "Provides information about the current device",
            new FunctionDescription("GetDeviceName", "Returns the human-readable name of the device", [],
                Text("Name of the device")),
            new FunctionDescription("GetDeviceType", "Returns the type of the device such as phone, laptop, desktop or tablet", [],
                new TypeDescription(TypeKind.String, Enum: ["phone", "laptop", "desktop", "tablet"])),
            new FunctionDescription("GetOperatingSystem", "Returns the operating system running on the device", [],
                Text("Name and version of the operating system")));
    }

    public static PluginManifest Notifications()
    {
        return Manifest("Notifications plugin", "Device notification capabilities",
            new FunctionDescription("SendNotification", "Displays a notification to the user on the device",
            [
                new PropertyDescription("title", Text("Notification title", maxLength: 100), true),
                new PropertyDescription("message", Text("Notification message"), true)
            ], null));
    }

    public static PluginManifest Clipboard()
    {
        return Manifest("Clipboard plugin", "Device clipboard capabilities",
            new FunctionDescription("GetClipboard", "Returns the current text content of the device clipboard", [],
                Text("Text in the clipboard", nullable: true)),
            new FunctionDescription("SetClipboard", "Sets the text content of the device clipboard",
                [new PropertyDescription("content", Text("Text content to put into the clipboard"), true)], null));
    }

    public static PluginManifest Display()
    {
        return Manifest("Display plugin", "Device display capabilities",
            new FunctionDescription("ShowMessage", "Displays a message on the device screen",
                [new PropertyDescription("message", Text("Message to display"), true)], null),
            new FunctionDescription("ShowImage", "Displays an image identified by an image reference on the device screen",
                [new PropertyDescription("imageReference", Text("Reference to the image that should be displayed"), true)], null));
    }

    public static PluginManifest Location()
    {
        return Manifest("Location plugin", "Device location capabilities",
            new FunctionDescription("GetLocation", "Returns the current device location as a human-readable string", [],
                Text("Location of the device")));
    }

    public static PluginManifest SystemControl()
    {
        return Manifest("System control plugin", "Device system control capabilities",
            new FunctionDescription("RestartDevice", "Restarts the device", [],
                new TypeDescription(TypeKind.Boolean, Description: "Whether the restart started")),
            new FunctionDescription("ShutdownDevice", "Shuts down the device", [],
                new TypeDescription(TypeKind.Boolean, Description: "Whether the shutdown started")));
    }

    private static TypeDescription Text(string description, bool nullable = false, int? maxLength = null, string? contentEncoding = null)
    {
        return new TypeDescription(TypeKind.String, nullable, description, MaxLength: maxLength, ContentEncoding: contentEncoding);
    }

    // the same name and version always give the same hash, so a second seed run changes nothing
    private static PluginManifest Manifest(string name, string description, params FunctionDescription[] functions)
    {
        const string version = "1.0.0";
        var hash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes($"{name} {version}")));

        return new PluginManifest(name, description, version, hash, functions);
    }
}
