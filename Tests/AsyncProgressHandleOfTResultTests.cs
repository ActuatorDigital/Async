using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class AsyncProgressHandleOfTResultTests : AsyncHandleOfTResultTests
{
    protected override AsyncHandle<bool> CreateAsyncObject()
    {
        return new AsyncProgressHandle<bool>();
    }

    [Test]
    public void Progress_CompleteWithoutProgressCall_DoesNotCallProgress()
    {
        // Arrange
        const float ExpectedReturnedProgress = -1;
        float returnedProgress = -1;
        var async = new AsyncProgressHandle<bool>();

        // Act
        async.Progressed((x) => returnedProgress = x);
        async.Complete(true);

        // Assert
        Assert.AreEqual(ExpectedReturnedProgress, returnedProgress);
    }

    [Test]
    public void Progress_WithHandleButNoComplete_DoesNotCallProgress()
    {
        // Arrange
        const float ExpectedReturnedProgress = -1;
        float returnedProgress = -1;
        var async = new AsyncProgressHandle<bool>();

        // Act
        async.Progressed((x) => returnedProgress = x)
            .Then(delegate { });

        // Assert
        Assert.AreEqual(ExpectedReturnedProgress, returnedProgress);
    }

    [Test]
    public void Progress_WithHandleAndCompleteButNoProgressCall_DoesNotCallProgress()
    {
        // Arrange
        const float ExpectedReturnedProgress = -1;
        float returnedProgress = -1;
        var async = new AsyncProgressHandle<bool>();

        // Act
        async.Progressed((x) => returnedProgress = x)
            .Then(delegate { });
        async.Complete(true);

        // Assert
        Assert.AreEqual(ExpectedReturnedProgress, returnedProgress);
    }

    [Test]
    public void Progress_WithHandleAndCompleteAndProgressCall_DoesUpdateProgress()
    {
        // Arrange
        const float ExpectedReturnedProgress = 1;
        float returnedProgress = -1;
        var async = new AsyncProgressHandle<bool>();
        IProgressHandle progressHandle = async;

        // Act
        async.Progressed((x) => returnedProgress = x)
            .Then((r) => progressHandle.Progress(ExpectedReturnedProgress));
        async.Complete(true);

        // Assert
        Assert.AreEqual(ExpectedReturnedProgress, returnedProgress);
    }
}