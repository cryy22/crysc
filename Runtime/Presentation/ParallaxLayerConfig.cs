using UnityEngine;

namespace Crysc.Presentation
{
    [CreateAssetMenu(fileName = "New ParallaxLayerConfig", menuName = "gg/Config/Parallax Layer Config")]
    public class ParallaxLayerConfig : ScriptableObject
    {
        [field: SerializeField] public float Speed { get; private set; }
    }
}
