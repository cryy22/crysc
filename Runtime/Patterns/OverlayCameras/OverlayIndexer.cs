using UnityEngine;

namespace Crysc.Patterns.OverlayCameras
{
    [RequireComponent(typeof(Camera))]
    public class OverlayIndexer : MonoBehaviour
    {
        [SerializeField] private int IndexInput;

        public int Index => IndexInput;

        private void Awake() { GetComponent<Camera>().depth = Index; }
    }
}
