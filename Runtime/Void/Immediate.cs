namespace AIR.Async
{
    public class Immediate : IAsync
    {
        public IAsync Then(ThenHandler then)
        {
            then?.Invoke();
            return this;
        }

        public void Catch(CatchHandler _)
        { }// Method intentionally left empty.
    }
}