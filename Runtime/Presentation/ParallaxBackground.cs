using System;
using System.Collections.Generic;
using UnityEngine;

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

        public static ParallaxBackground I { get; private set; }

        [SerializeField] private Transform FocalPoint;
        [SerializeField] private LayerSpeed[] LayerSpeeds;

        private readonly Dictionary<Transform, Vector3> _transformsInitialPositions = new();
        private readonly Dictionary<Layer, HashSet<Transform>> _layersTransforms = new();
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

            UpdateRegistrants(vocalDeltaRatio: vocalDeltaRatio);
        }

        public void Register(Layer layer, Transform registrant)
        {
            _transformsInitialPositions.Add(key: registrant, value: registrant.localPosition);
            _layersTransforms[layer].Add(registrant);
        }

        public void Deregister(Layer layer, Transform registrant)
        {
            _transformsInitialPositions.Remove(registrant);
            _layersTransforms[layer].Remove(registrant);
        }

        private void UpdateRegistrants(Vector2 vocalDeltaRatio)
        {
            foreach (LayerSpeed layerSpeed in LayerSpeeds)
            foreach (Transform registrant in _layersTransforms[layerSpeed.Layer])
                registrant.localPosition = _transformsInitialPositions[registrant] + new Vector3(
                    x: vocalDeltaRatio.x * layerSpeed.Speed * registrant.lossyScale.x,
                    y: vocalDeltaRatio.y * layerSpeed.Speed * registrant.lossyScale.y,
                    z: 0
                );
        }

        [Serializable]
        private struct LayerSpeed
        {
            public Layer Layer;
            public float Speed;
        }
    }
}
