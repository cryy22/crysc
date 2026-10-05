#region

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public static class SortingLayersUtility
    {
        // used by Odin Inspector (search the string)
        public static IEnumerable<string> GetSortingLayers()
        {
            return SortingLayer.layers.Select(l => l.name);
        }
    }
}
