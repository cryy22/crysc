#region

using UnityEngine;
using UnityEngine.Rendering;

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

            if (go.TryGetComponent(out SortingManipulatorRoot root))
            {
                if (root.Skip)
                    return;

                if (root.SortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                root.SortingLayerID = sortingLayerId;
                root.SortOrder -= sourceSortingOrder;
                root.SortOrder += sortingOrder;
            }

            else if (go.TryGetComponent(out SortingGroup group))
            {
                if (group.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                group.sortingLayerID = sortingLayerId;
                group.sortingOrder -= sourceSortingOrder;
                group.sortingOrder += sortingOrder;
                return;
            }

            else if (go.TryGetComponent(out Canvas canvas))
            {
                if (canvas.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                canvas.sortingLayerID = sortingLayerId;
                canvas.sortingOrder -= sourceSortingOrder;
                canvas.sortingOrder += sortingOrder;
                return;
            }

            else if (go.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                if (spriteRenderer.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                spriteRenderer.sortingLayerID = sortingLayerId;
                spriteRenderer.sortingOrder -= sourceSortingOrder;
                spriteRenderer.sortingOrder += sortingOrder;
            }

            else if (go.TryGetComponent(out MeshRenderer meshRenderer))
            {
                if (meshRenderer.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                meshRenderer.sortingLayerID = sortingLayerId;
                meshRenderer.sortingOrder -= sourceSortingOrder;
                meshRenderer.sortingOrder += sortingOrder;
            }

            else if (go.TryGetComponent(out ParticleSystemRenderer particleRenderer))
            {
                if (particleRenderer.sortingLayerID != sourceSortingLayerId)
                {
                    Debug.LogWarning($"{go.name} sorting layer does not match root object.");
                    return;
                }

                particleRenderer.sortingLayerID = sortingLayerId;
                particleRenderer.sortingOrder -= sourceSortingOrder;
                particleRenderer.sortingOrder += sortingOrder;
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

        public static (int sortingLayerId, int sortingOrder, bool found) GetSortingDetails(this GameObject go)
        {
            if (go.TryGetComponent(out SortingManipulatorRoot root))
                return (root.SortingLayerID, root.SortOrder, true);

            if (go.TryGetComponent(out SortingGroup group))
                return (group.sortingLayerID, group.sortingOrder, true);

            if (go.TryGetComponent(out Canvas canvas))
                return (canvas.sortingLayerID, canvas.sortingOrder, true);

            if (go.TryGetComponent(out SpriteRenderer spriteRenderer))
                return (spriteRenderer.sortingLayerID, spriteRenderer.sortingOrder, true);

            if (go.TryGetComponent(out MeshRenderer meshRenderer))
                return (meshRenderer.sortingLayerID, meshRenderer.sortingOrder, true);

            if (go.TryGetComponent(out ParticleSystemRenderer particleRenderer))
                return (particleRenderer.sortingLayerID, particleRenderer.sortingOrder, true);

            Debug.LogWarning("No sorting component found on GameObject " + go.name);
            return (0, 0, false);
        }
    }
}
