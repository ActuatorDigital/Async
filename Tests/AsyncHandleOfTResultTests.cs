using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class AsyncHandleOfTResultTests
{
    [Test]
    public void Catch_ExceptionThrown_Called()
    {
        // Arrange
        bool catchHit = false;
        var async = CreateAsyncObject();

        // Act
        async.Then((b) => throw new System.Exception());
        async.Catch((e) => catchHit = true);
        async.Complete(true);

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
        async.Then((b) => thenInvoked = true);
        async.Then((b) => throw new System.Exception());
        try
        {
            async.Complete(true);
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
        async.Then((b) => throw new System.Exception());
        async.Then((b) => thenInvoked = true);
        try
        {
            async.Complete(true);
        }
        catch (System.Exception)
        {
            // Eat silently, we care about the correct flow of thens in this test
        }

        // Assert
        Assert.IsFalse(thenInvoked, "Then should not have been invoked, but has been.");
    }

    [Test]
    public void Then_HandleAlreadyComplete_DoesInvoke()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Complete(true);
        async.Then((result) => thenInvoked = result);

        // Assert
        Assert.IsTrue(thenInvoked, "Then was not invoked. But should have been as task was already complete.");
    }

    [Test]
    public void Then_HandleDeliveredAfter_Invokes()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Complete(true);
        async.Then((result) => thenInvoked = result);

        // Assert
        Assert.IsTrue(thenInvoked, "Then should have been invoked with result, but was not.");
    }

    [Test]
    public void Then_HandleDeliveredBefore_Invokes()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Then((result) => thenInvoked = result);
        async.Complete(true);

        // Assert
        Assert.IsTrue(thenInvoked, "Then should have been invoked with result, but was not.");
    }

    [Test]
    public void Then_HandleNotDelivered_DoesNotInvoke()
    {
        // Arrange
        bool thenInvoked = false;
        var async = CreateAsyncObject();

        // Act
        async.Then((result) => thenInvoked = result);

        // Assert
        Assert.IsFalse(thenInvoked, "Then was invoked, but should not have been.");
    }

    [Test]
    public void Then_MutlipleDelivieries_AllInvoked()
    {
        // Arrange
        const int EXPECTED_INVOCATIONS = 10;
        int actualInvocations = 0;
        var async = CreateAsyncObject();

        // Act
        async.Then((result) => actualInvocations++);
        for (int i = 0; i < EXPECTED_INVOCATIONS; i++)
            async.Complete(true);

        // Assert
        var message = $"The was not called the expected times ({EXPECTED_INVOCATIONS}).";
        Assert.AreEqual(EXPECTED_INVOCATIONS, actualInvocations, message);
    }

    protected virtual AsyncHandle<bool> CreateAsyncObject() => new AsyncHandle<bool>();
}