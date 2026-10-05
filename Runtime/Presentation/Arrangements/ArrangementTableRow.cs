#region

using Crysc.Helpers;
using UnityEngine;

#endregion

namespace Crysc.Presentation.Arrangements
{
    public class ArrangementTableRow : MonoBehaviour, IArrangementElement
    {
        [field: SerializeField] public SimpleArrangement Arrangement { get; private set; }
        [field: SerializeField] public SortingEnforcer SortingEnforcer { get; private set; }
        [field: SerializeField] public ParallaxRegistrar ParallaxRegistrar { get; private set; }

        public Transform Transform => transform;
    }
}
