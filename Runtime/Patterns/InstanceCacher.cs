using UnityEngine;

namespace Crysc.Patterns
{
    public class InstanceCacher<T> where T : Object
    {
        private readonly InstanceCreator _creator;
        private T _instance;

        public InstanceCacher(InstanceCreator creator) { _creator = creator; }

        public InstanceCacher(string resourceAddress) { _creator = () => LoadAsset(resourceAddress); }

        public delegate T InstanceCreator();

        public T I
        {
            get
            {
                // don't bypass null check when T is a Unity Component
                // ReSharper disable once ConvertIfStatementToNullCoalescingExpression
                if (_instance == null) _instance = _creator();
                return _instance;
            }
        }

        private static T LoadAsset(string address) { return Resources.Load<T>(address); }
    }
}
