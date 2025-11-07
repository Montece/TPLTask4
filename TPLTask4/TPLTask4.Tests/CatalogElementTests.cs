using Xunit;

namespace TPLTask4.Tests;

public class CatalogElementTests
{
    [Fact]
    public void Ctor_ShouldSetValue()
    {
        var element = new CatalogElement("hello");
        Assert.Equal("hello", element.Value);
    }

    [Fact]
    public void LockUnlock_ShouldNotThrow()
    {
        var element = new CatalogElement("x");

        element.Lock();
        element.Unlock();
    }

    [Fact]
    public void Lock_AfterDispose_ShouldThrow()
    {
        var element = new CatalogElement("x");
        element.Dispose();

        Assert.Throws<ObjectDisposedException>(element.Lock);
    }

    [Fact]
    public void Unlock_AfterDispose_ShouldThrow()
    {
        var element = new CatalogElement("x");
        element.Dispose();

        Assert.Throws<ObjectDisposedException>(element.Unlock);
    }
}