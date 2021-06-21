namespace AIR.Async
{
    public class Immediate<TResult> : IAsync<TResult>
    {
        private readonly TResult _result;

        public Immediate(TResult result) => _result = result;

        public IAsync<TResult> Then(ThenHandler<TResult> then)
        {
            then?.Invoke(_result);
            return this;
        }

        public void Catch(CatchHandler _)
        { }// Method intentionally left empty.
    }
}