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

        public float Speed { get; set; }

        private readonly Dictionary<Transform, Vector3> _transformsBasePositions = new();
        private readonly Dictionary<ParallaxLayerConfig, HashSet<Transform>> _layersTransforms = new();
        private readonly HashSet<Transform> _affectedBySpeed = new();

        private float _distance;
        private Camera _camera;

        private void Awake()
        {
            if (I != null)
            {
                Destroy(gameObject);
                return;
            }

            I = this;

            _camera = Camera.main;
        }

        private void Update()
        {
            Vector2 focalPixelDelta = Input.mousePosition - _camera.WorldToScreenPoint(FocalPoint.position);
            Vector2 vocalDeltaRatio = Vector2.ClampMagnitude(
                vector: focalPixelDelta / _camera.pixelWidth,
                maxLength: .66f
            );

            _distance += Speed * Time.deltaTime;
            UpdateRegistrants(vocalDeltaRatio: vocalDeltaRatio, distance: _distance);
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

        private void UpdateRegistrants(Vector2 vocalDeltaRatio, float distance)
        {
            foreach (var (layer, transforms) in _layersTransforms)
            {
                float xDelta = distance * layer.Speed % LayerWidth;
                foreach (Transform registrant in transforms)
                    registrant.localPosition =
                        _transformsBasePositions[registrant] +
                        (_affectedBySpeed.Contains(registrant) ? Vector3.right * xDelta : Vector3.zero) +
                        new Vector3(
                            x: vocalDeltaRatio.x * layer.Speed * registrant.lossyScale.x,
                            y: vocalDeltaRatio.y * layer.Speed * registrant.lossyScale.y,
                            z: 0
                        );
            }
        }
    }
}
