namespace AIR.Async
{
    public delegate void ThenHandler<in TResult>(TResult result);

    public interface IAsync<TResult> : ICatchHandle
    {
        IAsync<TResult> Then(ThenHandler<TResult> then);
    }
}