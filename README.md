# Survivalcraft UI Expansion v0.4.0

Target: Survivalcraft 2.4.0.0 / API 1.9.3.

## Changes
- Keeps the existing HUD time/position overlay.
- Uses the game's existing `•••` More button and its `MoreContents` bar.
- Inserts a text button named `Debug` immediately after the existing `?` (`HelpButton`).
- Tapping `Debug` toggles an F3-style debug overlay with precise XYZ coordinates.
- No second `•••` button is created.

The Debug button is inserted at runtime so the original game `GameWidget.xml` is not replaced.
