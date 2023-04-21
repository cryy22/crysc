using UnityEngine;

namespace Crysc.Patterns.Initialization
{
    public abstract class InitializationFactory<T, TConfig, TInitParams> : ScriptableObject
        where T : InitializationBehaviour<TInitParams>
    {
        [SerializeField] private T Prefab;
        public abstract T Create(TConfig config);
        protected T Instantiate() { return Instantiate(Prefab); }
    }

    public abstract class InitializationFactory<T, TConfig> : InitializationFactory<T, TConfig, TConfig>
        where T : InitializationBehaviour<TConfig>
    {
        public override T Create(TConfig config)
        {
            T instance = Instantiate();
            instance.Initialize(config);
            return instance;
        }
    }
}
