#region

using UnityEngine;

#endregion

namespace Crysc.Patterns.Initialization
{
    public abstract class InitializationFactory<T, TBlueprint> : ScriptableObject
        where T : Object
    {
        [SerializeField] private T Prefab;

        public abstract T Create(TBlueprint config);

        protected T Instantiate()
        {
            return Instantiate(Prefab);
        }
    }
}
