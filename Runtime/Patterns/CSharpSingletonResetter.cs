#region

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Crysc.Patterns
{
    public static class CSharpSingletonResetter
    {
        private static readonly List<Action> _resetActions = new();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnPlay()
        {
            foreach (Action action in _resetActions)
                action();
        }

        public static void AddResetAction(Action action)
        {
            _resetActions.Add(action);
        }
    }
}
