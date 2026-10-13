#region

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Crysc.Utilities
{
    [DefaultExecutionOrder(-100)]
    public class PersistentSingletonParent : MonoBehaviour
    {
        private static readonly HashSet<string> _typeIds = new();

        [field: SerializeField] private string TypeId { get; set; } = "";

        private void Awake()
        {
            if (!_typeIds.Add(TypeId))
            {
                DestroyImmediate(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ResetAll()
        {
            _typeIds?.Clear();
        }
    }
}
