using BepInEx.Configuration;
using UnityEngine;

namespace HxOverlay;

public class OverlayKeybinds
{
    public static ConfigEntry<KeyboardShortcut> PlayerBaseOverlay;
    
    public static void RegisterAll()
    {
        PlayerBaseOverlay = Register(
            "Player Base indicators",
            KeyCode.F8,
            OverlayConfig.PlayerBaseOverlay.Description
        );
    }
    
    private static ConfigEntry<KeyboardShortcut> Register(string name, KeyCode defaultKey, ConfigDescription description)
    {
        var entry = OverlayPlugin.Config.Bind("Keybinds", name, new KeyboardShortcut(defaultKey), description);
        OverlayPlugin.Logger.LogInfo($"Registered keybind '{name}'.");
        
        return entry;
    }
    
    private static ConfigEntry<KeyboardShortcut> Register(string name, KeyCode defaultKey, string description)
    {
        return Register(name, defaultKey, new ConfigDescription(description));
    }

    public static void Update()
    {
        if (PlayerBaseOverlay.Value.IsDown())
        {
            OverlayConfig.PlayerBaseOverlay.Value = !OverlayConfig.PlayerBaseOverlay.Value;
        }
    }
}