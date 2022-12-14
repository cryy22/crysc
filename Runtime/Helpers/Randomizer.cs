using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;

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

        public static T GetRandomElement<T>(IEnumerable<T> enumerable)
        {
            List<T> list = enumerable.ToList();
            return list[Random.Range(minInclusive: 0, maxExclusive: list.Count)];
        }

        public static T GetWeightedRandomElement<T>(IEnumerable<T> enumerable, Func<T, int> weightFunc)
        {
            List<ElementWeight<T>> elementWeights = new();
            var totalWeight = 0;

            foreach (T element in enumerable)
            {
                elementWeights.Add(item: new ElementWeight<T>(element: element, minWeight: totalWeight));
                totalWeight += weightFunc(element);
            }

            int randomWeight = Random.Range(minInclusive: 0, maxExclusive: totalWeight);
            return elementWeights.Last(ew => randomWeight >= ew.MinWeight).Element;
        }

        private readonly struct ElementWeight<T>
        {
            public T Element { get; }
            public int MinWeight { get; }

            public ElementWeight(T element, int minWeight)
            {
                Element = element;
                MinWeight = minWeight;
            }
        }
    }
}
