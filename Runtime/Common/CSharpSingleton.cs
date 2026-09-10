#region

using System;
using UnityEngine;

#endregion

namespace Crysc.Common
{
    public abstract class CSharpSingleton<T> where T : CSharpSingleton<T>, new()
    {
        private static T _instance;
        public static T I => _instance ??= new T();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnPlay()
        {
            _instance = null;
        }

        protected CSharpSingleton()
        {
            if (_instance != null)
                throw new InvalidOperationException("Singleton already initialized");
        }
    }
}
