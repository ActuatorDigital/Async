namespace AIR.Async
{
    public interface ICompleteHandle<in TResult>
    {
        void Complete(TResult result);
    }
}