using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class AsyncHandleUnionOfTResultTests
{
    [Test]
    public void Then_NoDependenciesAdded_DoesInvoke()
    {
        // Arranges
        var mockAsyncHandles = new AsyncHandle<bool>[] { };
        bool completed = false;

        // Act
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Then((r) => completed = true);

        // Assert
        Assert.IsTrue(completed, "No handles added, but union did not complete.");
    }

    [Test]
    public void Then_NoCompleteDependencies_DoesNotInvoke()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle<bool>();
        var mockAsyncHandleTwo = new AsyncHandle<bool>();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool completed = false;

        // Act
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Then((r) => completed = true);

        // Assert
        Assert.IsFalse(completed, "No handles completed, but union did.");
    }

    [Test]
    public void Then_OneDependencyCompletes_DoesNotInvoke()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle<bool>();
        var mockAsyncHandleTwo = new AsyncHandle<bool>();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool completed = false;

        // Act
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Then((r) => completed = true);
        mockAsyncHandleOne.Complete(true);

        // Assert
        Assert.IsFalse(completed, "Not all dependencies completed, but union did.");
    }

    [Test]
    public void Then_AllDependencyCompleted_InvokesForEachHandle()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle<bool>();
        var mockAsyncHandleTwo = new AsyncHandle<bool>();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        int resultArrayLength = 0;

        // Act
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Then((r) => resultArrayLength = r.Length);
        foreach (var mockAsyncHandle in mockAsyncHandles)
            mockAsyncHandle.Complete(true);

        // Assert
        Assert.AreEqual(resultArrayLength, mockAsyncHandles.Length, "All dependencies completed, but union did not.");
    }

    [Test]
    public void Then_DependencyCompletesLate_InvokesAfterLastCompletes()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle<bool>();
        var mockAsyncHandleTwo = new AsyncHandle<bool>();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        int resultArrayLength = 0;
        const int ExpectedResultArrayLengthMockOne = 0;
        const int ExpectedResultArrayLengthMockTwo = 2;

        // Act
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Then((r) => resultArrayLength = r.Length);
        mockAsyncHandleOne.Complete(true);

        // Assert
        Assert.AreEqual(ExpectedResultArrayLengthMockOne, resultArrayLength, "Only one dependency completed, but union acted as if all where done.");
        mockAsyncHandleTwo.Complete(true);
        Assert.AreEqual(ExpectedResultArrayLengthMockTwo, resultArrayLength, "All dependencies completed, but union did not.");
    }

    [Test]
    public void Then_MultipleDependenciesAddedMultipleThensGiven_DoesInvokeAllThenHandlers()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle<bool>();
        var mockAsyncHandleTwo = new AsyncHandle<bool>();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool isComplete = false;
        int isCompleteCount = 0;
        const bool ExpectedComplete = true;
        const int ExpectedCount = 1;

        // Act
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Then((r) => isComplete = true);
        asyncUnion.Then((r) => isCompleteCount++);
        mockAsyncHandleOne.Complete(true);
        mockAsyncHandleTwo.Complete(true);

        // Assert
        Assert.AreEqual(ExpectedComplete, isComplete, "The first 'then' was not called but should have been.");
        Assert.AreEqual(ExpectedCount, isCompleteCount, "Then was not called the correct number of times.");
    }

    [Test]
    public void Then_AlreadyCompleteDependenciesLateBound_DoesInvoke()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle<bool>();
        var mockAsyncHandleTwo = new AsyncHandle<bool>();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        int isCompleteCount = 0;
        const int ExpectedCount = 1;

        // Act
        mockAsyncHandleOne.Complete(true);
        mockAsyncHandleTwo.Complete(true);
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Then((r) => isCompleteCount++);

        // Assert
        Assert.AreEqual(ExpectedCount, isCompleteCount, "Then was not called the correct number of times.");
    }

    [Test]
    public void Catch_InnerAsyncThrows_DoesCall()
    {
        // Arrange
        var mockAsyncHandleOne = new AsyncHandle<bool>();
        var mockAsyncHandleTwo = new AsyncHandle<bool>();
        var mockAsyncHandles = new[] { mockAsyncHandleOne, mockAsyncHandleTwo };
        bool isCalled = false;

        // Act
        var asyncUnion = new AsyncHandleUnion<bool, bool[]>(mockAsyncHandles);
        asyncUnion.Catch((e) => isCalled = true);
        mockAsyncHandleTwo.Then((r) => throw new System.Exception());
        mockAsyncHandleOne.Complete(true);
        mockAsyncHandleTwo.Complete(true);

        // Assert
        Assert.IsTrue(isCalled, "The Union's Catch was not called but should have been.");
    }
}