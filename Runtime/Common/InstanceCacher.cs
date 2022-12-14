using UnityEngine.AddressableAssets;

namespace Crysc.Common
{
    public class InstanceCacher<T> where T : class
    {
        private readonly InstanceCreator _creator;
        private T _instance;

        public InstanceCacher(InstanceCreator creator) { _creator = creator; }

        public InstanceCacher(string assetAddress) { _creator = () => LoadAsset(assetAddress); }

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

        private static T LoadAsset(string assetAddress)
        {
            return Addressables.LoadAssetAsync<T>(assetAddress).WaitForCompletion();
        }
    }
}
