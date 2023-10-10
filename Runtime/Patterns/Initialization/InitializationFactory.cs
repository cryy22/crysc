using UnityEngine;

namespace Crysc.Patterns.Initialization
{
    public abstract class InitializationFactory<T, TConfig> : ScriptableObject
        where T : InitializationBehaviour<TConfig>
    {
        [SerializeField] private T Prefab;

        public virtual T CreateEnemy(TConfig config)
        {
            T instance = Instantiate();
            instance.Initialize(config);
            return instance;
        }

        protected T Instantiate() { return Instantiate(Prefab); }
    }
}
