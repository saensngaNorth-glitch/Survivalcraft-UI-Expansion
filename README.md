# Survivalcraft UI Expansion v0.3.0
Target: Survivalcraft 2.4.0.0 / API 1.9.3.

This pass follows the public API 1.9 template architecture: ModLoader + HarmonyX + CanvasWidget. The Harmony postfix adds one widget to each player's `GuiWidget` after `ComponentPlayer.Load`; the widget reads `ModSettingsManager` and displays the current time and `ComponentBody.Position`.

Settings: HUD toggle, World Information toggle, HUD Scale 0.5–1.5.

Source-only: this environment has no .NET SDK, so no compiled DLL or installable `.scmod` is claimed here.
