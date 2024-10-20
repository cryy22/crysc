using UnityEngine;

namespace Crysc.Presentation
{
    public class ParallaxBackgroundRegistrar : MonoBehaviour
    {
        [SerializeField] private ParallaxBackground.Layer Layer;
        [SerializeField] private bool IsAffectedBySpeed;

        private void Start()
        {
            if (ParallaxBackground.I)
                ParallaxBackground.I.Register(
                    layer: Layer,
                    registrant: transform,
                    isAffectedBySpeed: IsAffectedBySpeed
                );
        }

        private void OnDestroy()
        {
            if (ParallaxBackground.I)
                ParallaxBackground.I.Deregister(
                    layer: Layer,
                    registrant: transform
                );
        }
    }
}
