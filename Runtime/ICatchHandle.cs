namespace AIR.Async
{
    //TODO want a way to signal that the async is done but failed, a reject/fail call equiv
    public delegate void CatchHandler(System.Exception exception);

    public interface ICatchHandle
    {
        void Catch(CatchHandler catchHandler);
    }
}