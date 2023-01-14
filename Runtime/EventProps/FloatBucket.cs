using UnityEngine;

namespace Crysc.EventProps
{
    public class FloatBucket : Bucket<float>
    {
        public FloatBucket(float value) : base(value: value) { }

        public void ChangeValue(float value) { SetValue(Current + value); }

        public override void SetValue(float value)
        {
            value = Mathf.Clamp(value: value, min: 0, max: Max);
            base.SetValue(value);
        }
    }
}
