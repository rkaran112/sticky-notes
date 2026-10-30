using Brush = System.Windows.Media.Brush;
using BrushConverter = System.Windows.Media.BrushConverter;
using Brushes = System.Windows.Media.Brushes;

namespace StickyNotes;

public record NoteColorDef(string Name, string Background, string CaptionAccent);

public static class NoteColors
{
    // Palette modeled on the Windows 11 Sticky Notes swatches.
    public static readonly NoteColorDef[] All =
    [
        new("Yellow", "#FFF9C4", "#F5E27A"),
        new("Green", "#C9E4CA", "#9FCBA3"),
        new("Pink", "#F6D6E0", "#EBB3C7"),
        new("Purple", "#DCD0E8", "#C4AEDA"),
        new("Blue", "#C7E3F1", "#A3CFE6"),
        new("Gray", "#E3E3E3", "#C9C9C9"),
        new("Charcoal", "#4C4C4C", "#3A3A3A"),
    ];

    public static NoteColorDef Get(string name) =>
        Array.Find(All, c => c.Name == name) ?? All[0];

    public static Brush BackgroundBrush(string name) =>
        (Brush)new BrushConverter().ConvertFromString(Get(name).Background)!;

    public static Brush CaptionBrush(string name) =>
        (Brush)new BrushConverter().ConvertFromString(Get(name).CaptionAccent)!;

    public static Brush TextBrush(string name) =>
        name == "Charcoal" ? Brushes.WhiteSmoke : Brushes.Black;
}
