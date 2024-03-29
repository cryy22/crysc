using UnityEngine;

namespace Crysc.Utilities
{
    public class PersistentSingletonParent : MonoBehaviour
    {
        private static PersistentSingletonParent _instance;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
