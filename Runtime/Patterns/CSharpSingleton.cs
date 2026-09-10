#region

using System;

#endregion

namespace Crysc.Patterns
{
    public abstract class CSharpSingleton<T> where T : CSharpSingleton<T>, new()
    {
        private static T _instance;
        public static T I => _instance ??= new T();

        protected CSharpSingleton()
        {
            if (_instance != null)
                throw new InvalidOperationException("Singleton already initialized");

            CSharpSingletonResetter.AddResetAction(() => _instance = null);
        }
    }
}
