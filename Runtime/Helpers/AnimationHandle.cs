#region

using System;
using PrimeTween;

#endregion

namespace Crysc.Helpers
{
    public readonly struct AnimationHandle
    {
        private readonly Sequence _sequence;

        public bool IsRunning => _sequence.isAlive;

        public AnimationHandle(Sequence sequence)
        {
            _sequence = sequence;
        }

        public AnimationHandle(Tween tween)
        {
            _sequence = Sequence.Create().Chain(tween);
        }

        public void Stop()
        {
            _sequence.Stop();
        }

        public void ChainCallback<T>(T target, Action<T> action) where T : class
        {
            _sequence.ChainCallback(target: target, callback: action);
        }
    }
}
