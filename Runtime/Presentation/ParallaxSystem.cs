using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Crysc.Presentation
{
    [DefaultExecutionOrder(-1)]
    public class ParallaxSystem : MonoBehaviour
    {
        public static ParallaxSystem I { get; private set; }

        [field: SerializeField] public float LayerWidth { get; private set; } = 20;
        [SerializeField] private Vector2 InitialFocalPoint;

        [field: SerializeField] public float PivotDistance { get; private set; } = 10;
        [field: SerializeField] public float Speed { get; set; }
        [field: SerializeField] public Vector2 MouseEffectModifier { get; set; } = Vector2.one;
        [field: SerializeField] public float CameraMouseAlignmentSpeed { get; set; } = 1f;

        public Vector2 FocalPoint { get; private set; }

        private readonly Dictionary<Transform, Vector3> _transformsBasePositions = new();
        private readonly Dictionary<ParallaxLayerConfig, HashSet<Transform>> _layersTransforms = new();
        private readonly Dictionary<ParallaxLayerConfig, float> _layersPivotDeltas = new();
        private readonly Dictionary<ParallaxLayerConfig, float> _layersMovementDeltas = new();
        private readonly Dictionary<ParallaxLayerConfig, float> _layersForeshorteningFactors = new();
        private readonly HashSet<Transform> _affectedBySpeed = new();

        private float _distance;
        private Camera _camera;
        private Transform _cameraTransform;
        private Vector2 _focalPointScreenOffset;
        private Vector2 _currentFocalDelta;

        private void Awake()
        {
            if (I != null)
            {
                Destroy(gameObject);
                return;
            }

            I = this;

            _camera = Camera.main;
            if (_camera)
            {
                FocalPoint = InitialFocalPoint;
                _focalPointScreenOffset = _camera.WorldToScreenPoint(FocalPoint);
            }
            else
            {
                Debug.LogWarning("No camera, defaulting to screen center focal point", gameObject);
                FocalPoint = Vector3.zero;
                _focalPointScreenOffset = new Vector3(
                    x: Screen.width / 2f,
                    y: Screen.height / 2f,
                    z: 0
                );
            }
        }

        private void Update()
        {
            Vector2 clampedPosition = Input.mousePosition;
            clampedPosition.x = Mathf.Clamp(value: clampedPosition.x, min: 0, max: Screen.width);
            clampedPosition.y = Mathf.Clamp(value: clampedPosition.y, min: 0, max: Screen.height);

            Vector2 focalDeltaPixels = clampedPosition - _focalPointScreenOffset;
            focalDeltaPixels *= MouseEffectModifier;
            Vector2 focalDelta = focalDeltaPixels / Screen.width;

            Vector2 focalDeltaDiff = focalDelta - _currentFocalDelta;
            _currentFocalDelta +=
                focalDeltaDiff * (Time.deltaTime % CameraMouseAlignmentSpeed / CameraMouseAlignmentSpeed);

            _distance += Speed * Time.deltaTime;
            UpdateRegistrants();
        }

        public void UpdateFocalPoint(Vector2 focalPoint)
        {
            if (!_camera)
            {
                Debug.LogWarning("No camera, focal point ignored");
                return;
            }

            FocalPoint = focalPoint;
            _focalPointScreenOffset = _camera.WorldToScreenPoint(FocalPoint);
        }

        public void Register(ParallaxLayerConfig layer, Transform registrant, bool isAffectedBySpeed = true)
        {
            if ((layer == null) || (registrant == null))
            {
                Debug.LogWarning(
                    $"ParallaxBackgroundRegistrar: Attempted to register null layer or registrant (layer: {(layer != null ? layer.name : null)}, registrant: {(registrant != null ? registrant.name : null)})"
                );
                return;
            }

            if (_transformsBasePositions.ContainsKey(registrant))
                return;
            _transformsBasePositions.Add(key: registrant, value: registrant.localPosition);
            if (!_layersTransforms.ContainsKey(layer))
            {
                _layersTransforms.Add(key: layer, value: new HashSet<Transform>());
                _layersPivotDeltas.Add(
                    key: layer,
                    value: (layer.DistanceFromObserver - PivotDistance) / layer.DistanceFromObserver
                );
                _layersMovementDeltas.Add(
                    key: layer,
                    value: 1 + ((PivotDistance - layer.DistanceFromObserver) / layer.DistanceFromObserver)
                );
                _layersForeshorteningFactors.Add(
                    key: layer,
                    value: 0.5f / (layer.DistanceFromObserver * layer.DistanceFromObserver)
                );
            }

            _layersTransforms[layer].Add(registrant);
            if (isAffectedBySpeed)
                _affectedBySpeed.Add(registrant);
        }

        public void Deregister(ParallaxLayerConfig layer, Transform registrant)
        {
            _transformsBasePositions.Remove(registrant);
            if (_layersTransforms.TryGetValue(key: layer, value: out HashSet<Transform> layersTransforms))
            {
                layersTransforms.Remove(registrant);
                if (layersTransforms.Count == 0)
                {
                    _layersTransforms.Remove(layer);
                    _layersPivotDeltas.Remove(layer);
                    _layersMovementDeltas.Remove(layer);
                    _layersForeshorteningFactors.Remove(layer);
                }
            }

            _affectedBySpeed.Remove(registrant);
        }

        public void ResetDistance()
        {
            _distance = 0;
        }

        private void UpdateRegistrants()
        {
            foreach ((ParallaxLayerConfig layer, HashSet<Transform> transforms) in _layersTransforms)
            {
                var foreshorteningFactor = _layersForeshorteningFactors[layer];
                var foreshorteningScale = Vector2.one - _currentFocalDelta * _currentFocalDelta * foreshorteningFactor;
                var adjustedLayerWidth = LayerWidth * foreshorteningScale.x;

                var movementDelta = _layersMovementDeltas[layer];
                float xDelta = _distance * movementDelta * foreshorteningScale.x;
                xDelta = (xDelta + adjustedLayerWidth / 2f) % adjustedLayerWidth
                    - adjustedLayerWidth / 2f; // xDelta range should be -LayerWidth/2 to LayerWidth/2

                var pivotDelta = _layersPivotDeltas[layer];

                foreach (Transform registrant in transforms)
                {
                    registrant.localPosition =
                        _transformsBasePositions[registrant] +
                        (_affectedBySpeed.Contains(registrant) ? Vector3.right * xDelta : Vector3.zero) +
                        (Vector3) (_currentFocalDelta * pivotDelta);

                    registrant.localScale = new Vector3(
                        x: foreshorteningScale.x,
                        y: foreshorteningScale.y,
                        z: 1f
                    );
                }
            }
        }
    }
}
