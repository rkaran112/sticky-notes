using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using StickyNotes.Data;

namespace StickyNotes;

public partial class NoteWindow : Window
{
    private readonly NoteRepository _repository;
    private readonly DispatcherTimer _contentSaveTimer;
    private readonly DispatcherTimer _boundsSaveTimer;
    private bool _isDeleting;
    private bool _isLoaded;

    public Note Note { get; }

    public event EventHandler<Note>? NewNoteRequested;
    public event EventHandler<string>? Deleted;

    public NoteWindow(Note note, NoteRepository repository)
    {
        InitializeComponent();

        Note = note;
        _repository = repository;

        _contentSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(600) };
        _contentSaveTimer.Tick += (_, _) =>
        {
            _contentSaveTimer.Stop();
            _repository.UpdateContent(Note.Id, ContentBox.Text);
        };

        _boundsSaveTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(400) };
        _boundsSaveTimer.Tick += (_, _) =>
        {
            _boundsSaveTimer.Stop();
            _repository.UpdateBounds(Note.Id, Left, Top, Width, Height);
        };

        Left = note.Left;
        Top = note.Top;
        Width = note.Width;
        Height = note.Height;
        ContentBox.Text = note.Content;

        ApplyColor(note.Color);

        LocationChanged += (_, _) => DebounceBoundsSave();
        SizeChanged += (_, _) => DebounceBoundsSave();
        Closing += NoteWindow_Closing;

        Loaded += (_, _) => _isLoaded = true;
    }

    private void DebounceBoundsSave()
    {
        if (!_isLoaded) return;
        _boundsSaveTimer.Stop();
        _boundsSaveTimer.Start();
    }

    private void ContentBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_isLoaded) return;
        Note.Content = ContentBox.Text;
        _contentSaveTimer.Stop();
        _contentSaveTimer.Start();
    }

    private void ApplyColor(string colorName)
    {
        Note.Color = colorName;
        var bg = NoteColors.BackgroundBrush(colorName);
        var caption = NoteColors.CaptionBrush(colorName);
        var fg = NoteColors.TextBrush(colorName);

        RootGrid.Background = bg;
        CaptionBar.Background = caption;
        ContentBox.Foreground = fg;
        ContentBox.CaretBrush = fg;

        foreach (var child in CaptionBar.Children)
        {
            if (child is System.Windows.Controls.Button b) b.Foreground = fg;
        }
    }

    private void NewNoteButton_Click(object sender, RoutedEventArgs e)
    {
        NewNoteRequested?.Invoke(this, Note);
    }

    private void MenuButton_Click(object sender, RoutedEventArgs e)
    {
        var menu = new ContextMenu();

        foreach (var color in NoteColors.All)
        {
            var item = new MenuItem
            {
                Header = color.Name,
                Icon = new System.Windows.Shapes.Ellipse
                {
                    Width = 14,
                    Height = 14,
                    Fill = NoteColors.BackgroundBrush(color.Name),
                    Stroke = System.Windows.Media.Brushes.Gray,
                    StrokeThickness = 0.5,
                },
            };
            item.Click += (_, _) =>
            {
                ApplyColor(color.Name);
                _repository.UpdateColor(Note.Id, color.Name);
            };
            menu.Items.Add(item);
        }

        menu.Items.Add(new Separator());

        var deleteItem = new MenuItem { Header = "Delete note" };
        deleteItem.Click += (_, _) => DeleteAndClose();
        menu.Items.Add(deleteItem);

        menu.PlacementTarget = MenuButton;
        menu.IsOpen = true;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e) => DeleteAndClose();

    private void DeleteAndClose()
    {
        _isDeleting = true;
        Close();
    }

    private void NoteWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
    {
        _contentSaveTimer.Stop();
        _boundsSaveTimer.Stop();

        if (_isDeleting)
        {
            _repository.Delete(Note.Id);
            Deleted?.Invoke(this, Note.Id);
        }
        else
        {
            Flush();
        }
    }

    public void Flush()
    {
        _repository.UpdateContent(Note.Id, ContentBox.Text);
        _repository.UpdateBounds(Note.Id, Left, Top, Width, Height);
    }
}
