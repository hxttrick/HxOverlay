using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace HxOverlay;

public class OverlayConfig
{
    public static ConfigEntry<bool> PlayerBaseOverlay;

    private static List<ConfigEntryBase> entries = [];
    
    public static void RegisterAll()
    {
        OverlayPlugin.Logger.LogInfo("Registering config entries...");

        PlayerBaseOverlay = Register(
            "Overlays",
            "Player Base indicators",
            false,
            "Show indicators for 'PlayerBase' effect areas, aka regions where mobs cannot spawn due to player built base pieces."
        );
        
        entries.ForEach(entry => OverlayPlugin.Logger.LogInfo($"Loaded entry {entry.Definition.Key} as {entry.BoxedValue}."));
    }
    
    private static ConfigEntry<T> Register<T>(string section, string name, T defaultValue, ConfigDescription description, KeyCode key)
    {
        var entry = OverlayPlugin.Config.Bind(section, name, defaultValue, description);
        OverlayPlugin.Logger.LogInfo($"Registered entry '{name}'.");

        entries.Add(entry);
        
        return entry;
    }
    
    private static ConfigEntry<T> Register<T>(string section, string name, T defaultValue, string description, KeyCode key = KeyCode.None)
    {
        return Register(section, name, defaultValue, new ConfigDescription(description), key);
    }
}