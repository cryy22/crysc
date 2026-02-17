using UnityEngine;

namespace Crysc.Utilities
{
    public class DestroyUnlessEditor : MonoBehaviour
    {
        [SerializeField] private bool AlwaysDestroy;

        private void Awake()
        {
#if !UNITY_EDITOR
            Destroy(gameObject);
#else
            if (AlwaysDestroy)
                Destroy(gameObject);
#endif
        }
    }
}
