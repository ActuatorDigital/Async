namespace AIR.Async
{
    public delegate void ProgressHandler(float progress);

    public interface IProgressHandle
    {
        void Progress(float progress);
    }
}