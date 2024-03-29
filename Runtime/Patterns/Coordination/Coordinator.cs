using System;
using System.Collections;
using UnityEngine;

namespace Crysc.Patterns.Coordination
{
    public class Coordinator<TConfig, TState> : MonoBehaviour
        where TConfig : CoordinationConfig
        where TState : CoordinationState
    {
        [SerializeField] private GameObject Container;
        private bool _isActive;

        public bool IsActive
        {
            get => _isActive;
            private set
            {
                _isActive = value;
                Changed?.Invoke(sender: this, e: new CoordinationEventArgs(value));
            }
        }

        protected TConfig Config { get; private set; }
        protected TState State { get; private set; }

        public event EventHandler<CoordinationEventArgs> Changed;

        protected virtual void Awake()
        {
            if (Container != null) Container.SetActive(false);
        }

        public IEnumerator BeginAndWaitForEnd(TConfig config, TState state)
        {
            Begin(config: config, state: state);
            yield return new WaitUntil(() => !IsActive);
        }

        public virtual void Begin(TConfig config, TState state)
        {
            if (gameObject.activeInHierarchy == false)
                throw new ApplicationException("#Begin called on Coordinator that is not active in hierarchy.");
            if (IsActive)
                Debug.LogWarning("#Begin called on Coordinator that is already active.");

            Config = config;
            State = state;

            IsActive = true;
            if (Container != null) Container.SetActive(true);
        }

        public virtual void End()
        {
            if (!IsActive) Debug.LogWarning("#End called on Coordinator that is not active.");

            Config = null;
            State = null;

            if (Container != null) Container.SetActive(false);
            IsActive = false;
        }
    }

    public class Coordinator : Coordinator<CoordinationConfig, CoordinationState>
    { }
}
