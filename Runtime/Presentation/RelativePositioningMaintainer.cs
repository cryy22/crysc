#region

using System;
using TMPro;
using UnityEditor;
using UnityEngine;

#endregion

namespace Crysc.Presentation
{
    [ExecuteAlways]
    public class RelativePositioningMaintainer : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private SpriteRenderer LeadRenderer;
        [SerializeField] private TMP_Text LeadText;

        [SerializeField] private FollowBoxCollider[] FollowBoxColliders;
        [SerializeField] private FollowRenderer[] FollowRenderers;
        [SerializeField] private FollowText[] FollowTexts;

        private enum LeadType
        {
            None,
            Renderer,
            Text,
        }

        private enum Alignment
        {
            Center,
            Left,
            Right,
        }

        private LeadType _leadType = LeadType.None;

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

        private Vector2 _currentSize;

        private void Update()
        {
            if (Application.isPlaying)
                return;

            if (_leadType == LeadType.None)
                DetermineLeadType();

            if (_leadType == LeadType.None)
                return;

            if (GetLeadSize() != _currentSize)
                ResetSizes();
        }

        private void DetermineLeadType()
        {
            if (LeadRenderer && LeadText)
            {
                Debug.LogWarning("RelativeSizeMaintainer can only have one lead.");
                _leadType = LeadType.None;
                return;
            }

            if (LeadRenderer)
                _leadType = LeadType.Renderer;
            else if (LeadText)
                _leadType = LeadType.Text;
            else
                _leadType = LeadType.None;
        }

        private Vector2 GetLeadSize()
        {
            return _leadType switch
            {
                LeadType.Renderer => LeadRenderer.size * LeadRenderer.transform.lossyScale,
                LeadType.Text     => (Vector2) LeadText.textBounds.size * LeadText.transform.lossyScale,
                _                 => Vector2.zero,
            };
        }

        private void ResetSizes()
        {
            _currentSize = GetLeadSize();

            foreach (FollowBoxCollider followBoxCollider in FollowBoxColliders)
            {
                BoxCollider2D boxCollider = followBoxCollider.Collider;
                if (!boxCollider)
                    continue;

                Vector2 targetSize = _currentSize / boxCollider.transform.lossyScale;
                targetSize += followBoxCollider.SizeOffset;
                boxCollider.size = targetSize;
                // SetPosition(positioningT: boxCollider.transform, offset: followBoxCollider.PositioningOffset);
            }

            foreach (FollowRenderer followRenderer in FollowRenderers)
            {
                SpriteRenderer spriteRenderer = followRenderer.Renderer;
                if (!spriteRenderer)
                    continue;

                Vector2 targetSize = _currentSize
                    / (followRenderer.IncludeScale ? spriteRenderer.transform.lossyScale : Vector2.one);
                targetSize += followRenderer.SizeOffset;
                spriteRenderer.size = targetSize;

                SetPosition(
                    positioningT: spriteRenderer.transform,
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

                Vector2 targetSize = _currentSize / text.transform.lossyScale;
                targetSize += followText.SizeOffset;
                text.rectTransform.sizeDelta = targetSize;
                SetPosition(positioningT: text.transform, offset: followText.PositioningOffset);
            }
        }

        private void SetPosition(Transform positioningT, Vector2 offset, Alignment alignment = Alignment.Center)
        {
            if (alignment == Alignment.Center)
            {
                positioningT.localPosition = offset;
                return;
            }

            positioningT.localPosition = new Vector2(
                x: _currentSize.x * 0.5f * (alignment == Alignment.Left ? -1 : 1),
                y: 0
            ) + offset;
        }

        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                if (this)
                {
                    DetermineLeadType();
                    if (_leadType != LeadType.None)
                        ResetSizes();
                }
            };
        }
#else
        private void Awake()
        {
            Destroy(this);
        }
#endif
    }
}
