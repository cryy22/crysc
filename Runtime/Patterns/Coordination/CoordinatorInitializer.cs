using UnityEngine;

namespace Crysc.Patterns.Coordination
{
    public class CoordinatorInitializer<TConfig, TState> : MonoBehaviour
        where TConfig : CoordinationConfig
        where TState : CoordinationState
    {
        [SerializeField] private Coordinator<TConfig, TState> Coordinator;
        [SerializeField] protected TConfig Config;
        [SerializeField] protected TState State;

        [SerializeField] private bool InitializeImmediately = true;

        private void Start()
        {
            if (InitializeImmediately) Initialize();
        }

        public virtual void Initialize()
        {
            if (Coordinator.IsActive)
            {
                Debug.LogWarning("CoordinatorInitialize attempted to initialize while Coordinator already active.");
                return;
            }

            Coordinator.Begin(config: Config, state: State);
        }
    }
}
