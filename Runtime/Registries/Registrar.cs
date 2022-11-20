using UnityEngine;

namespace Crysc.Registries
{
    public abstract class Registrar<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private Registry<T> Registry;
        public T Registrant { get; private set; }

        private void Awake() { Registrant = GetComponent<T>(); }
        private void Start() { Registry.Register(this); }
        private void OnDestroy() { Registry.Unregister(this); }
    }
}
