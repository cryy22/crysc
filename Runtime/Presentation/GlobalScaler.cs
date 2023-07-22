using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Presentation
{
    public class GlobalScaler : MonoBehaviour
    {
        [SerializeField] private Vector3 TargetGlobalScale = Vector3.one;
        [FormerlySerializedAs("Tolerance")] [SerializeField] private float ToleranceRatio = 0.01f;

        private Vector3 _tolerance;

        private void Start()
        {
            CorrectScale();
            _tolerance = new Vector3(
                x: ToleranceRatio * TargetGlobalScale.x,
                y: ToleranceRatio * TargetGlobalScale.y,
                z: ToleranceRatio * TargetGlobalScale.z
            );
        }

        private void Update()
        {
            if (
                Mathf.Abs(transform.lossyScale.x - TargetGlobalScale.x) < _tolerance.x &&
                Mathf.Abs(transform.lossyScale.y - TargetGlobalScale.y) < _tolerance.y &&
                Mathf.Abs(transform.lossyScale.z - TargetGlobalScale.z) < _tolerance.z
            ) return;

            CorrectScale();
        }

        private void CorrectScale()
        {
            Vector3 globalScale = transform.lossyScale;
            Vector3 localScale = transform.localScale;

            transform.localScale = new Vector3(
                x: (TargetGlobalScale.x / globalScale.x) * localScale.x,
                y: (TargetGlobalScale.y / globalScale.y) * localScale.y,
                z: (TargetGlobalScale.z / globalScale.z) * localScale.z
            );
        }
    }
}
