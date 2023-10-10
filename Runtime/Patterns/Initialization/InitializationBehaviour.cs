using UnityEngine;

namespace Crysc.Patterns.Initialization
{
    public abstract class InitializationBehaviour<TConfig> : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }
        public TConfig Config { get; private set; }

        public virtual void Initialize(TConfig config)
        {
            if (IsInitialized)
            {
                Debug.LogWarning($"Trying to initialize {GetType().Name}, but already initialized");
                return;
            }

            Config = config;
            IsInitialized = true;
        }
    }
}
