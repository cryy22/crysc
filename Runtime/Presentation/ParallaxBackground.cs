using System;
using System.Collections;
using System.Collections.Generic;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.Presentation
{
    public class ParallaxBackground : MonoBehaviour
    {
        [SerializeField] private Transform FocalPoint;
        [SerializeField] private List<ParallaxLayer> Layers = new();
        [SerializeField] private Image CurtainPanel;

        private readonly Dictionary<ParallaxLayer, Vector3> _layersInitialPositions = new();
        private Camera _camera;
        private CryRoutine _curtainRoutine;
        private Color _curtainClosedColor;
        private Color _curtainOpenColor;

        private void Awake()
        {
            _camera = Camera.main;
            foreach (ParallaxLayer layer in Layers)
                _layersInitialPositions.Add(key: layer, value: layer.Transform.position);
        }

        private void Start()
        {
            if (CurtainPanel)
            {
                Color initialColor = CurtainPanel.color;
                _curtainClosedColor = new Color(r: initialColor.r, g: initialColor.g, b: initialColor.b, a: 0.5f);
                _curtainOpenColor = new Color(r: initialColor.r, g: initialColor.g, b: initialColor.b, a: 0);
                CurtainPanel.color = _curtainOpenColor;
            }
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

        public void SetCurtain(bool isClosing)
        {
            _curtainRoutine?.Stop();
            _curtainRoutine = new CryRoutine(enumerator: RunCurtain(isClosing: isClosing), behaviour: this);
        }

        private IEnumerator RunCurtain(bool isClosing)
        {
            Color initialColor = CurtainPanel.color;
            Color targetColor = isClosing ? _curtainClosedColor : _curtainOpenColor;
            var t = 0f;

            while (t < 1)
            {
                t += Time.deltaTime / 1f;
                CurtainPanel.color = Color.Lerp(a: initialColor, b: targetColor, t: t);
                yield return null;
            }

            CurtainPanel.color = targetColor;
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
