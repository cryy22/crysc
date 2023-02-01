using System;
using UnityEngine;

namespace Crysc.Patterns.Coordination
{
    public class Coordinator<TConfig, TState> : MonoBehaviour
        where TConfig : CoordinationConfig
        where TState : CoordinationState
    {
        [SerializeField] private GameObject Container;
        private bool _isActive;

        public event EventHandler<CoordinationEventArgs> Changed;

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

        protected virtual void Awake()
        {
            if (Container != null) Container.SetActive(false);
        }

        public virtual void BeginCoordination(TConfig config, TState state)
        {
            Config = config;
            State = state;

            IsActive = true;
            if (Container != null) Container.SetActive(true);
        }

        public virtual void EndCoordination()
        {
            Config = null;
            State = null;

            if (Container != null) Container.SetActive(false);
            IsActive = false;
        }
    }

    public class Coordinator : Coordinator<CoordinationConfig, CoordinationState>
    { }
}
