using System.Collections.Generic;
using System.Linq;

namespace AIR.Async
{
    public class AsyncHandleUnion : IAsync
    {
        private readonly Dictionary<IAsync, bool> _workingAsyncs = new Dictionary<IAsync, bool>();

        private event CatchHandler _catchHandler;

        public AsyncHandleUnion(params IAsync[] asyncs)
        {
            foreach (var async in asyncs)
            {
                _workingAsyncs.Add(async, false);
                async.Catch((e) => _catchHandler?.Invoke(e));
            }
        }

        public void Catch(CatchHandler catchHandler) => _catchHandler += catchHandler;

        public IAsync Then(ThenHandler then)
        {
            if (_workingAsyncs.Count == 0)
            {
                then.Invoke();
            }
            else
            {
                foreach (var async in _workingAsyncs.Keys.ToArray())
                    async.Then(() => TryFinishAll(async, then));
            }
            return this;
        }

        private void TryFinishAll(IAsync async, ThenHandler then)
        {
            _workingAsyncs[async] = true;
            foreach (var a in _workingAsyncs)
                if (!a.Value) return;

            then?.Invoke();
        }
    }
}