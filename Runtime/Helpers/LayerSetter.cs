#region

using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public static class LayerSetter
    {
        public static void SetLayerRecursively(GameObject obj, int layer)
        {
            obj.gameObject.layer = layer;
            foreach (Transform child in obj.transform)
                SetLayerRecursively(obj: child.gameObject, layer: layer);
        }

        public static void SwapLayerRecursively(GameObject obj, int from, int to)
        {
            if (obj.layer == from)
                obj.layer = to;

            foreach (Transform child in obj.transform)
                SwapLayerRecursively(obj: child.gameObject, from: from, to: to);
        }
    }
}
