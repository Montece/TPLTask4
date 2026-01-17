namespace TPLTask4;

internal sealed class Catalog : IDisposable
{
    private bool _disposed;
    private bool _isSorting;
    private Thread? _sortThread;
    private readonly Action<string> _logMethod;
    private readonly ManualResetEventSlim _sortEvent = new(false);
    private readonly CatalogElement _headSentinel;
    private readonly CatalogElement _tailSentinel;

    public Catalog(Action<string> logMethod)
    {
        ArgumentNullException.ThrowIfNull(logMethod);

        _logMethod = logMethod;
        
        CatalogElement.CreateSentinels(out _headSentinel, out _tailSentinel);
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
        
        _headSentinel.Lock();
        var realElement = _headSentinel.Next;
        realElement.Lock();
        
        try
        {
            var newElement = new CatalogElement(newValue, realElement, _headSentinel);
            
            if (realElement != _headSentinel)
            {
                realElement.Previous = newElement;
            }
            
            _headSentinel.Next = newElement;
        }
        finally
        {
            realElement.Unlock();
            _headSentinel.Unlock();
        }
    }

    public void Sort()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));

        var sorted = false;

        while (!sorted)
        {
            sorted = true;

            _headSentinel.Lock();
            var current = _headSentinel.Next;
            
            try
            {
                if (current == _tailSentinel)
                {
                    return;
                }
                
                current.Lock();
            }
            finally
            {
                _headSentinel.Unlock();
            }

            while (current != _tailSentinel)
            {
                var next = current.Next;
                
                try
                {
                    if (next == _tailSentinel)
                    {
                        current.Unlock();
                        break;
                    }
                    
                    next.Lock();
                }
                catch
                {
                    current.Unlock();
                    throw;
                }

                try
                {
                    if (string.Compare(current.Value, next.Value, StringComparison.CurrentCultureIgnoreCase) > 0)
                    {
                        var previous = current.Previous;
                        var nextNext = next.Next;
                        
                        previous.Lock();
                        if (nextNext != _tailSentinel)
                        {
                            nextNext.Lock();
                        }

                        try
                        {
                            next.Previous = previous;
                            next.Next = current;
                            current.Previous = next;
                            current.Next = nextNext;

                            if (nextNext != _tailSentinel)
                            {
                                nextNext.Previous = current;
                            }

                            previous.Next = next;

                            sorted = false;
                        }
                        finally
                        {
                            if (nextNext != _tailSentinel)
                            {
                                nextNext.Unlock();
                            }
                            previous.Unlock();
                        }
                        
                        next.Unlock();
                    }
                    else
                    {
                        current.Unlock();
                        current = next;
                    }
                }
                catch
                {
                    next.Unlock();
                    current.Unlock();
                    throw;
                }
            }
        }
    }

    private IEnumerable<CatalogElement> GetAllElements()
    {
        ObjectDisposedException.ThrowIf(_disposed, typeof(Catalog));

        _headSentinel.Lock();
        CatalogElement cursor;
        
        try
        {
            cursor = _headSentinel.Next;
        }
        finally
        {
            _headSentinel.Unlock();
        }

        while (cursor != _tailSentinel)
        {
            cursor.Lock();

            yield return cursor;

            var next = cursor.Next;
            cursor.Unlock();

            cursor = next;
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

        var cursor = _headSentinel.Next;

        while (cursor != _tailSentinel)
        {
            var next = cursor.Next;

            cursor.Dispose();

            cursor = next;
        }
        
        _headSentinel.Dispose();
        _tailSentinel.Dispose();
    }
}