namespace TPLTask4;

internal class CatalogElement : IDisposable
{
    public string? Value { get; }

    public CatalogElement? Next { get; set; }
    public CatalogElement? Previous { get; set; }

    private Mutex Mutex { get; } = new(false);

    private bool _disposed;

    public CatalogElement(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Value = value;
    }

    public void Lock()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));

        Mutex.WaitOne();
    }

    public void Unlock()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));

        Mutex.ReleaseMutex();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        Mutex.Dispose();
    }
}