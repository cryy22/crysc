using UnityEngine;

namespace Crysc.Patterns
{
    public class InstanceCacher<T> where T : Object
    {
        public delegate T InstanceCreator();

        private readonly InstanceCreator _creator;
        private T _instance;

        public T I => _instance != null ? _instance : _instance = _creator();

        public InstanceCacher(InstanceCreator creator) { _creator = creator; }
        public InstanceCacher(string resourceAddress) { _creator = () => Resources.Load<T>(resourceAddress); }
    }
}
