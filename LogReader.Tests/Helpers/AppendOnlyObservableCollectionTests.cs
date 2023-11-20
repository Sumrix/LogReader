using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Diagnostics;
using LogReader.Core.Helpers;

namespace LogReader.Tests.Helpers;

[TestFixture]
public class AppendOnlyObservableCollectionTests
{
    private const string IndexerName = "Item[]";

    [Test]
    public void AddRange_AddsItemsCorrectly()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();
        var itemsToAdd = new[] { 1, 2, 3, 4, 5 };

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        Assert.That(collection, Has.Count.EqualTo(itemsToAdd.Length));
        CollectionAssert.AreEqual(itemsToAdd, collection.ToList());
    }

    [Test]
    public void AddRange_RaisesChangedEvents()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>(new[] { 1, 2 });
        var itemsToAdd = new[] { 3, 4, 5 };
        var addRaised = new TaskCompletionSource<bool>();
        var countRaised = new TaskCompletionSource<bool>();
        var indexerRaised = new TaskCompletionSource<bool>();
        IList? addedItems = null;
        var addedIndex = 0;

        collection.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                addedItems = e.NewItems;
                addedIndex = e.NewStartingIndex;
                addRaised.SetResult(true);
            }
        };

        collection.PropertyChanged += (_, e) =>
        {
            switch (e.PropertyName)
            {
                case nameof(collection.Count):
                    countRaised.SetResult(true);
                    break;
                case IndexerName:
                    indexerRaised.SetResult(true);
                    break;
            }
        };

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        Assert.Multiple(async () =>
        {
            Assert.That(await addRaised.Task, Is.True);
            Assert.That(await countRaised.Task, Is.True);
            Assert.That(await indexerRaised.Task, Is.True);
            CollectionAssert.AreEqual(itemsToAdd, addedItems);
            Assert.That(addedIndex, Is.EqualTo(2));
        });
    }

    [Test]
    public void AddRange_InParallelContext_AddsItemsCorrectly()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();
        var itemsToAdd = Enumerable.Range(1, 1_000_000).ToList();

        // Act
        Parallel.ForEach(itemsToAdd.Chunk(100), items => collection.AddRange(items));

        // Assert
        Assert.That(collection, Has.Count.EqualTo(itemsToAdd.Count));
        CollectionAssert.AreEquivalent(itemsToAdd, collection.ToList());
    }

    [Test]
    public async Task AddRange_InParallelContext_RaisesEventsCorrectly()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();
        var itemsToAdd = Enumerable.Range(1, 100).ToList();
        var eventsRaised = 0;
        var raisedAllEvents = new TaskCompletionSource<bool>();

        collection.CollectionChanged += (_, e) =>
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                Interlocked.Increment(ref eventsRaised);
                if (eventsRaised == itemsToAdd.Count)
                {
                    raisedAllEvents.SetResult(true);
                }
            }
        };

        // Act
        Parallel.ForEach(itemsToAdd, item => collection.AddRange(new[] { item }));

        // Assert
        Assert.That(await raisedAllEvents.Task, Is.True);
    }

    [Test]
    public void ParallelWriteAndIndexAccess_ShouldReturnCorrectItems()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>(Enumerable.Range(1, 5000).ToList());
        var itemsToAdd = Enumerable.Range(5001, 5000).ToList();
        var errors = new ConcurrentBag<string>();

        // Act
        Parallel.Invoke(
            () => collection.AddRange(itemsToAdd),
            () =>
            {
                for (var i = 0; i < collection.Count; i++)
                {
                    if (collection[i] != i + 1)
                    {
                        errors.Add($"Index {i} has incorrect value {collection[i]}.");
                    }
                }
            }
        );

        // Assert
        Assert.That(errors.IsEmpty, Is.True, $"Errors found during index access: {string.Join(", ", errors)}");
    }

    [Test]
    public void ParallelWriteAndEnumeration_ShouldEnumerateCorrectItems()
    {
        // Arrange
        var itemsToInit = Enumerable.Range(1, 1_000_000).ToList();
        var collection = new AppendOnlyObservableCollection<int>(itemsToInit);
        var itemsToAdd = Enumerable.Range(1_000_001, 2_000_000).ToList();
        var enumeratedItems = new List<int>();
        var enumerationCompleted = new TaskCompletionSource<bool>();

        // Act
        Parallel.Invoke(
            () => collection.AddRange(itemsToAdd),
            () =>
            {
                foreach (var item in collection)
                {
                    enumeratedItems.Add(item);
                }
                enumerationCompleted.SetResult(true);
            }
        );

        // Waiting for enumeration to complete
        enumerationCompleted.Task.Wait();

        // Assert
        CollectionAssert.AreEqual(itemsToInit, enumeratedItems, "Enumerated items do not match the collection.");
    }

    [Test]
    public void GetEnumerator_EnumeratesItemsCorrectly()
    {
        // Arrange
        var initItems = new[] { 1, 2, 3, 4, 5 };
        var collection = new AppendOnlyObservableCollection<int>(initItems);
        var index = 0;

        // Act & Assert
        foreach (var item in collection)
        {
            Assert.That(item, Is.EqualTo(initItems[index]));
            index++;
        }
    }

    [Test]
    public void Indexer_ReturnsCorrectItem()
    {
        // Arrange
        var initItems = new[] { 1, 2, 3, 4, 5 };
        var collection = new AppendOnlyObservableCollection<int>(initItems);

        // Act & Assert
        for (var i = 0; i < initItems.Length; i++)
        {
            Assert.That(collection[i], Is.EqualTo(initItems[i]));
        }
    }

    [Test]
    public void Count_ReturnsCorrectValue()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();
        var itemsToAdd = Enumerable.Range(1, 10).ToList();

        // Act
        collection.AddRange(itemsToAdd);

        // Assert
        Assert.That(collection, Has.Count.EqualTo(itemsToAdd.Count));
    }

    [Test]
    public void Indexer_AccessWithinBounds_Succeeds()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>(new[] { 1, 2, 3 });

        // Act
        var item = collection[1];

        // Assert
        Assert.That(item, Is.EqualTo(2));
    }

    [Test]
    public void Indexer_AccessOutOfBounds_ThrowsException()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>(new[] { 1, 2, 3 });

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            _ = collection[5];
        });
    }

    [Test]
    public void AddRange_NullCollection_ThrowsArgumentNullException()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();

        // Act
        Assert.Throws<ArgumentNullException>(() => collection.AddRange(null!));
    }

    [Test]
    public async Task AddRange_EmptyCollection_DoesNotRaiseEvents()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();
        var eventsRaised = false;

        collection.CollectionChanged += (_, _) => eventsRaised = true;

        // Act
        collection.AddRange(Enumerable.Empty<int>());

        await Task.Delay(1);

        // Assert
        Assert.That(eventsRaised, Is.False, "Events should not be raised for an empty collection.");
    }

    [Test]
    public void AddRange_LargeCollection_Performance()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();
        var largeCollection = Enumerable.Range(0, 10_000_000);

        // Act
        var stopwatch = Stopwatch.StartNew();
        collection.AddRange(largeCollection);
        stopwatch.Stop();

        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(300), "Performance issue: adding a large collection took too long.");
    }

    [Test]
    public void AddRange_HighFrequency_AddsItemsWithoutDelay()
    {
        // Arrange
        var collection = new AppendOnlyObservableCollection<int>();
        var highFrequencyItems = Enumerable.Range(0, 10_000);

        // Act
        var stopwatch = Stopwatch.StartNew();
        foreach (var item in highFrequencyItems)
        {
            collection.AddRange(new[] { item });
        }
        stopwatch.Stop();

        // Assert
        Assert.That(stopwatch.ElapsedMilliseconds, Is.LessThan(40), "Performance issue: adding items at a high frequency took too long.");
    }
}