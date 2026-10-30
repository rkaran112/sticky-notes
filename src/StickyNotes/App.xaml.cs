using System.IO;
using System.Windows;
using StickyNotes.Data;
using StickyNotes.Services;

namespace StickyNotes;

public partial class App : System.Windows.Application
{
    private readonly SingleInstanceService _singleInstance = new();
    private readonly NoteRepository _repository = new();
    private readonly Dictionary<string, NoteWindow> _openWindows = new();
    private TrayIconService? _tray;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (!_singleInstance.TryAcquire())
        {
            // Another instance is already running with all notes loaded; nothing to do.
            Shutdown();
            return;
        }

        var isFirstRun = !File.Exists(Database.DbPath);
        Database.Initialize();

        var notes = _repository.GetAll();
        if (notes.Count == 0)
        {
            var welcome = new Note
            {
                Content = "Welcome to Sticky Notes!\n\n" +
                           "• Drag the top bar to move\n" +
                           "• Drag an edge/corner to resize\n" +
                           "• Click + for a new note\n" +
                           "• Click ⋮ for colors or to delete\n\n" +
                           "This app runs quietly in the tray and starts with Windows.",
            };
            _repository.Insert(welcome);
            notes.Add(welcome);
        }

        foreach (var note in notes)
        {
            OpenWindow(note);
        }

        if (isFirstRun)
        {
            AutostartService.Enable();
        }

        _tray = new TrayIconService();
        _tray.NewNoteRequested += (_, _) => CreateAndOpenNewNote(anchor: null);
        _tray.ShowAllRequested += (_, _) => ShowAllNotes();
        _tray.ExitRequested += (_, _) => Shutdown();

        // Windows sends WM_QUERYENDSESSION on shutdown/restart/logoff and can then kill the
        // process before the debounce timers in NoteWindow get a chance to fire, so force an
        // immediate synchronous save of every open note here.
        SessionEnding += (_, _) =>
        {
            foreach (var window in _openWindows.Values)
            {
                window.Flush();
            }
        };

        Exit += (_, _) => _tray?.Dispose();
    }

    private void OpenWindow(Note note)
    {
        var window = new NoteWindow(note, _repository);
        window.NewNoteRequested += (_, anchor) => CreateAndOpenNewNote(anchor);
        window.Deleted += (_, id) => _openWindows.Remove(id);
        _openWindows[note.Id] = window;
        window.Show();
    }

    private void CreateAndOpenNewNote(Note? anchor)
    {
        var note = new Note();

        if (anchor is not null)
        {
            note.Left = anchor.Left + 28;
            note.Top = anchor.Top + 28;
        }
        else
        {
            var offset = (_openWindows.Count % 8) * 28;
            note.Left = 120 + offset;
            note.Top = 120 + offset;
        }

        _repository.Insert(note);
        OpenWindow(note);
        _openWindows[note.Id].Activate();
    }

    private void ShowAllNotes()
    {
        foreach (var window in _openWindows.Values)
        {
            if (window.WindowState == WindowState.Minimized)
            {
                window.WindowState = WindowState.Normal;
            }
            window.Show();
            window.Activate();
        }
    }
}
