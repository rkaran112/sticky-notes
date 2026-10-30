namespace StickyNotes.Services;

public sealed class SingleInstanceService : IDisposable
{
    private const string MutexName = "Local\\StickyNotesApp_SingleInstance_9F3B1C7E";
    private Mutex? _mutex;

    public bool TryAcquire()
    {
        _mutex = new Mutex(initiallyOwned: true, MutexName, out var createdNew);
        return createdNew;
    }

    public void Dispose()
    {
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
    }
}
