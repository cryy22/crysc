#region

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public class SortingManipulatorRoot : MonoBehaviour
    {
        [field: SerializeField, ValueDropdown("GetSortingLayers")]
        public string SortingLayerName { get; set; }
        [field: SerializeField] public int SortOrder { get; set; }
        [field: SerializeField] public bool Skip { get; private set; }

        public int SortingLayerID
        {
            get => SortingLayer.NameToID(SortingLayerName);
            set => SortingLayerName = SortingLayer.IDToName(value);
        }

        private IEnumerable<string> GetSortingLayers()
        {
            return SortingLayer.layers.Select(l => l.name);
        }
    }
}
