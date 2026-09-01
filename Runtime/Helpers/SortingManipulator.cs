#region

using UnityEngine;

#endregion

namespace Crysc.Helpers
{
    public static class SortingManipulator
    {
        public static void SetSortingDetails(
            this GameObject go,
            int sortingLayerId,
            int sortingOrder,
            int sourceSortingLayerId = 0,
            int sourceSortingOrder = 0,
            bool sourceFound = false
        )
        {
            if (!sourceFound)
            {
                (sourceSortingLayerId, sourceSortingOrder, sourceFound) = go.GetSortingDetails();

                if (!sourceFound)
                {
                    Debug.LogWarning("Root game object has not sorting layer component.");
                    return;
                }
            }

            if (go.GetComponent<Canvas>())
            {
                var canvas = go.GetComponent<Canvas>();

                if (canvas.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                canvas.sortingLayerID = sortingLayerId;
                canvas.sortingOrder -= sourceSortingOrder;
                canvas.sortingOrder += sortingOrder;
            }

            if (go.GetComponent<SpriteRenderer>())
            {
                var renderer = go.GetComponent<SpriteRenderer>();

                if (renderer.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                renderer.sortingLayerID = sortingLayerId;
                renderer.sortingOrder -= sourceSortingOrder;
                renderer.sortingOrder += sortingOrder;
            }

            if (go.GetComponent<MeshRenderer>())
            {
                var renderer = go.GetComponent<MeshRenderer>();

                if (renderer.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                renderer.sortingLayerID = sortingLayerId;
                renderer.sortingOrder -= sourceSortingOrder;
                renderer.sortingOrder += sortingOrder;
            }

            if (go.GetComponent<ParticleSystemRenderer>())
            {
                var renderer = go.GetComponent<ParticleSystemRenderer>();

                if (renderer.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                renderer.sortingLayerID = sortingLayerId;
                renderer.sortingOrder -= sourceSortingOrder;
                renderer.sortingOrder += sortingOrder;
            }

            foreach (Transform child in go.transform)
                child.gameObject.SetSortingDetails(
                    sortingLayerId: sortingLayerId,
                    sortingOrder: sortingOrder,
                    sourceSortingLayerId: sourceSortingLayerId,
                    sourceSortingOrder: sourceSortingOrder,
                    sourceFound: sourceFound
                );
        }

        private static (int sortingLayerIdOffset, int sortingOrderOffset) GetOffsets(
            int sortingLayerId,
            int sortingOrder,
            int baselineSortingLayerId,
            int baselineSortingOrder
        )
        {
            if (baselineSortingLayerId < 0)
                return (0, 0);

            return (
                sortingLayerId - baselineSortingLayerId,
                sortingOrder - baselineSortingOrder
            );
        }

        public static (int sortingLayerId, int sortingOrder, bool found) GetSortingDetails(this GameObject go)
        {
            if (go.GetComponent<Canvas>())
            {
                var canvas = go.GetComponent<Canvas>();
                return (canvas.sortingLayerID, canvas.sortingOrder, true);
            }

            if (go.GetComponent<SpriteRenderer>())
            {
                var renderer = go.GetComponent<SpriteRenderer>();
                return (renderer.sortingLayerID, renderer.sortingOrder, true);
            }

            if (go.GetComponent<MeshRenderer>())
            {
                var renderer = go.GetComponent<MeshRenderer>();
                return (renderer.sortingLayerID, renderer.sortingOrder, true);
            }

            if (go.GetComponent<ParticleSystemRenderer>())
            {
                var renderer = go.GetComponent<ParticleSystemRenderer>();
                return (renderer.sortingLayerID, renderer.sortingOrder, true);
            }

            Debug.LogWarning("No sorting component found on GameObject " + go.name);
            return (0, 0, false);
        }
    }
}
