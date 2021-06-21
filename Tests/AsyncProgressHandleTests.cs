using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class AsyncProgressHandleTests : AsyncHandleTests
{
    protected override AsyncHandle CreateAsyncObject()
    {
        return new AsyncProgressHandle();
    }

    [Test]
    public void Progress_CompleteWithoutProgressCall_DoesNotCallProgress()
    {
        // Arrange
        const float ExpectedReturnedProgress = -1;
        float returnedProgress = -1;
        var async = new AsyncProgressHandle();

        // Act
        async.Progressed((x) => returnedProgress = x);
        async.Complete();

        // Assert
        Assert.AreEqual(ExpectedReturnedProgress, returnedProgress);
    }

    [Test]
    public void Progress_WithHandleButNoComplete_DoesNotCallProgress()
    {
        // Arrange
        const float ExpectedReturnedProgress = -1;
        float returnedProgress = -1;
        var async = new AsyncProgressHandle();

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
        var async = new AsyncProgressHandle();

        // Act
        async.Progressed((x) => returnedProgress = x)
            .Then(delegate { });
        async.Complete();

        // Assert
        Assert.AreEqual(ExpectedReturnedProgress, returnedProgress);
    }

    [Test]
    public void Progress_WithHandleAndCompleteAndProgressCall_DoesUpdateProgress()
    {
        // Arrange
        const float ExpectedReturnedProgress = 1;
        float returnedProgress = -1;
        var async = new AsyncProgressHandle();
        IProgressHandle progressHandle = async;

        // Act
        async.Progressed((x) => returnedProgress = x)
            .Then(() => progressHandle.Progress(ExpectedReturnedProgress));
        async.Complete();

        // Assert
        Assert.AreEqual(ExpectedReturnedProgress, returnedProgress);
    }
}