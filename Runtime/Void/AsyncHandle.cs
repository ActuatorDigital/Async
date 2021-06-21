using System;
using System.Collections.Generic;

namespace AIR.Async
{
    public class AsyncHandle : IAsync, ICompleteHandle
    {
        protected readonly Queue<ThenHandler> _asyncBuffer = new Queue<ThenHandler>();
        private bool _complete;

        protected event CatchHandler _catchHandler;

        public IAsync Then(ThenHandler then)
        {
            if (_complete)
                then?.Invoke();
            else
                _asyncBuffer.Enqueue(then);

            return this;
        }

        public void Complete()
        {
            try
            {
                while (_asyncBuffer.Count > 0)
                    _asyncBuffer.Dequeue()?.Invoke();
                _complete = true;
            }
            catch (Exception e)
            {
                DoCatch(e);
            }
        }

        protected void DoCatch(Exception e)
        {
            if (_catchHandler != null)
                _catchHandler?.Invoke(e);
            else
                throw e;
        }

        public void Catch(CatchHandler catchHandler) => _catchHandler += catchHandler;
    }
}