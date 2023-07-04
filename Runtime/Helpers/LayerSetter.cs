using UnityEngine;

namespace Crysc.Helpers
{
    public static class LayerSetter
    {
        public static void SetLayerRecursively(Component obj, int layer)
        {
            obj.gameObject.layer = layer;
            foreach (Transform child in obj.transform) SetLayerRecursively(obj: child, layer: layer);
        }
    }
}
