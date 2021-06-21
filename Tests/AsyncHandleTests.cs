using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class AsyncHandleTests
{
    [Test]
    public void Catch_ExceptionThrown_Called()
    {
        // Arrange
        bool catchHit = false;
        var async = CreateAsyncObject();

        // Act
        async.Then(() => throw new System.Exception());
        async.Catch((e) => catchHit = true);
        async.Complete();

        // Assert
        Assert.IsTrue(catchHit, "Catch should have been called, but was not.");
    }

    [Test]
    public void Then_ExceptionThrownAfter_Invoke()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Then(() => thenInvoked = true);
        async.Then(() => throw new System.Exception());
        try
        {
            async.Complete();
        }
        catch (System.Exception)
        {
            // Eat silently, we care about the correct flow of thens in this test
        }

        // Assert
        Assert.IsTrue(thenInvoked, "Then should have been invoked, but was not.");
    }

    [Test]
    public void Then_ExceptionThrownBefore_DoesNotInvoke()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Then(() => throw new System.Exception());
        async.Then(() => thenInvoked = true);
        try
        {
            async.Complete();
        }
        catch (System.Exception)
        {
            // Eat silently, we care about the correct flow of thens in this test
        }

        // Assert
        Assert.IsFalse(thenInvoked, "Then should not have been invoked, but has been.");
    }

    [Test]
    public void Then_HandleDeliveredAfter_Invokes()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Then(() => thenInvoked = true);
        //TODO: kinda hate that you cannot fluent them
        async.Complete();

        // Assert
        Assert.IsTrue(thenInvoked, "Then should have been invoked, but was not.");
    }

    [Test]
    public void Then_HandleDeliveredBefore_Invokes()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Complete();
        async.Then(() => thenInvoked = true);

        // Assert
        Assert.IsTrue(thenInvoked, "Then should have been invoked, but was not.");
    }

    [Test]
    public void Then_HandleNotDelivered_DoesNotInvoke()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Then(() => thenInvoked = true);

        // Assert
        Assert.IsFalse(thenInvoked, "Then was invoked, but should not have been.");
    }

    protected virtual AsyncHandle CreateAsyncObject() => new AsyncHandle();
}