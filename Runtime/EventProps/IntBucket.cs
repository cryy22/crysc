using UnityEngine;

namespace Crysc.EventProps
{
    public class IntBucket : Bucket<int>
    {
        public IntBucket(int value) : base(value: value) { }

        public void ChangeValue(int value) { SetValue(Current + value); }

        public override void SetValue(int value)
        {
            value = Mathf.Clamp(value: value, min: 0, max: Max);
            base.SetValue(value);
        }
    }
}
