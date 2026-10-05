# Survivalcraft UI Expansion v0.4.0

Target: Survivalcraft 2.4.0.0 / API 1.9.3.

## Changes
- Removes the normal UI Expansion overlay from the gameplay screen so it cannot overlap the game's HUD.
- Uses the game's existing `•••` More button and its `MoreContents` bar.
- Inserts a bitmap-style `Debug` button immediately after the existing `?` (`HelpButton`), using the game's own button style and 68x64 sizing.
- Tapping `Debug` toggles an F3-style debug overlay with precise XYZ coordinates.
- No second `•••` button is created.

The Debug button is inserted at runtime so the original game `GameWidget.xml` is not replaced.
