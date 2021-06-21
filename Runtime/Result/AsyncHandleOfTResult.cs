using System;
using System.Collections.Generic;

namespace AIR.Async
{
    public class AsyncHandle<TResult> : IAsync<TResult>, ICompleteHandle<TResult>
    {
        private readonly List<ThenHandler<TResult>> _asyncBuffer = new List<ThenHandler<TResult>>();
        private bool _completed;
        private TResult _result;

        private event CatchHandler _catchHandler;

        public IAsync<TResult> Then(ThenHandler<TResult> then)
        {
            if (_completed)
                then?.Invoke(_result);
            else
                _asyncBuffer.Add(then);

            return this;
        }

        public void Complete(TResult result)
        {
            try
            {
                foreach (var thenHandler in _asyncBuffer)
                    thenHandler?.Invoke(result);

                _result = result;
                _completed = true;
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