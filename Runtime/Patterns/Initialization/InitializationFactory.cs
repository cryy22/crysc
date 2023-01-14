using UnityEngine;

namespace Crysc.Patterns.Initialization
{
    public abstract class InitializationFactory<T, TConfig, TInitParams> : ScriptableObject
        where T : InitializationBehaviour<TInitParams>
    {
        [SerializeField] private T Prefab;

        public T Create(TConfig config)
        {
            T instance = Instantiate(Prefab);
            instance.Initialize(GetInitParams(config));

            return instance;
        }

        protected abstract TInitParams GetInitParams(TConfig config);
    }

    public abstract class InitializationFactory<T, TConfig> : InitializationFactory<T, TConfig, TConfig>
        where T : InitializationBehaviour<TConfig>
    {
        protected override TConfig GetInitParams(TConfig config) { return config; }
    }
}
