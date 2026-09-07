#region

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
    }
}
