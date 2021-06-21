namespace AIR.Async
{
    public delegate void ThenHandler();

    public interface IAsync : ICatchHandle
    {
        IAsync Then(ThenHandler then);
    }
}