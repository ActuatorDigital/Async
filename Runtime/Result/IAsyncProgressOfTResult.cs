namespace AIR.Async
{
    public interface IAsyncProgress<TResult>
    {
        IAsync<TResult> Progressed(ProgressHandler progressed);
    }
}