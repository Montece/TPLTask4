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

        var sequence = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();
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

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();
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
    public void Sort_EmptyCatalog_ShouldNotCallLogger()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Sort();
        catalog.Show();

        logger.Verify(x => x(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void Sort_OneItem_ShouldKeepSingleItem()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("only");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();

        Assert.Single(logged);
        Assert.Equal("only", logged.First());
    }

    [Fact]
    public void Sort_TwoItems_AlreadySorted_ShouldKeepOrder()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("alpha");
        catalog.Add("bravo");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();

        Assert.Equal(new[] { "alpha", "bravo" }, logged);
    }

    [Fact]
    public void Sort_TwoItems_Reversed_ShouldSwapToAscending()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("bravo");
        catalog.Add("alpha");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();
        Assert.Equal(new[] { "alpha", "bravo" }, logged);
    }

    [Fact]
    public void Sort_TwoItems_DifferentCasing_ShouldSortIgnoreCase()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("Bravo");
        catalog.Add("alpha");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();
        Assert.Equal(new[] { "alpha", "Bravo" }, logged);
    }

    [Fact]
    public void Sort_ItemsWithSameValue_ShouldNotLoseOrDuplicateItems()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("same");
        catalog.Add("same");
        catalog.Add("same");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();

        Assert.Equal(3, logged.Length);
        Assert.All(logged, v => Assert.Equal("same", v));
    }

    [Fact]
    public void Sort_ItemsWithSameValueDifferentCase_ShouldTreatThemAsEqual()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("Alpha");
        catalog.Add("alpha");
        catalog.Add("ALPHA");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();

        Assert.Equal(3, logged.Length);
        Assert.Contains("Alpha", logged);
        Assert.Contains("alpha", logged);
        Assert.Contains("ALPHA", logged);
    }

    [Fact]
    public void Sort_AlreadySortedManyItems_ShouldKeepOrder()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("delta");
        catalog.Add("charlie");
        catalog.Add("bravo");
        catalog.Add("alpha");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();

        Assert.Equal(new[] { "alpha", "bravo", "charlie", "delta" }, logged);
    }

    [Fact]
    public void Sort_ReverseSortedManyItems_ShouldBeSortedAscending()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("alpha");
        catalog.Add("bravo");
        catalog.Add("charlie");
        catalog.Add("delta");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();

        Assert.Equal(new[] { "alpha", "bravo", "charlie", "delta" }, logged);
    }

    [Fact]
    public void Sort_MixedOrderWithDuplicates_ShouldSortAndPreserveCount()
    {
        var logger = new Mock<Action<string>>();
        var catalog = new Catalog(logger.Object);

        catalog.Add("beta");
        catalog.Add("alpha");
        catalog.Add("beta");
        catalog.Add("gamma");

        catalog.Sort();
        catalog.Show();

        var logged = logger.Invocations.Select(i => i.Arguments.First().ToString()).ToArray();

        Assert.Equal(4, logged.Length);
        Assert.Equal(new[] { "alpha", "beta", "beta", "gamma" }, logged);
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