using HarmonyLib;

namespace Game;

[HarmonyPatch(typeof(ComponentPlayer), nameof(ComponentPlayer.Load))]
internal static class UIExpansionHarmonyPatch
{
    private static void Postfix(ComponentPlayer __instance)
    {
        if (__instance.GuiWidget == null)
            return;

        if (__instance.GuiWidget.Children.Find<UIExpansionWidget>("UIExpansionRoot", false) != null)
            return;

        var widget = new UIExpansionWidget(__instance)
        {
            Name = "UIExpansionRoot"
        };

        __instance.GuiWidget.AddChildren(widget);
    }
}
