using UnityEngine;

namespace Crysc.Common
{
    [DefaultExecutionOrder(-1)]
    public class SingletonBehaviour<T> : MonoBehaviour where T : SingletonBehaviour<T>
    {
        public static T I { get; private set; }

        protected virtual void Awake()
        {
            if (I != null)
            {
                Debug.LogWarning("Multiple instances of singleton " + typeof(T).Name);
                Destroy(gameObject);
                return;
            }

            I = (T) this;
        }
    }
}
