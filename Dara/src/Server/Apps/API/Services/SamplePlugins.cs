using Dara.Server.Modules.Plugins.Application.Data;

namespace Dara.Server.Apps.API.Services;

public static class SamplePlugins
{
    public static PluginDescriptor CameraPlugin()
    {
        var builder = PluginDescriptor.Builder;
        builder
            .WithName("Camera plugin")
            .WithDescription("Device camera capabilities")
            .AddFunction(f => f
                .WithName("CameraCapture")
                .WithDescription("Takes photo using device camera and return content of created picture")
                .WithReturnTypeName("string")
                );
        return builder.Build();
    }
    
    public static PluginDescriptor FileManagementPlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("File management plugin")
            .WithDescription("File and directory management capabilities")
            .AddFunction(f => f
                .WithName("SaveFile")
                .WithDescription("Saves content as a file")
                .AddParameter(p => p
                    .WithName("path")
                    .WithDescription("Path where the file will be saved")
                    .WithTypeName("string"))
                .AddParameter(p => p
                    .WithName("content")
                    .WithDescription("Content that will be saved")
                    .WithTypeName("string")))
            .AddFunction(f => f
                .WithName("ReadFile")
                .WithDescription("Reads content of a file from the specified path")
                .AddParameter(p => p
                    .WithName("path")
                    .WithDescription("Path of the file to read")
                    .WithTypeName("string"))
                .WithReturnTypeName("string"))
            .AddFunction(f => f
                .WithName("FileExists")
                .WithDescription("Checks whether a file exists at the specified path")
                .AddParameter(p => p
                    .WithName("path")
                    .WithDescription("Path of the file to check")
                    .WithTypeName("string"))
                .WithReturnTypeName("bool"))
            .AddFunction(f => f
                .WithName("PhotosDirectory")
                .WithDescription("Gets the path to the directory used for storing photos")
                .WithReturnTypeName("string"));

        return builder.Build();
    }
    
    public static PluginDescriptor StoragePlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("Storage plugin")
            .WithDescription("Device storage information and management")
            .AddFunction(f => f
                .WithName("GetFreeSpace")
                .WithDescription("Returns the amount of free storage space available on the device in bytes")
                .WithReturnTypeName("long"))
            .AddFunction(f => f
                .WithName("GetTotalSpace")
                .WithDescription("Returns the total storage capacity of the device in bytes")
                .WithReturnTypeName("long"));

        return builder.Build();
    }

    public static PluginDescriptor DeviceInformationPlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("Device information plugin")
            .WithDescription("Provides information about the current device")
            .AddFunction(f => f
                .WithName("GetDeviceName")
                .WithDescription("Returns the human-readable name of the device")
                .WithReturnTypeName("string"))
            .AddFunction(f => f
                .WithName("GetDeviceType")
                .WithDescription("Returns the type of the device such as phone, laptop, desktop or tablet")
                .WithReturnTypeName("string"))
            .AddFunction(f => f
                .WithName("GetOperatingSystem")
                .WithDescription("Returns the operating system running on the device")
                .WithReturnTypeName("string"));

        return builder.Build();
    }
    
    public static PluginDescriptor NotificationsPlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("Notifications plugin")
            .WithDescription("Device notification capabilities")
            .AddFunction(f => f
                .WithName("SendNotification")
                .WithDescription("Displays a notification to the user on the device")
                .AddParameter(p => p
                    .WithName("title")
                    .WithDescription("Notification title")
                    .WithTypeName("string"))
                .AddParameter(p => p
                    .WithName("message")
                    .WithDescription("Notification message")
                    .WithTypeName("string")));

        return builder.Build();
    }
    
    public static PluginDescriptor ClipboardPlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("Clipboard plugin")
            .WithDescription("Device clipboard capabilities")
            .AddFunction(f => f
                .WithName("GetClipboard")
                .WithDescription("Returns the current text content of the device clipboard")
                .WithReturnTypeName("string"))
            .AddFunction(f => f
                .WithName("SetClipboard")
                .WithDescription("Sets the text content of the device clipboard")
                .AddParameter(p => p
                    .WithName("content")
                    .WithDescription("Text content to put into the clipboard")
                    .WithTypeName("string")));

        return builder.Build();
    }
    public static PluginDescriptor DisplayPlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("Display plugin")
            .WithDescription("Device display capabilities")
            .AddFunction(f => f
                .WithName("ShowMessage")
                .WithDescription("Displays a message on the device screen")
                .AddParameter(p => p
                    .WithName("message")
                    .WithDescription("Message to display")
                    .WithTypeName("string")))
            .AddFunction(f => f
                .WithName("ShowImage")
                .WithDescription("Displays an image identified by an image reference on the device screen")
                .AddParameter(p => p
                    .WithName("imageReference")
                    .WithDescription("Reference to the image that should be displayed")
                    .WithTypeName("string")));

        return builder.Build();
    }
    
    public static PluginDescriptor LocationPlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("Location plugin")
            .WithDescription("Device location capabilities")
            .AddFunction(f => f
                .WithName("GetLocation")
                .WithDescription("Returns the current device location as a human-readable string")
                .WithReturnTypeName("string"));

        return builder.Build();
    }
    
    public static PluginDescriptor SystemControlPlugin()
    {
        var builder = PluginDescriptor.Builder;

        builder
            .WithName("System control plugin")
            .WithDescription("Device system control capabilities")
            .AddFunction(f => f
                .WithName("RestartDevice")
                .WithDescription("Restarts the device")
                .WithReturnTypeName("bool"))
            .AddFunction(f => f
                .WithName("ShutdownDevice")
                .WithDescription("Shuts down the device")
                .WithReturnTypeName("bool"));

        return builder.Build();
    }
}