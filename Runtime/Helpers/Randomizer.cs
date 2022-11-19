using System.Collections.Generic;
using UnityEngine;

namespace Crysc.Helpers
{
    public static class Randomizer
    {
        public static void RandomizeElements<T>(List<T> list)
        {
            for (var i = 0; i < list.Count - 1; i++)
            {
                int randomIndex = Random.Range(minInclusive: i, maxExclusive: list.Count);
                (list[i], list[randomIndex]) = (list[randomIndex], list[i]);
            }
        }
    }
}
