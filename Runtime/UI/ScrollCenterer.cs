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

            float viewportWidth = ScrollRect.viewport.rect.width;
            float contentWidth = ScrollRect.content.rect.width;
            float viewportHeight = ScrollRect.viewport.rect.height;
            float contentHeight = ScrollRect.content.rect.height;

            float leftmostHorizontalScroll =
                Mathf.Clamp01((maxPosition.x - viewportWidth) / (contentWidth - viewportWidth));
            float rightmostHorizontalScroll =
                Mathf.Clamp01(minPosition.x / (contentWidth - viewportWidth));
            ScrollRect.horizontalNormalizedPosition = Mathf.Clamp(
                value: ScrollRect.horizontalNormalizedPosition,
                min: leftmostHorizontalScroll,
                max: rightmostHorizontalScroll
            );

            // this is specifically for handling dropdown scroll centering.
            // may need to parameterize to handle different vertical centering scenarios.
            float downmostVerticalScroll =
                Mathf.Clamp01(((maxPosition.y + contentHeight) - viewportHeight) / (contentHeight - viewportHeight));
            float upmostVerticalScroll =
                Mathf.Clamp01((minPosition.y + contentHeight) / (contentHeight - viewportHeight));
            ScrollRect.verticalNormalizedPosition = Mathf.Clamp(
                value: ScrollRect.verticalNormalizedPosition,
                min: downmostVerticalScroll,
                max: upmostVerticalScroll
            );
        }
    }
}
