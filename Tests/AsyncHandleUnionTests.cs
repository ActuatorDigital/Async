using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class AsyncHandleUnionTests
{
    [Test]
    public void Then_NoDependenciesAdded_DoesInvoke()
    {
        // Arranges
        var mockAsyncHandles = new AsyncHandle[] { };
        bool completed = false;

        // Act
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Then(() => completed = true);

        // Assert
        Assert.IsTrue(completed, "No handles added, but union did not complete.");
    }

    [Test]
    public void Then_NoCompleteDependencies_DoesNotInvoke()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle();
        var mockAsyncHandleTwo = new AsyncHandle();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool completed = false;

        // Act
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Then(() => completed = true);

        // Assert
        Assert.IsFalse(completed, "No handles completed, but union did.");
    }

    [Test]
    public void Then_OneDependencyCompletes_DoesNotInvoke()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle();
        var mockAsyncHandleTwo = new AsyncHandle();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool completed = false;

        // Act
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Then(() => completed = true);
        mockAsyncHandleOne.Complete();

        // Assert
        Assert.IsFalse(completed, "Not all dependencies completed, but union did.");
    }

    [Test]
    public void Then_AllDependencyCompleted_InvokesForEachHandle()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle();
        var mockAsyncHandleTwo = new AsyncHandle();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };

        bool isComplete = false;
        const bool ExpectedResult = true;

        // Act
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Then(() => isComplete = ExpectedResult);
        foreach (var mockAsyncHandle in mockAsyncHandles)
            mockAsyncHandle.Complete();

        // Assert
        Assert.AreEqual(ExpectedResult, isComplete, "All dependencies completed, but union did not.");
    }

    [Test]
    public void Then_DependencyCompletesLate_InvokesAfterLastCompletes()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle();
        var mockAsyncHandleTwo = new AsyncHandle();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool isComplete = false;
        const bool ExpectedResultAfterDone = true;
        const bool ExpectedResultBeforeDone = false;

        // Act
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Then(() => isComplete = true);
        mockAsyncHandleOne.Complete();

        // Assert
        Assert.AreEqual(ExpectedResultBeforeDone, isComplete, "Only one dependency completed, but union acted as if all where done.");
        mockAsyncHandleTwo.Complete();
        Assert.AreEqual(ExpectedResultAfterDone, isComplete, "All dependencies completed, but union did not.");
    }

    [Test]
    public void Then_MultipleDependenciesAddedMultipleThensGiven_DoesInvokeAllThenHandlers()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle();
        var mockAsyncHandleTwo = new AsyncHandle();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool isComplete = false;
        int isCompleteCount = 0;
        const bool ExpectedComplete = true;
        const int ExpectedCount = 1;

        // Act
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Then(() => isComplete = true);
        asyncUnion.Then(() => isCompleteCount++);
        mockAsyncHandleOne.Complete();
        mockAsyncHandleTwo.Complete();

        // Assert
        Assert.AreEqual(ExpectedComplete, isComplete, "The first 'then' was not called but should have been.");
        Assert.AreEqual(ExpectedCount, isCompleteCount, "Then was not called the correct number of times.");
    }

    [Test]
    public void Then_AlreadyCompleteDependenciesLateBound_DoesInvoke()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle();
        var mockAsyncHandleTwo = new AsyncHandle();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        int isCompleteCount = 0;
        const int ExpectedCount = 1;

        // Act
        mockAsyncHandleOne.Complete();
        mockAsyncHandleTwo.Complete();
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Then(() => isCompleteCount++);

        // Assert
        Assert.AreEqual(ExpectedCount, isCompleteCount, "Then was not called the correct number of times.");
    }

    [Test]
    public void Catch_InnerAsyncThrows_DoesCall()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle();
        var mockAsyncHandleTwo = new AsyncHandle();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool isCalled = false;

        // Act
        var asyncUnion = new AsyncHandleUnion(mockAsyncHandles);
        asyncUnion.Catch((e) => isCalled = true);
        mockAsyncHandleTwo.Then(() => throw new System.Exception());
        mockAsyncHandleOne.Complete();
        mockAsyncHandleTwo.Complete();

        // Assert
        Assert.IsTrue(isCalled, "The Union's Catch was not called but should have been.");
    }
}