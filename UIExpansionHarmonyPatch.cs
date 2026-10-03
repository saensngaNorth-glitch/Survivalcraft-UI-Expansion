using HarmonyLib;
namespace Game;
[HarmonyPatch(typeof(ComponentPlayer), nameof(ComponentPlayer.Load))]
internal static class UIExpansionHarmonyPatch
{
    private static void Postfix(ComponentPlayer __instance)
    {
        if (__instance.GuiWidget == null) return;
        __instance.GuiWidget.AddChildren(new UIExpansionWidget(__instance));
    }
}
