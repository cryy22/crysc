using System;
using UnityEngine;

namespace Crysc.Coordination
{
    public class Coordinator : MonoBehaviour
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

        protected virtual void Awake()
        {
            if (Container != null) Container.SetActive(false);
        }

        public virtual void BeginCoordination()
        {
            IsActive = true;
            if (Container != null) Container.SetActive(true);
        }

        public virtual void EndCoordination()
        {
            if (Container != null) Container.SetActive(false);
            IsActive = false;
        }
    }
}
