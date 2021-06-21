using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class ImmediateOfTResultTests
{
    [Test]
    public void Then_HandleSet_DoesInvokeImmediately()
    {
        // Arrange
        bool thenInvoked = false;
        bool immediateResultParamGiven = false;
        const bool ImmediateResultParam = true;
        var async = new Immediate<bool>(ImmediateResultParam);

        // Act
        async.Then((x) => { immediateResultParamGiven = x; thenInvoked = true; });

        // Assert
        Assert.IsTrue(thenInvoked, "Then was not invoked, but should have been.");
        Assert.AreEqual(ImmediateResultParam, immediateResultParamGiven, "Immediate result Param does not match what it was given.");
    }
}