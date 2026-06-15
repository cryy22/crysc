using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Crysc.Presentation
{
    // NOTE: assumes width in units is always 16, if it starts failing check whether CameraUnitWidthLocker is set to 16
    // and working properly

    [ExecuteAlways]
    public class PixelLayoutTranslator : MonoBehaviour
    {
        private const float _roundingStep = 1 / 8f;
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
                x: Mathf.Round(unitPos.x * _pixelsPerUnit / _roundingStep) * _roundingStep,
                y: Mathf.Round(unitPos.y * _pixelsPerUnit / _roundingStep) * _roundingStep
            );
        }

        public void SetWithReferencePosition(Vector2 referencePos)
        {
            Vector2 unitPos = referencePos / _pixelsPerUnit;
            unitPos += _cornerOffsetUnits;
            unitPos -= _windowUnitSize / 2;
            unitPos.y *= -1;
            _transform.position = unitPos;
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

#if UNITY_EDITOR
        private bool _wasUsingSpriteRenderer;

        private void OnValidate()
        {
            _camera = Camera.main;
            if (_camera == null)
                return;

            _transform = transform;

            UpdateTranslationValues();

            if (!UseSpriteRenderer || !SpriteRenderer)
            {
                _wasUsingSpriteRenderer = false;
                return;
            }

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
            else if (!_wasUsingSpriteRenderer)
            {
                ElementSize = SpriteRenderer.size * _pixelsPerUnit;
                UpdateTranslationValues();
            }
            else
            {
                EditorApplication.delayCall += () =>
                {
                    if (!this || !SpriteRenderer)
                        return;

                    Debug.Log(
                        $"ElementSize: {ElementSize}, PixelsPerUnit: {_pixelsPerUnit}, WindowUnitSize: {_windowUnitSize}"
                    );

                    SpriteRenderer.size = ElementSize / _pixelsPerUnit;
                };
            }

            _wasUsingSpriteRenderer = true;
        }

        private bool RendererUsesSimpleDrawMode()
        {
            return SpriteRenderer && (SpriteRenderer.drawMode == SpriteDrawMode.Simple);
        }
#endif
    }
}
