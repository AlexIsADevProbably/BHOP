using LabApi.Events.CustomHandlers;

namespace BHOP;

using System;
using System.IO;
using System.Linq;
using System;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins.Enums;
using CustomEventHandler.Events;

public class Plugin : LabApi.Loader.Features.Plugins.Plugin<Config>
{
    public static Plugin Instance { get; private set; } = null!;
    public override string Name => "BHOP";
    public override string Author => "Alex";
    public override string Description => "Bhop Plugin for surfs.fun";
    public override Version Version => new(1, 0, 0);
    public override Version RequiredApiVersion => new(LabApiProperties.CompiledVersion);
    public override LoadPriority Priority => LoadPriority.High;
    public Events Events { get; } = new();


    public override void Enable()
    {
        if (Config is null)
            throw new Exception("Config is null!");
        
        if (Config.Debug)
            Logger.Debug("Debug mode enabled.");
            
        Instance = this;
        try
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

            string pluginsDirectory = Path.Combine(
                homeDirectory,
                ".config",
                "SCP Secret Laboratory",
                "LabAPI",
                "plugins"
            );

            string audioPath = Directory
                .GetDirectories(pluginsDirectory, "*", SearchOption.AllDirectories)
                .FirstOrDefault(dir =>
                    Path.GetFileName(dir).Equals("sounds", StringComparison.OrdinalIgnoreCase) &&
                    File.Exists(Path.Combine(dir, "bhop.ogg"))
                );

            if (string.IsNullOrWhiteSpace(audioPath))
            {
                throw new Exception($"Could not find sounds folder in: {pluginsDirectory}");
            }

            AudioClipStorage.LoadClip(Path.Combine(audioPath, "bhop.ogg"), "bhop");
            AudioClipStorage.LoadClip(Path.Combine(audioPath, "aura.ogg"), "aura");
            AudioClipStorage.LoadClip(Path.Combine(audioPath, "boing.ogg"), "boing");

            Logger.Info($"BHOP Custom Audio Clips Loaded Successfully from: {audioPath}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Failed to load BHOP custom audio clips: {ex}");
        }
        CustomHandlersManager.RegisterEventsHandler(Events);

    }

    public override void Disable()
    {
        Instance = null!;
        CustomHandlersManager.UnregisterEventsHandler(Events);
    }
}
