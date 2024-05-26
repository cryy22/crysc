using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Crysc.Presentation
{
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

        [SerializeField] private Transform FocalPoint;
        [SerializeField] private LayerSpeed[] LayerSpeeds;
        [SerializeField] private List<ParallaxLayer> Layers = new();

        private readonly Dictionary<Transform, Vector3> _transformsInitialPositions = new();
        private readonly Dictionary<Layer, List<Transform>> _layersTransforms = new();
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            foreach (Transform layerTransform in Layers.SelectMany(layer => layer.Transforms))
                _transformsInitialPositions.Add(key: layerTransform, value: layerTransform.localPosition);
        }

        private void Update()
        {
            Vector2 focalPixelDelta = Input.mousePosition - _camera.WorldToScreenPoint(FocalPoint.position);
            Vector2 vocalDeltaRatio = Vector2.ClampMagnitude(
                vector: focalPixelDelta / _camera.pixelWidth,
                maxLength: .66f
            );

            foreach (ParallaxLayer layer in Layers) UpdateLayer(layer: layer, vocalDeltaRatio: vocalDeltaRatio);
        }

        private void UpdateLayer(ParallaxLayer layer, Vector2 vocalDeltaRatio)
        {
            foreach (Transform layerTransform in layer.Transforms)
                layerTransform.localPosition = _transformsInitialPositions[layerTransform] + new Vector3(
                    x: vocalDeltaRatio.x * layer.Speed * layerTransform.lossyScale.x,
                    y: vocalDeltaRatio.y * layer.Speed * layerTransform.lossyScale.y,
                    z: 0
                );
        }

        [Serializable]
        public struct ParallaxLayer
        {
            public string Name;
            public Transform[] Transforms;
            public float Speed;
        }

        [Serializable]
        private struct LayerSpeed
        {
            public Layer Layer;
            public float Speed;
        }
    }
}
