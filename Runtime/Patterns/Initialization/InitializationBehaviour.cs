using UnityEngine;

namespace Crysc.Patterns.Initialization
{
    public abstract class InitializationBehaviour<TInitParams> : MonoBehaviour
    {
        public bool IsInitialized { get; private set; }
        public TInitParams InitParams { get; private set; }

        public virtual void Initialize(TInitParams initParams)
        {
            if (IsInitialized)
            {
                Debug.LogWarning($"Trying to initialize {GetType().Name}, but already initialized");
                return;
            }

            InitParams = initParams;
            IsInitialized = true;
        }
    }
}
