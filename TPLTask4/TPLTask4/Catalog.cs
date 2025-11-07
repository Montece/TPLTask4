namespace TPLTask4;

internal sealed class Catalog : IDisposable
{
    private CatalogElement? Head { get; set; }

    private bool _disposed;
    private bool _isSorting;
    private Thread? _sortThread;
    private readonly Action<string> _logMethod;
    private readonly ManualResetEventSlim _sortEvent = new(false);

    public Catalog(Action<string> logMethod)
    {
        ArgumentNullException.ThrowIfNull(logMethod);

        _logMethod = logMethod;
    }

    public void Show()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));

        foreach (var element in GetAllElements())
        {
            if (element.Value is not null)
            {
                _logMethod(element.Value);
            }
        }
    }

    public void Add(string newValue)
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));
        ArgumentNullException.ThrowIfNull(newValue);

        var newElement = new CatalogElement(newValue);

        if (Head is null)
        {
            Head = newElement;
        }
        else
        {
            Head.Previous = newElement;
            newElement.Previous = null;
            newElement.Next = Head;
            Head = newElement;
        }
    }

    public void Sort()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));

        if (Head is null)
        {
            return;
        }

        var sorted = false;

        while (!sorted)
        {
            sorted = true;

            var current = Head;

            while (current?.Next is not null)
            {
                var next = current.Next;

                if (next is null)
                {
                    break;
                }

                if (string.Compare(current.Value, next.Value, StringComparison.CurrentCultureIgnoreCase) > 0)
                {
                    var lockedObjects = new[]
                    {
                        current,
                        current.Previous,
                        next,
                        next.Next
                    };

                    Array.ForEach(lockedObjects, x => x?.Lock());

                    var previous = current.Previous;
                    var nextNext = next.Next;

                    next.Previous = previous;
                    next.Next = current;
                    current.Previous = next;
                    current.Next = nextNext;

                    if (nextNext is not null)
                    {
                        nextNext.Previous = current;
                    }

                    if (previous is not null)
                    {
                        previous.Next = next;
                    }
                    else
                    {
                        Head = next;
                    }

                    sorted = false;

                    Array.ForEach(lockedObjects, x => x?.Unlock());
                }
                else
                {
                    current = next;
                }
            }
        }
    }

    private IEnumerable<CatalogElement> GetAllElements()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));

        var cursor = Head;

        while (cursor is not null)
        {
            cursor.Lock();

            yield return cursor;

            cursor.Unlock();

            cursor = cursor.Next;
        }
    }

    public void BeginSortProcess()
    {
        if (_isSorting)
        {
            return;
        }

        _isSorting = true;

        _sortEvent.Set();

        _sortThread = new(SortProcess);
        _sortThread.Start();
    }

    public void EndSortProcess()
    {
        if (!_isSorting)
        {
            return;
        }

        _isSorting = false;

        _sortEvent.Reset();

        _sortThread?.Join();

        _sortThread = null;
    }

    private void SortProcess()
    {
        while (true)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));

            if (_sortEvent.IsSet)
            {
                return;
            }

            Sort();

            if (_sortEvent.IsSet)
            {
                return;
            }
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        
        _sortEvent.Dispose();

        var cursor = Head;

        while (cursor is not null)
        {
            var next = cursor.Next;

            cursor.Dispose();

            cursor = next;
        }
    }
}