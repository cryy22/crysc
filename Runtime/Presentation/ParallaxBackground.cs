using System;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Crysc.Presentation
{
    [DefaultExecutionOrder(-1)]
    public class ParallaxBackground : MonoBehaviour
    {
        public enum Layer
        {
            SkyMesa,
            Formation,
            CactusForest,
            PathBG,
            TrailContent,
            PathFG,
            FGFoliage,
            FGCactus0,
            FGCactus1,
            FGCactus2,
        }

        private const float _layerWidth = 20;

        public static ParallaxBackground I { get; private set; }

        [SerializeField] private Transform FocalPoint;
        [SerializeField] private LayerSpeed[] LayerSpeeds;

        public float Speed { get; set; }

        private readonly Dictionary<Transform, Vector3> _transformsBasePositions = new();
        private readonly Dictionary<Layer, HashSet<Transform>> _layersTransforms = new();
        private readonly HashSet<Transform> _affectedBySpeed = new();

        private float _distance;
        private Camera _camera;
        private Layer[] _layers;

        private void Awake()
        {
            if (I != null)
            {
                Destroy(gameObject);
                return;
            }

            I = this;

            _camera = Camera.main;
            _layers = (Layer[]) Enum.GetValues(typeof(Layer));

            foreach (Layer layer in _layers)
                _layersTransforms[layer] = new HashSet<Transform>();
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

        public void Register(Layer layer, Transform registrant, bool isAffectedBySpeed = true)
        {
            _transformsBasePositions.Add(key: registrant, value: registrant.localPosition);
            _layersTransforms[layer].Add(registrant);
            if (isAffectedBySpeed) _affectedBySpeed.Add(registrant);
        }

        public void Deregister(Layer layer, Transform registrant)
        {
            _transformsBasePositions.Remove(registrant);
            _layersTransforms[layer].Remove(registrant);
            _affectedBySpeed.Remove(registrant);
        }

        public void ResetDistance() { _distance = 0; }

        private void UpdateRegistrants(Vector2 vocalDeltaRatio, float distance)
        {
            foreach (LayerSpeed layerSpeed in LayerSpeeds)
            {
                float xDelta = distance * layerSpeed.Speed % _layerWidth;
                foreach (Transform registrant in _layersTransforms[layerSpeed.Layer])
                    registrant.localPosition =
                        _transformsBasePositions[registrant] +
                        (_affectedBySpeed.Contains(registrant) ? Vector3.right * xDelta : Vector3.zero) +
                        new Vector3(
                            x: vocalDeltaRatio.x * layerSpeed.Speed * registrant.lossyScale.x,
                            y: vocalDeltaRatio.y * layerSpeed.Speed * registrant.lossyScale.y,
                            z: 0
                        );
            }
        }

        [Serializable]
        private struct LayerSpeed
        {
            public Layer Layer;
            public float Speed;
        }
    }
}
