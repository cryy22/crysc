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

        [field: SerializeField] public float LayerWidth { get; private set; } = 25;
        [SerializeField] private Transform FocalPoint;

        [field: SerializeField] public float Speed { get; set; } = 0;
        // [field: SerializeField] public float CameraSpeed { get; private set; } = 0;
        [field: SerializeField] public Vector2 MouseEffectModifier { get; set; } = Vector2.one;

        private readonly Dictionary<Transform, Vector3> _transformsBasePositions = new();
        private readonly Dictionary<ParallaxLayerConfig, HashSet<Transform>> _layersTransforms = new();
        private readonly HashSet<Transform> _affectedBySpeed = new();

        private float _distance;
        private Camera _camera;
        private Transform _cameraTransform;
        private Vector3 _cameraBasePosition;
        private Vector3 _focalPoint;
        private Vector3 _focalPointScreenOffset;

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
                // _cameraTransform = _camera.transform;
                // _cameraBasePosition = _cameraTransform.localPosition;
                _focalPoint = FocalPoint ? FocalPoint.position : Vector3.zero;
                _focalPointScreenOffset = _camera.WorldToScreenPoint(_focalPoint);
            }
            else
            {
                _focalPoint = Vector3.zero;
                _focalPointScreenOffset = new Vector3(
                    x: Screen.width / 2f,
                    y: Screen.height / 2f,
                    z: 0
                );
            }       
        }

        private void Update()
        {
            var clampedPosition = Input.mousePosition;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, 0, Screen.width);
            clampedPosition.y = Mathf.Clamp(clampedPosition.y, 0, Screen.height);
            
            Vector2 focalPixelDelta = clampedPosition - _focalPointScreenOffset;
            focalPixelDelta *= -1;
            focalPixelDelta *= MouseEffectModifier;
            
            Vector2 focalDeltaRatio = Vector2.ClampMagnitude(
                vector: focalPixelDelta / Screen.width,
                maxLength: .66f
            );

            _distance += Speed * Time.deltaTime;
            // if (_camera)
            //     UpdateCamera(focalDeltaRatio);
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
        
        // private void UpdateCamera(Vector2 focalDeltaRatio)
        // {
        //     _cameraTransform.localPosition = _cameraBasePosition + new Vector3(
        //         x: focalDeltaRatio.x * CameraSpeed * _cameraTransform.lossyScale.x,
        //         y: focalDeltaRatio.y * CameraSpeed * _cameraTransform.lossyScale.y,
        //         z: 0
        //     );
        //     _cameraTransform.LookAt(_focalPoint);
        // }
        
        private void UpdateRegistrants(Vector2 focalDeltaRatio, float distance)
        {
            foreach (var (layer, transforms) in _layersTransforms)
            {
                float xDelta = distance * layer.Speed;
                xDelta = (xDelta + LayerWidth / 2f) % LayerWidth - LayerWidth / 2f; // xDelta range should be -LayerWidth/2 to LayerWidth/2
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
