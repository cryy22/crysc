using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Crysc.Presentation
{
    [ExecuteAlways]
    public class RelativeSizeMaintainer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer LeadRenderer;
        [SerializeField] private FollowBoxCollider[] FollowBoxColliders;
        [SerializeField] private FollowRenderer[] FollowRenderers;
        [SerializeField] private FollowText[] FollowTexts;

        [Serializable]
        private struct FollowBoxCollider
        {
            [SerializeField] public BoxCollider2D Collider;
            [SerializeField] public Vector2 Offset;
        }

        [Serializable]
        private struct FollowRenderer
        {
            [SerializeField] public SpriteRenderer Renderer;
            [SerializeField] public Vector2 Offset;
        }

        [Serializable]
        private struct FollowText
        {
            [SerializeField] public TMP_Text Text;
            [SerializeField] public Vector2 Offset;
        }

        private Vector2 _currentSize;

        private void Update()
        {
            if (!LeadRenderer || (LeadRenderer.size == _currentSize))
                return;

            ResetSizes();
        }

        private void ResetSizes()
        {
            _currentSize = LeadRenderer.size * LeadRenderer.transform.lossyScale;

            foreach (FollowBoxCollider followBoxCollider in FollowBoxColliders)
            {
                if (!followBoxCollider.Collider)
                    continue;

                followBoxCollider.Collider.size = new Vector2(
                    x: _currentSize.x / followBoxCollider.Collider.transform.lossyScale.x + followBoxCollider.Offset.x,
                    y: _currentSize.y / followBoxCollider.Collider.transform.lossyScale.y + followBoxCollider.Offset.y
                );
            }

            foreach (FollowRenderer followRenderer in FollowRenderers)
            {
                if (!followRenderer.Renderer)
                    continue;

                Transform followRendererTransform = followRenderer.Renderer.transform;
                followRenderer.Renderer.size = new Vector2(
                    x: _currentSize.x / followRendererTransform.lossyScale.x + followRenderer.Offset.x,
                    y: _currentSize.y / followRendererTransform.lossyScale.y + followRenderer.Offset.y
                );
            }

            foreach (FollowText followText in FollowTexts)
            {
                if (!followText.Text)
                    continue;

                RectTransform followTextRectTransform = followText.Text.rectTransform;
                followTextRectTransform.sizeDelta = new Vector2(
                    x: _currentSize.x / followTextRectTransform.lossyScale.x + followText.Offset.x,
                    y: _currentSize.y / followTextRectTransform.lossyScale.y + followText.Offset.y
                );
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                if (this)
                    ResetSizes();
            };
        }
#endif
    }
}
