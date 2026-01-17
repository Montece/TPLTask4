using Xunit;

namespace TPLTask4.Tests;

public sealed class CatalogElementTests
{
    [Fact]
    public void Ctor_ShouldSetValue()
    {
        CatalogElement? head = null;
        CatalogElement? tail = null;

        try
        {
            CatalogElement.CreateSentinels(out head, out tail);

            using var element = new CatalogElement("hello", head, tail);
            Assert.Equal("hello", element.Value);
        }
        finally
        {
            head?.Dispose();
            tail?.Dispose();
        }
    }

    [Fact]
    public void LockUnlock_ShouldNotThrow()
    {
        CatalogElement? head = null;
        CatalogElement? tail = null;

        try
        {
            CatalogElement.CreateSentinels(out head, out tail);

            using var element = new CatalogElement("x", head, tail);

            element.Lock();
            element.Unlock();
        }
        finally
        {
            head?.Dispose();
            tail?.Dispose();
        }
    }

    [Fact]
    public void Lock_AfterDispose_ShouldThrow()
    {
        CatalogElement? head = null;
        CatalogElement? tail = null;

        try
        {
            CatalogElement.CreateSentinels(out head, out tail);

            var element = new CatalogElement("x", head, tail);
            element.Dispose();

            Assert.Throws<ObjectDisposedException>(element.Lock);
        }
        finally
        {
            head?.Dispose();
            tail?.Dispose();
        }
    }

    [Fact]
    public void Unlock_AfterDispose_ShouldThrow()
    {
        CatalogElement? head = null;
        CatalogElement? tail = null;

        try
        {
            CatalogElement.CreateSentinels(out head, out tail);

            var element = new CatalogElement("x", head, tail);
            element.Dispose();

            Assert.Throws<ObjectDisposedException>(element.Unlock);
        }
        finally
        {
            head?.Dispose();
            tail?.Dispose();
        }
    }
}