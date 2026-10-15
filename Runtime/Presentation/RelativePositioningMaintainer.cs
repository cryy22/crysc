#region

using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

#endregion

namespace Crysc.Presentation
{
    public class RelativePositioningMaintainer : MonoBehaviour
    {
#if UNITY_EDITOR
        private const float RoundingFactor = 100000f;
        private const string UndoName = "Reset Positioning";

        [SerializeField] private SpriteRenderer LeadRenderer;
        [SerializeField] private TMP_Text LeadText;

        [SerializeField] private FollowBoxCollider[] FollowBoxColliders;
        [SerializeField] private FollowRenderer[] FollowRenderers;
        [SerializeField] private FollowText[] FollowTexts;

        private enum Alignment
        {
            Center,
            Left,
            Right,
        }

        [Serializable]
        private struct FollowBoxCollider
        {
            [SerializeField] public BoxCollider2D Collider;
            [SerializeField] public Vector2 SizeOffset;
            [SerializeField] public Vector2 PositioningOffset;
        }

        [Serializable]
        private struct FollowRenderer
        {
            [SerializeField] public SpriteRenderer Renderer;
            [SerializeField] public Vector2 SizeOffset;
            [SerializeField] public Vector2 PositioningOffset;
            [SerializeField] public bool IncludeScale;
        }

        [Serializable]
        private struct FollowText
        {
            [SerializeField] public TMP_Text Text;
            [SerializeField] public Vector2 SizeOffset;
            [SerializeField] public Vector2 PositioningOffset;
        }

        [Button]
        private void ResetPositioning()
        {
            if (!LeadRenderer && !LeadText)
                return;

            if (LeadRenderer && LeadText)
            {
                Debug.LogWarning("RelativePositioningMaintainer can only have one lead.");
                return;
            }

            Vector2 leadSize = GetLeadSize();

            foreach (FollowBoxCollider followBoxCollider in FollowBoxColliders)
            {
                BoxCollider2D boxCollider = followBoxCollider.Collider;
                if (!boxCollider)
                    continue;

                Vector2 targetSize = leadSize / GetRelativeScale(boxCollider.transform) + followBoxCollider.SizeOffset;
                ApplyIfChanged(
                    undoTarget: boxCollider,
                    current: boxCollider.size,
                    target: targetSize,
                    apply: size => boxCollider.size = size
                );
                ApplyIfChanged(
                    undoTarget: boxCollider,
                    current: boxCollider.offset,
                    target: followBoxCollider.PositioningOffset,
                    apply: offset => boxCollider.offset = offset
                );
            }

            foreach (FollowRenderer followRenderer in FollowRenderers)
            {
                SpriteRenderer spriteRenderer = followRenderer.Renderer;
                if (!spriteRenderer)
                    continue;

                Vector2 scale = followRenderer.IncludeScale
                    ? GetRelativeScale(spriteRenderer.transform)
                    : Vector2.one;
                ApplyIfChanged(
                    undoTarget: spriteRenderer,
                    current: spriteRenderer.size,
                    target: leadSize / scale + followRenderer.SizeOffset,
                    apply: size => spriteRenderer.size = size
                );

                SetPosition(
                    positioningT: spriteRenderer.transform,
                    leadSize: leadSize,
                    offset: followRenderer.PositioningOffset,
                    alignment: (spriteRenderer.sprite.pivot.x / spriteRenderer.sprite.rect.width) switch
                    {
                        < 0.4f => Alignment.Left,
                        > 0.6f => Alignment.Right,
                        _      => Alignment.Center,
                    }
                );
            }

            foreach (FollowText followText in FollowTexts)
            {
                TMP_Text text = followText.Text;
                if (!text)
                    continue;

                ApplyIfChanged(
                    undoTarget: text.rectTransform,
                    current: text.rectTransform.sizeDelta,
                    target: leadSize / GetRelativeScale(text.transform) + followText.SizeOffset,
                    apply: size => text.rectTransform.sizeDelta = size
                );
                SetPosition(positioningT: text.transform, leadSize: leadSize, offset: followText.PositioningOffset);
            }
        }

        private Vector2 GetLeadSize()
        {
            if (LeadRenderer)
                return LeadRenderer.size * GetRelativeScale(LeadRenderer.transform);

            LeadText.ForceMeshUpdate();
            return (Vector2) LeadText.textBounds.size * GetRelativeScale(LeadText.transform);
        }

        private Vector2 GetRelativeScale(Transform t)
        {
            return (Vector2) t.lossyScale / transform.lossyScale;
        }

        private static void SetPosition(
            Transform positioningT,
            Vector2 leadSize,
            Vector2 offset,
            Alignment alignment = Alignment.Center
        )
        {
            Vector2 alignmentPosition = alignment switch
            {
                Alignment.Left  => new Vector2(x: leadSize.x * -0.5f, y: 0),
                Alignment.Right => new Vector2(x: leadSize.x * 0.5f, y: 0),
                _               => Vector2.zero,
            };

            ApplyIfChanged(
                undoTarget: positioningT,
                current: positioningT.localPosition,
                target: alignmentPosition + offset,
                apply: position => positioningT.localPosition = new Vector3(
                    x: position.x,
                    y: position.y,
                    z: positioningT.localPosition.z
                )
            );
        }

        private static void ApplyIfChanged(Object undoTarget, Vector2 current, Vector2 target, Action<Vector2> apply)
        {
            Vector2 roundedTarget = Round(target);
            if (!IsSignificantChange(current: current, target: roundedTarget))
                return;

            Undo.RecordObject(objectToUndo: undoTarget, name: UndoName);
            apply(roundedTarget);
        }

        private static bool IsSignificantChange(Vector2 current, Vector2 target)
        {
            const float threshold = RoundingFactor / 2;

            if (Mathf.Abs(current.x - target.x) >= threshold) return true;
            if (Mathf.Abs(current.y - target.y) >= threshold) return true;
            return false;
        }

        private static Vector2 Round(Vector2 value)
        {
            return new Vector2(x: Round(value.x), y: Round(value.y));
        }

        private static float Round(float value)
        {
            return Mathf.Round(value * RoundingFactor) / RoundingFactor;
        }
#else
        private void Awake()
        {
            Destroy(this);
        }
#endif
    }
}
