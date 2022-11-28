using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crysc.UI
{
    public class UIParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Transform FocalPoint;
        [SerializeField] private List<ParallaxLayer> Layers = new();

        private readonly Dictionary<ParallaxLayer, Vector3> _layersInitialPositions = new();
        private Camera _camera;

        private void Awake()
        {
            _camera = Camera.main;
            foreach (ParallaxLayer layer in Layers)
                _layersInitialPositions.Add(key: layer, value: layer.Transform.position);
        }

        private void Update()
        {
            Vector2 focalPixelDelta = Input.mousePosition - _camera.WorldToScreenPoint(FocalPoint.position);
            Vector2 vocalDeltaRatio = Vector2.ClampMagnitude(
                vector: focalPixelDelta / _camera.pixelWidth,
                maxLength: 1
            );

            foreach (ParallaxLayer layer in Layers) UpdateLayer(layer: layer, vocalDeltaRatio: vocalDeltaRatio);
        }

        private void UpdateLayer(ParallaxLayer layer, Vector2 vocalDeltaRatio)
        {
            layer.Transform.position = _layersInitialPositions[layer] + new Vector3(
                x: vocalDeltaRatio.x * layer.Speed,
                y: vocalDeltaRatio.y * layer.Speed,
                z: 0
            );
        }

        [Serializable]
        public struct ParallaxLayer
        {
            public Transform Transform;
            public float Speed;
        }
    }
}
