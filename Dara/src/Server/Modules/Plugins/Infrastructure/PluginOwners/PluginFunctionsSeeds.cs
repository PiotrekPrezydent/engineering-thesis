using System.Collections.Immutable;
using Dara.Server.Modules.Plugins.Domain.PluginOwners.Plugins;

namespace Dara.Server.Modules.Plugins.Infrastructure.PluginOwners;

public static class PluginFunctionsSeeds
{
    public static ImmutableArray<PluginFunction> GetAudioControlFunctions()
    {
        return
        [
            new PluginFunction(
                "SetVolume",
                "Sets the system volume to the requested level (0-100).",
                "bool",
                [
                    new("level", "Volume level from 0 to 100", "int")
                ]
            ),
            new PluginFunction(
                "MuteVolume",
                "Mutes the system audio.",
                "bool",
                []
            ),
            new PluginFunction(
                "UnmuteVolume",
                "Unmutes the system audio.",
                "bool",
                []
            ),
            new PluginFunction(
                "GetVolumeLevel",
                "Returns the current system volume level as an integer from 0 to 100.",
                "int",
                []
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetPowerManagementFunctions()
    {
        return
        [
            new PluginFunction(
                "LockDevice",
                "Locks the operating system session, requiring a password to unlock.",
                "bool",
                []
            ),
            new PluginFunction(
                "SleepDevice",
                "Puts the device into sleep or suspend mode to save power.",
                "bool",
                []
            ),
            new PluginFunction(
                "RestartDevice",
                "Restarts the device gracefully. Requires confirmation or high priority.",
                "bool",
                [
                    new("force", "If true, forces applications to close without saving", "bool")
                ]
            ),
            new PluginFunction(
                "ShutdownDevice",
                "Shuts down the device completely.",
                "bool",
                [
                    new("delayInSeconds", "Time to wait before shutting down. 0 for immediate.", "int")
                ]
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetDeviceLocatorFunctions()
    {
        return
        [
            new PluginFunction(
                "TriggerFindMyDeviceSound",
                "Plays a very loud sound on the device overriding silent mode to help the user locate it.",
                "bool",
                [
                    new("durationInSeconds", "How long the sound should play", "int")
                ]
            ),
            new PluginFunction(
                "FlashCameraLed",
                "Flashes the camera LED flashlight to visually locate the device in the dark.",
                "bool",
                [
                    new("durationInSeconds", "Duration of the flashing effect", "int")
                ]
            ),
            new PluginFunction(
                "VibrateDevice",
                "Triggers the device's vibration motor in a continuous pattern.",
                "bool",
                [
                    new("durationInSeconds", "How long the device should vibrate", "int")
                ]
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetTelephonyFunctions()
    {
        return
        [
            new PluginFunction(
                "MakePhoneCall",
                "Initiates a cellular phone call to the specified phone number.",
                "bool",
                [
                    new("phoneNumber", "The phone number to call, including country code if necessary", "string"),
                    new("useSpeakerphone", "If true, the call starts on the device's loudspeaker", "bool")
                ]
            ),
            new PluginFunction(
                "SendSmsMessage",
                "Sends an SMS text message to the specified phone number.",
                "bool",
                [
                    new("phoneNumber", "The destination phone number", "string"),
                    new("messageContent", "The text content of the SMS", "string")
                ]
            ),
            new PluginFunction(
                "RejectIncomingCall",
                "Rejects the currently incoming phone call and optionally sends a predefined SMS template.",
                "bool",
                [
                    new("sendSmsTemplate", "Optional SMS text to send to the caller after rejecting", "string")
                ]
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetCrossDeviceHandoffFunctions()
    {
        return
        [
            new PluginFunction(
                "ReceiveAndOpenUrl",
                "Receives a URL sent from another device and immediately opens it in the default browser.",
                "bool",
                [
                    new("url", "The URL to open", "string")
                ]
            ),
            new PluginFunction(
                "ReceiveTextNote",
                "Receives a piece of text pushed from another device and displays it on the screen as a sticky note or alert.",
                "bool",
                [
                    new("content", "The text content pushed from the remote device", "string"),
                    new("title", "Optional title for the pushed note", "string")
                ]
            ),
            new PluginFunction(
                "ShareLocationToDevice",
                "Retrieves the current GPS coordinates of this device and returns them to the server to be shared.",
                "string",
                [] 
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetVoiceInteractionFunctions()
    {
        return
        [
            new PluginFunction(
                "SpeakText",
                "Uses the device's Text-To-Speech (TTS) engine to read the provided text out loud.",
                "bool",
                [
                    new("textToSpeak", "The text that the assistant should read aloud", "string"),
                    new("languageCode", "Language code for the TTS voice (e.g., en-US, pl-PL)", "string")
                ]
            ),
            new PluginFunction(
                "TriggerVoiceCommandPrompt",
                "Wakes up the device's microphone and prompts the user for a follow-up voice command.",
                "bool",
                [
                    new("promptMessage", "Optional text to speak before activating the microphone", "string")
                ]
            ),
            new PluginFunction(
                "StopSpeaking",
                "Immediately stops any ongoing Text-To-Speech playback on the device.",
                "bool",
                []
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetRemoteSensorFunctions()
    {
        return
        [
            new PluginFunction(
                "TakeRemotePhoto",
                "Captures a photo using the device's camera silently and returns a temporary URL or base64 string of the image.",
                "string",
                [
                    new("useFrontCamera", "If true, uses the front-facing (selfie) camera; otherwise uses the main rear camera", "bool"),
                    new("enableFlash", "If true, forces the camera flash to fire", "bool")
                ]
            ),
            new PluginFunction(
                "RecordAudioSnippet",
                "Records an audio snippet using the device's microphone for a specified duration and returns a link to the file.",
                "string",
                [
                    new("durationInSeconds", "The length of the recording in seconds", "int")
                ]
            ),
            new PluginFunction(
                "GetDeviceSensorsData",
                "Retrieves current environmental data from the device, such as ambient light level, temperature, or barometric pressure, if available.",
                "string",
                []
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetRemoteAuthenticationFunctions()
    {
        return
        [
            new PluginFunction(
                "RequestBiometricApproval",
                "Triggers a native biometric (Fingerprint, FaceID, Windows Hello) prompt on the device to approve a remote action. Returns true only if the user authenticates.",
                "bool",
                [
                    new("approvalReason", "The text displayed to the user explaining what they are approving (e.g., 'Confirm login on PC')", "string")
                ]
            ),
            new PluginFunction(
                "RequestPinEntry",
                "Displays a secure prompt on the device asking the user to type a short PIN or password, then returns the entered value.",
                "string",
                [
                    new("promptMessage", "Instructions for the user", "string")
                ]
            )
        ];
    }
    
    public static ImmutableArray<PluginFunction> GetNotificationSyncFunctions()
    {
        return
        [
            new PluginFunction(
                "FetchUnreadNotifications",
                "Retrieves a serialized list of recent, unread push notifications currently active on the device.",
                "string",
                [
                    new("maxCount", "The maximum number of notifications to retrieve", "int"),
                    new("filterByApp", "Optional. If provided, retrieves notifications only from a specific app (e.g., WhatsApp)", "string")
                ]
            ),
            new PluginFunction(
                "DismissNotification",
                "Remotely clears or dismisses a specific notification on the device.",
                "bool",
                [
                    new("notificationId", "The unique identifier of the notification to dismiss", "string")
                ]
            ),
            new PluginFunction(
                "SendUrgentAlert",
                "Bypasses Do Not Disturb mode to display a full-screen, high-priority alert on the device.",
                "bool",
                [
                    new("title", "The headline of the alert", "string"),
                    new("message", "The main body of the alert", "string")
                ]
            )
        ];
    }
    
    
}