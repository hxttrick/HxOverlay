using System.Reflection;
using UnityEngine;

namespace HxOverlay;

public class Utils
{
    public static AssetBundle LoadAssetBundle()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        
        string resourceName = "HxOverlay.overlay.assetbundle";
        
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
        {
            OverlayPlugin.Logger.LogError($"Could not find embedded resource '{resourceName}'. Available Resources: {string.Join(", ", assembly.GetManifestResourceNames())}");
            return null;
        }
        
        byte[] data = new byte[stream.Length];
        int offset = 0;
        while (offset < stream.Length)
        {
            int read = stream.Read(data, offset, data.Length - offset);
            
            if (read == 0)
                break;
            
            offset += read;
        }
        
        var bundle = AssetBundle.LoadFromMemory(data);

        if (bundle == null)
        {
            OverlayPlugin.Logger.LogError("Failed to load embedded AssetBundle.");
            return null;
        }

        foreach (var asset in bundle.GetAllAssetNames())
        {
            OverlayPlugin.Logger.LogInfo($"Loaded asset '{asset}'.");
        }
        
        return bundle;
    }
    
    public static class NamingConventions
    {
        public static string GameObject(string name)
        {
            return $"HxOverlay_{name}";
        }
    }
}