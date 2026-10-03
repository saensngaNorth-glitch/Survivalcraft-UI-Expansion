using System;
using System.Collections.Generic;
using Engine;
using HarmonyLib;
namespace Game;
public sealed class UIExpansionModLoader : ModLoader
{
    public const string PackageName = "pirachpol.UIExpansion";
    public override void __ModInitialize()
    {
        ModsManager.RegisterHook("OnLoadingFinished", this, 10000);
        new Harmony(PackageName).PatchAll();
    }
    public override void OnLoadingFinished(List<Action> actions)
    {
        ModSettingsManager.TryGet(out bool hud, PackageName, "UIExpansionSettings", "HUD");
        ModSettingsManager.TryGet(out bool worldInfo, PackageName, "UIExpansionSettings", "WorldInfo");
        ModSettingsManager.TryGet(out float scale, PackageName, "UIExpansionSettings", "HUDScale");
        Log.Information($"UI Expansion 0.3.0 loaded. HUD={hud}, WorldInfo={worldInfo}, Scale={scale:0.##}");
    }
}
