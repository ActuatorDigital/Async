namespace AIR.Async
{
    public class AsyncProgressHandle<TResult> : AsyncHandle<TResult>, IAsyncProgress<TResult>, IProgressHandle
    {
        private ProgressHandler _progressed;

        public IAsync<TResult> Progressed(ProgressHandler progressed)
        {
            _progressed = progressed;
            return this;
        }

        public void Progress(float progress) => _progressed?.Invoke(progress);
    }
}