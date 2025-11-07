using Moq;
using Xunit;

namespace TPLTask4.Tests;

public class CatalogTests
{
    [Fact]
    public void Add_ShouldPutNewItemToHead()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("B");
        catalog.Add("A");

        catalog.Show();

        logger.Verify(x => x("A"), Times.Once);
        logger.Verify(x => x("B"), Times.Once);

        var sequence = logger.Invocations.Select(i => i.Arguments[0].ToString()).ToArray();
        Assert.Equal(new[] { "A", "B" }, sequence);
    }

    [Fact]
    public void Sort_ShouldSortItemsAscendingIgnoreCase()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("delta");
        catalog.Add("Bravo");
        catalog.Add("alpha");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments[0].ToString()).ToArray();
        Assert.Equal(new[] { "alpha", "Bravo", "delta" }, logged);
    }

    [Fact]
    public void Add_Null_ShouldThrow()
    {
        var catalog = new Catalog(_ => { });

        Assert.Throws<ArgumentNullException>(() => catalog.Add(null!));
    }

    [Fact]
    public void Show_AfterDispose_ShouldThrow()
    {
        var catalog = new Catalog(_ => { });
        catalog.Add("x");
        catalog.Dispose();

        Assert.Throws<ObjectDisposedException>(catalog.Show);
    }

    [Fact]
    public void Sort_EmptyCatalog_ShouldNotThrow()
    {
        var catalog = new Catalog(_ => { });

        catalog.Sort();
    }

    [Fact]
    public void Dispose_ShouldDisposeAllElements()
    {
        var catalog = new Catalog(_ => { });
        catalog.Add("c");
        catalog.Add("b");
        catalog.Add("a");

        catalog.Dispose();

        Assert.Throws<ObjectDisposedException>(() => catalog.Add("zzz"));
    }
}