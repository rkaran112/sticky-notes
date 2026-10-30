using System.IO;
using System.Windows.Forms;

namespace StickyNotes.Services;

public sealed class TrayIconService : IDisposable
{
    private readonly NotifyIcon _notifyIcon;
    private readonly ToolStripMenuItem _startWithWindowsItem;

    public event EventHandler? NewNoteRequested;
    public event EventHandler? ShowAllRequested;
    public event EventHandler? ExitRequested;

    public TrayIconService()
    {
        var newNoteItem = new ToolStripMenuItem("New Note");
        newNoteItem.Click += (_, _) => NewNoteRequested?.Invoke(this, EventArgs.Empty);

        var showAllItem = new ToolStripMenuItem("Show All Notes");
        showAllItem.Click += (_, _) => ShowAllRequested?.Invoke(this, EventArgs.Empty);

        _startWithWindowsItem = new ToolStripMenuItem("Start with Windows")
        {
            Checked = AutostartService.IsEnabled(),
            CheckOnClick = true,
        };
        _startWithWindowsItem.Click += (_, _) =>
        {
            if (_startWithWindowsItem.Checked) AutostartService.Enable();
            else AutostartService.Disable();
        };

        var exitItem = new ToolStripMenuItem("Exit");
        exitItem.Click += (_, _) => ExitRequested?.Invoke(this, EventArgs.Empty);

        var menu = new ContextMenuStrip();
        menu.Items.Add(newNoteItem);
        menu.Items.Add(showAllItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(_startWithWindowsItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        var iconPath = Environment.ProcessPath;
        var icon = !string.IsNullOrEmpty(iconPath) && File.Exists(iconPath)
            ? System.Drawing.Icon.ExtractAssociatedIcon(iconPath)
            : System.Drawing.SystemIcons.Application;

        _notifyIcon = new NotifyIcon
        {
            Icon = icon,
            Text = "Sticky Notes",
            Visible = true,
            ContextMenuStrip = menu,
        };
        _notifyIcon.DoubleClick += (_, _) => ShowAllRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
