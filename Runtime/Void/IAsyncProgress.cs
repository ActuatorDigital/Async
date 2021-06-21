namespace AIR.Async
{
    public interface IAsyncProgress
    {
        IAsync Progressed(ProgressHandler progressed);
    }
}