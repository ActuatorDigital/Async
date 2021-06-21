namespace AIR.Async
{
    public class AsyncProgressHandle : AsyncHandle, IAsyncProgress, IProgressHandle
    {
        private ProgressHandler _progressed;

        public IAsync Progressed(ProgressHandler progressed)
        {
            _progressed = progressed;
            return this;
        }

        public void Progress(float progress) => _progressed?.Invoke(progress);
    }
}