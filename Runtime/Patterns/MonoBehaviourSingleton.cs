#region

using UnityEngine;

#endregion

namespace Crysc.Patterns
{
    public class MonoBehaviourSingleton<T> : MonoBehaviour
        where T : MonoBehaviourSingleton<T>
    {
        public static T I { get; private set; }

        protected virtual void Awake()
        {
            if (I)
            {
                Destroy(gameObject);
                return;
            }

            I = (T) this;
        }
    }
}
