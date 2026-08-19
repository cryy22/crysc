#region

using PrimeTween;

#endregion

namespace Crysc.Helpers
{
    public readonly struct AnimationHandle
    {
        private readonly Tween _tween;

        public bool IsRunning => _tween.isAlive;

        public AnimationHandle(Tween tween)
        {
            _tween = tween;
        }

        public void Stop()
        {
            _tween.Stop();
        }
    }
}
