using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Crysc.UI
{
    public class ScrollCenterer : MonoBehaviour
    {
        [SerializeField] private ScrollRect ScrollRect;
        [SerializeField] private RectTransform ListItemParent;
        [SerializeField] private float OffsetPercentage = 0.125f;

        private GameObject _selected;

        private void Update()
        {
            if (EventSystem.current.currentSelectedGameObject == _selected) return;

            _selected = EventSystem.current.currentSelectedGameObject;
            if (ListItemParent.Cast<Transform>().Any(item => item.gameObject == _selected) == false) return;

            var target = _selected.GetComponent<RectTransform>();
            if (!target) return;

            float viewportWidth = ScrollRect.viewport.rect.width;
            float contentWidth = ScrollRect.content.rect.width;

            var targetCorners = new Vector3[4];
            target.GetWorldCorners(targetCorners);

            Vector2 minPosition = Vector2.positiveInfinity;
            Vector2 maxPosition = Vector2.negativeInfinity;
            foreach (Vector3 corner in targetCorners)
            {
                Vector3 localCorner = ScrollRect.content.InverseTransformPoint(corner);
                minPosition = Vector2.Min(lhs: minPosition, rhs: localCorner);
                maxPosition = Vector2.Max(lhs: maxPosition, rhs: localCorner);
            }

            minPosition -= target.rect.size * OffsetPercentage;
            maxPosition += target.rect.size * OffsetPercentage;

            float minHorizontalScroll = Mathf.Clamp01((maxPosition.x - viewportWidth) / (contentWidth - viewportWidth));
            float maxHorizontalScroll = Mathf.Clamp01(minPosition.x / (contentWidth - viewportWidth));
            ScrollRect.horizontalNormalizedPosition = Mathf.Clamp(
                value: ScrollRect.horizontalNormalizedPosition,
                min: minHorizontalScroll,
                max: maxHorizontalScroll
            );
        }
    }
}
