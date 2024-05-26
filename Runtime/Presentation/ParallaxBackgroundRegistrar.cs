using UnityEngine;

namespace Crysc.Presentation
{
    public class ParallaxBackgroundRegistrar : MonoBehaviour
    {
        [SerializeField] private ParallaxBackground.Layer Layer;
        private void Start() { ParallaxBackground.I?.Register(layer: Layer, registrant: transform); }
        private void OnDestroy() { ParallaxBackground.I?.Deregister(layer: Layer, registrant: transform); }
    }
}
