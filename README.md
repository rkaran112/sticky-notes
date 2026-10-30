# Sticky Notes

A lightweight, native Windows sticky-notes app (WPF/.NET 8) that mimics the built-in
Windows Sticky Notes: colored, draggable, resizable note windows, a system tray icon,
autosave to a local SQLite database, and autostart on login.

## Usage

- **New note**: click the `+` in a note's top bar, or "New Note" from the tray icon.
- **Move**: drag the top bar. **Resize**: drag an edge/corner (Aero Snap works).
- **Color / delete**: click `⋮` in the top bar.
- **Close app entirely**: right-click the tray icon → Exit. Closing all notes does
  *not* quit the app — it keeps running in the tray, same as the real Sticky Notes.
- **Autostart**: enabled automatically on first run. Toggle it anytime via the tray
  icon's "Start with Windows" checkbox.

## Data

Notes are stored in `%AppData%\StickyNotesApp\notes.db` (SQLite). Deleting a note
deletes its row immediately (no undo).

## Building / installing

Requires the .NET 8 SDK.

```powershell
./publish.ps1
```

This builds a Release, framework-dependent, ReadyToRun single-file exe and installs
it to `%LocalAppData%\Programs\StickyNotesApp\StickyNotes.exe`. Re-run it after any
code change to update the installed copy (the autostart registry entry always points
at this fixed install path, not the repo's `bin` folder).

## Uninstalling

1. Right-click the tray icon → Exit.
2. Remove the `StickyNotesApp` value under
   `HKCU\Software\Microsoft\Windows\CurrentVersion\Run` (or uncheck "Start with
   Windows" first, from the tray menu, while it's still running).
3. Delete `%LocalAppData%\Programs\StickyNotesApp` and, if you want to discard your
   notes, `%AppData%\StickyNotesApp`.

## Footprint

Release build: ~65MB private memory / ~125MB working set at idle, 0% CPU when idle
(no polling/timers running unless you're actively typing or dragging). Framework-
dependent (uses the shared .NET 8 Windows Desktop runtime already on this machine)
+ ReadyToRun for fast cold start, so it shouldn't meaningfully slow down boot.
