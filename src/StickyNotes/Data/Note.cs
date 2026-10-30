namespace StickyNotes.Data;

public class Note
{
    public string Id { get; set; } = Guid.NewGuid().ToString("N");
    public string Content { get; set; } = string.Empty;
    public string Color { get; set; } = "Yellow";
    public double Left { get; set; } = 100;
    public double Top { get; set; } = 100;
    public double Width { get; set; } = 260;
    public double Height { get; set; } = 260;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
