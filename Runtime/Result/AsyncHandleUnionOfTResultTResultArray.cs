using System.Collections.Generic;
using System.Linq;

namespace AIR.Async
{
    public class AsyncHandleUnion<TResult, TResultArray> : IAsync<TResultArray>
        where TResultArray : IEnumerable<TResult>
    {
        private readonly AsyncToCombine[] _asyncs;

        private event CatchHandler _catchHandler;

        public AsyncHandleUnion(IEnumerable<IAsync<TResult>> jobs)
        {
            var asyncsToCombines = new List<AsyncToCombine>();
            foreach (var job in jobs)
                asyncsToCombines.Add(new AsyncToCombine(job));

            _asyncs = asyncsToCombines.ToArray();
            foreach (var item in _asyncs)
                item.Async.Catch((e) => _catchHandler.Invoke(e));
        }

        public void Catch(CatchHandler catchHandler) => _catchHandler += catchHandler;

        public IAsync<TResultArray> Then(ThenHandler<TResultArray> then)
        {
            if (_asyncs.Length == 0)
            {
                then.Invoke((TResultArray)Enumerable.Empty<TResult>());
            }
            else
            {
                foreach (var asyncToCombine in _asyncs)
                {
                    asyncToCombine.Async.Then((r) => TryUnifiedComplete(asyncToCombine, r, then));
                }
            }

            return this;
        }

        private void TryUnifiedComplete(
            AsyncToCombine asyncToCombine,
            TResult resourceResult,
            ThenHandler<TResultArray> then)
        {
            asyncToCombine.Complete = true;
            asyncToCombine.Result = resourceResult;

            bool allComplete = true;
            foreach (var job in _asyncs)
                allComplete &= job.Complete;
            if (!allComplete) return;

            var results = new TResult[_asyncs.Length];
            for (var i = 0; i < _asyncs.Length; i++)
                results[i] = _asyncs[i].Result;

            var resultsEnumerable = (TResultArray)results.AsEnumerable();
            then?.Invoke(resultsEnumerable);
        }

        private class AsyncToCombine
        {
            public readonly IAsync<TResult> Async;

            public AsyncToCombine(IAsync<TResult> asyncWithResult)
            {
                Async = asyncWithResult;
                Complete = false;
            }

            public bool Complete { get; set; }
            public TResult Result { get; set; }
        }
    }
}