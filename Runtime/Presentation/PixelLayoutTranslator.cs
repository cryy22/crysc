#region

using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

#endregion

namespace Crysc.Presentation
{
    // NOTE: assumes width in units is always 16, if it starts failing check whether CameraUnitWidthLocker is set to 16
    // and working properly

    [ExecuteAlways]
    public class PixelLayoutTranslator : MonoBehaviour
    {
        [field: SerializeField] public Vector2 ReferenceLayoutSize { get; private set; } = new(x: 1280, y: 720);

        [field: FormerlySerializedAs("<UseSizeReferenceSprite>k__BackingField")]
        [field: SerializeField] public bool UseSpriteRenderer { get; private set; }

        [field: FormerlySerializedAs("<SizeReferenceSpriteRenderer>k__BackingField")]
        [field: SerializeField]
        [field: ShowIf("UseSpriteRenderer")] public SpriteRenderer SpriteRenderer { get; private set; }

        [field: SerializeField] [field: ShowIf("@UseSpriteRenderer && RendererUsesSimpleDrawMode()")]
        public float ReferenceSpriteScaleFactor { get; private set; } = 1;

        [field: SerializeField] [field: DisableIf("@UseSpriteRenderer && RendererUsesSimpleDrawMode()")]
        public Vector2 ElementSize { get; private set; } = new(x: 16, y: 16);

        [ShowInInspector] public Vector2 ReferencePosition
        {
            get => GetReferencePosition();
            set => SetWithReferencePosition(value);
        }

        private Camera _camera;
        private Transform _transform;
        private Vector2 _windowUnitSize;
        private float _pixelsPerUnit;
        private Vector2 _cornerOffsetUnits;

        private void Awake()
        {
            _camera = Camera.main;
            _transform = transform;

            UpdateTranslationValues();
        }

        public Vector2 GetReferencePosition()
        {
            Vector2 unitPos = _transform.position;
            unitPos.y *= -1;
            unitPos += _windowUnitSize / 2; // shift world origin to top-right
            unitPos -= _cornerOffsetUnits; // set unitPos to position of top-right corner of element

            return new Vector2(
                x: Round(unitPos.x * _pixelsPerUnit),
                y: Round(unitPos.y * _pixelsPerUnit)
            );
        }

        public void SetWithReferencePosition(Vector2 referencePos)
        {
            Vector2 unitPos = referencePos / _pixelsPerUnit;
            unitPos += _cornerOffsetUnits;
            unitPos -= _windowUnitSize / 2;
            unitPos.y *= -1;

            _transform.position = new Vector2(
                x: Round(unitPos.x),
                y: Round(unitPos.y)
            );
        }

        private void UpdateTranslationValues()
        {
            _windowUnitSize = new Vector2(
                x: _camera.orthographicSize * 2 * _camera.aspect,
                y: _camera.orthographicSize * 2
            );

            _pixelsPerUnit = ReferenceLayoutSize.x / _windowUnitSize.x;
            _cornerOffsetUnits = ElementSize / _pixelsPerUnit / 2;
        }

        private static float Round(float value)
        {
            const float roundingStep = 1 / 80f;
            return Mathf.Round(value / roundingStep) * roundingStep;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _camera = Camera.main;
            if (_camera == null)
                return;

            _transform = transform;

            UpdateTranslationValues();

            if (!UseSpriteRenderer || !SpriteRenderer)
                return;

            if (SpriteRenderer.drawMode == SpriteDrawMode.Simple)
            {
                ElementSize =
                    SpriteRenderer.sprite.rect.size / ReferenceSpriteScaleFactor
                    * new Vector2(
                        x: Mathf.Abs(_transform.lossyScale.x),
                        y: Mathf.Abs(_transform.lossyScale.y)
                    );

                UpdateTranslationValues();
            }
            else
            {
                EditorApplication.delayCall += () =>
                {
                    if (!this || !SpriteRenderer)
                        return;

                    SpriteRenderer.size = ElementSize / _pixelsPerUnit;
                    SpriteRenderer.size = new Vector2(
                        x: Round(SpriteRenderer.size.x),
                        y: Round(SpriteRenderer.size.y)
                    );
                };
            }
        }

        private bool RendererUsesSimpleDrawMode()
        {
            return SpriteRenderer && (SpriteRenderer.drawMode == SpriteDrawMode.Simple);
        }
#endif
    }
}
