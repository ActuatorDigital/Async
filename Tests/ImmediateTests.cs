using AIR.Async;
using NUnit.Framework;

[TestFixture]
public class ImmediateTests
{
    [Test]
    public void Then_HandleSet_DoesInvokeImmediately()
    {
        // Arrange
        bool thenInvoked = false;
        var async = new Immediate();

        // Act
        async.Then(() => thenInvoked = true);

        // Assert
        Assert.IsTrue(thenInvoked, "Then was not invoked, but should have been.");
    }
}