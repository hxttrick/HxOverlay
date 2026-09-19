using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace HxOverlay;

[BepInPlugin(
    "hxttrick.overlay",
    "HxOverlay",
    "1.0.0"
)]
public class OverlayPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Logger;
    internal static AssetBundle AssetBundle;
    internal static ConfigFile Config;
    
    private void Awake()
    {
        Logger = base.Logger;
        Config = base.Config;

        AssetBundle = Utils.LoadAssetBundle();

        OverlayConfig.RegisterAll();
        OverlayKeybinds.RegisterAll();

        Features.PlayerBaseOverlay.Start();
        
        new Harmony("hxttrick.overlay").PatchAll();
    }

    private void Update()
    {
        OverlayKeybinds.Update();
    }
}

