namespace TPLTask4;

internal class CatalogElement : IDisposable
{
    public string? Value { get; }

    public CatalogElement Next { get; set; }
    public CatalogElement Previous { get; set; }

    private Mutex Mutex { get; } = new(false);

    private bool _disposed;

    public CatalogElement(string value, CatalogElement next, CatalogElement previous)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(next);
        ArgumentNullException.ThrowIfNull(previous);

        Value = value;
        Next = next;
        Previous = previous;
    }

    private CatalogElement(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        Next = this;
        Previous = this;
        Value = value;
    }

    public static void CreateSentinels(out CatalogElement head, out CatalogElement tail)
    {
        head = new CatalogElement(string.Empty);
        tail = new CatalogElement(string.Empty);

        head.Next = tail;
        head.Previous = head;

        tail.Next = tail;
        tail.Previous = head;
    }

    public CatalogElement(string value, bool isHead, CatalogElement elementNextOrPrevious)
    {
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(elementNextOrPrevious);

        Value = value;
        Next = isHead ? elementNextOrPrevious : this;
        Previous = isHead ? this : elementNextOrPrevious;
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