using UnityEngine;

namespace Crysc.Initialization
{
    public abstract class InitializationFactory<TBehaviour, TConfig, TInitParams> : ScriptableObject
        where TBehaviour : InitializationBehaviour<TInitParams>
    {
        [SerializeField] private TBehaviour Prefab;

        public TBehaviour Create(TConfig config)
        {
            TBehaviour instance = Instantiate(Prefab);
            instance.Initialize(GetInitParams(config));

            return instance;
        }

        protected virtual TInitParams GetInitParams(TConfig config)
        {
            return config is TInitParams initParams ? initParams : default;
        }
    }
}
