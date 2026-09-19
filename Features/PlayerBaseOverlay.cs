using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace HxOverlay.Features;

public static class PlayerBaseOverlay
{
    static Shader playerBaseOverlayShader;
    static Material playerBaseOverlayMaterial;
    
    static Vector4[] allAreas = new Vector4[128];
    static List<GameObject> allOverlays = new (128);

    static string overlayName = Utils.NamingConventions.GameObject("PlayerBaseIndicator");
    
    public static void Start()
    {
        playerBaseOverlayShader = OverlayPlugin.AssetBundle.LoadAsset<Shader>("assets/effectarea.shader");
        playerBaseOverlayMaterial = new Material(playerBaseOverlayShader);
        
        Shader.SetGlobalColor(
            "_HxPlayerBaseColor",
            new Color(1.2f, 0.5f, 0.1f, 0.08f)
        );
        Shader.SetGlobalFloat("_HxPlayerBaseExtrude", 0.2f);
        
        OverlayConfig.PlayerBaseOverlay.SettingChanged += (_,_) =>
        {
            allOverlays.RemoveAll(overlay => overlay == null);
            foreach (var overlay in allOverlays)
                overlay.SetActive(OverlayConfig.PlayerBaseOverlay.Value);
        };
    }

    static void UpdateAllAreas()
    {
        int areaCount = 0;

        foreach (var area in GetAllEffectAreas())
        {
            if (area == null)
                continue;
            
            if (!area.isActiveAndEnabled)
                continue;

            if (area.m_type != EffectArea.Type.PlayerBase)
                continue;

            Vector3 p = area.transform.position;

            allAreas[areaCount++] = new Vector4(
                p.x,
                p.z,
                area.GetRadius(),
                0f
            );

            if (areaCount == allAreas.Length)
                break;
        }

        Shader.SetGlobalInt("_HxPlayerBaseCount", areaCount);
        Shader.SetGlobalVectorArray("_HxPlayerBaseAreas", allAreas);
    }
    
    private static List<EffectArea> GetAllEffectAreas()
    {
        return AccessTools.StaticFieldRefAccess<List<EffectArea>>(
            typeof(EffectArea),
            "s_allAreas"
        );
    }
    
    public static void AddOverlay(Heightmap hm)
    {
        var existing = hm.transform.Find(overlayName);

        var originalFilter = hm.GetComponent<MeshFilter>();

        if (originalFilter?.sharedMesh == null)
            return;
        
        if (existing != null)
        {
            MeshFilter overlayFilter = existing.GetComponent<MeshFilter>();

            if (overlayFilter != null)
                overlayFilter.sharedMesh = originalFilter.sharedMesh;

            return;
        }

        var overlay = new GameObject(overlayName);
        overlay.SetActive(OverlayConfig.PlayerBaseOverlay.Value);
        
        allOverlays.RemoveAll(overlay => overlay == null);
        allOverlays.Add(overlay);

        overlay.transform.SetParent(hm.transform, false);

        var filter = overlay.AddComponent<MeshFilter>();
        filter.sharedMesh = originalFilter.sharedMesh;

        var renderer = overlay.AddComponent<MeshRenderer>();
        renderer.sharedMaterial = playerBaseOverlayMaterial;
    }
    
    static class Patches
    {
        [HarmonyPatch(typeof(Heightmap), nameof(Heightmap.Regenerate))]
        static class Heightmap_Regenerate
        {
            static void Postfix(Heightmap __instance, bool ___m_isDistantLod)
            {
                if (!___m_isDistantLod)
                    AddOverlay(__instance);
            }
        }

        [HarmonyPatch(typeof(EffectArea), nameof(EffectArea.OnEnable))]
        static class EffectArea_OnEnable
        {
            static void Postfix(EffectArea __instance)
            {
                if ((__instance.m_type & EffectArea.Type.PlayerBase) != 0)
                    UpdateAllAreas();
            }
        }

        [HarmonyPatch(typeof(EffectArea), nameof(EffectArea.OnDisable))]
        static class EffectArea_OnDisable
        {
            static void Postfix(EffectArea __instance)
            {
                if ((__instance.m_type & EffectArea.Type.PlayerBase) != 0)
                    UpdateAllAreas();
            }
        }
    }
}