using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Crysc.Presentation
{
    [DefaultExecutionOrder(-1)]
    public class ParallaxSystem : MonoBehaviour
    {
        public const float LayerWidth = 20;

        public static ParallaxSystem I { get; private set; }

        [SerializeField] private Transform FocalPoint;

        [field: SerializeField] public float Speed { get; set; } = 0;
        [field: SerializeField] public Vector2 MouseEffectModifier { get; set; } = Vector2.one;

        private readonly Dictionary<Transform, Vector3> _transformsBasePositions = new();
        private readonly Dictionary<ParallaxLayerConfig, HashSet<Transform>> _layersTransforms = new();
        private readonly HashSet<Transform> _affectedBySpeed = new();

        private float _distance;
        private Camera _camera;
        private Vector3 _focalPoint;

        private void Awake()
        {
            if (I != null)
            {
                Destroy(gameObject);
                return;
            }

            I = this;

            _camera = Camera.main;
            if (_camera == null)
                _focalPoint = _camera.WorldToScreenPoint(FocalPoint.position);
            else
                _focalPoint = Vector3.zero;       
        }

        private void Update()
        {
            var clampedPosition = Input.mousePosition;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, 0, Screen.width);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0, Screen.height);
            
            Vector2 focalPixelDelta = clampedPosition - _focalPoint;
            focalPixelDelta *= MouseEffectModifier;
            
            Vector2 focalDeltaRatio = Vector2.ClampMagnitude(
                vector: focalPixelDelta / Screen.width,
                maxLength: .66f
            );

            _distance += Speed * Time.deltaTime;
            UpdateRegistrants(focalDeltaRatio: focalDeltaRatio, distance: _distance);
        }

        public void Register(ParallaxLayerConfig layer, Transform registrant, bool isAffectedBySpeed = true)
        {
            if (layer == null || registrant == null)
            {
                Debug.LogWarning($"ParallaxBackgroundRegistrar: Attempted to register null layer or registrant (layer: {(layer != null ? layer.name : null)}, registrant: {(registrant != null ? registrant.name : null)})");
                return;
            }

            _transformsBasePositions.Add(key: registrant, value: registrant.localPosition);
            if (!_layersTransforms.ContainsKey(layer))
                _layersTransforms.Add(layer, new HashSet<Transform>());

            _layersTransforms[layer].Add(registrant);
            if (isAffectedBySpeed)
                _affectedBySpeed.Add(registrant);
        }

        public void Deregister(ParallaxLayerConfig layer, Transform registrant)
        {
            _transformsBasePositions.Remove(registrant);
            if (_layersTransforms.TryGetValue(layer, out HashSet<Transform> layersTransforms))
                layersTransforms.Remove(registrant);
            _affectedBySpeed.Remove(registrant);
        }

        public void ResetDistance()
        {
            _distance = 0;
        }

        private void UpdateRegistrants(Vector2 focalDeltaRatio, float distance)
        {
            foreach (var (layer, transforms) in _layersTransforms)
            {
                float xDelta = distance * layer.Speed % LayerWidth;
                foreach (Transform registrant in transforms)
                    registrant.localPosition =
                        _transformsBasePositions[registrant] +
                        (_affectedBySpeed.Contains(registrant) ? Vector3.right * xDelta : Vector3.zero) +
                        new Vector3(
                            x: focalDeltaRatio.x * layer.Speed * registrant.lossyScale.x,
                            y: focalDeltaRatio.y * layer.Speed * registrant.lossyScale.y,
                            z: 0
                        );
            }
        }
    }
}
