using UnityEngine;

namespace Crysc.Common
{
    public abstract class InitializedBehaviour<TInitParams> : MonoBehaviour
    {
        protected bool IsInitialized { get; private set; }
        protected TInitParams InitParams { get; private set; }

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
