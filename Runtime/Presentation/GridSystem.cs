#region

using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Crysc.Presentation
{
    [ExecuteAlways]
    public class GridSystem : MonoBehaviour
    {
#if UNITY_EDITOR
        [field: SerializeField] public Vector2 ReferenceResolution { get; private set; } = new(x: 1280, y: 720);
        [field: SerializeField] public Color GridColor { get; private set; } = Color.orangeRed;
        [field: SerializeField] public float GridLineWidth = 2f;
        [field: SerializeField, ValueDropdown("GetSortingLayers")]
        public string SortingLayerName { get; private set; }

        [field: SerializeField] public MarginsType Margins { get; private set; }

        [field: Header("Guides")]
        [field: SerializeField] public float[] FieldWidths { get; private set; }
        [field: SerializeField] public float[] FieldHeights { get; private set; }
        [field: SerializeField] public float GutterWidth { get; private set; }
        [field: SerializeField] public float GutterHeight { get; private set; }

        private Camera _camera;
        private Canvas _canvas;
        private CanvasScaler _scaler;

        private RectTransform _marginBox;

        private void OnValidate()
        {
            EditorApplication.delayCall += () =>
            {
                if (this)
                    Refresh();
            };
        }

        private void Refresh()
        {
            // create missing objects
            _camera = Camera.main;
            if (!_canvas)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                    DestroyImmediate(transform.GetChild(i).gameObject);

                _canvas = new GameObject("GridSystemCanvas").AddComponent<Canvas>();
                _scaler = _canvas.gameObject.AddComponent<CanvasScaler>();
                _canvas.transform.SetParent(transform);
            }

            _canvas.worldCamera = _camera;
            _canvas.renderMode = RenderMode.ScreenSpaceCamera;
            _canvas.sortingLayerName = SortingLayerName;
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _scaler.matchWidthOrHeight = 0;
            _scaler.referenceResolution = ReferenceResolution;

            if (!_marginBox)
            {
                _marginBox = new GameObject("MarginBox").AddComponent<RectTransform>();
                _marginBox.SetParent(_canvas.transform);
                _marginBox.localScale = Vector3.one;
                _marginBox.localPosition = Vector3.zero;
                _marginBox.anchorMin = Vector3.zero;
                _marginBox.anchorMax = Vector3.one;
            }

            _marginBox.offsetMin = new Vector2(x: Margins.Left, y: Margins.Bottom);
            _marginBox.offsetMax = new Vector2(x: -Margins.Right, y: -Margins.Top);

            for (int i = _marginBox.transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(_marginBox.transform.GetChild(i).gameObject);

            var prototypeLine = new GameObject("PrototypeLine").AddComponent<Image>();
            prototypeLine.rectTransform.SetParent(_canvas.transform);
            prototypeLine.rectTransform.localScale = Vector3.one;
            prototypeLine.rectTransform.localPosition = Vector3.zero;
            prototypeLine.color = GridColor;

            Image horizontalPrototypeLine = Instantiate(original: prototypeLine, parent: _marginBox);
            horizontalPrototypeLine.gameObject.name = "HorizontalLine";
            horizontalPrototypeLine.rectTransform.pivot = new Vector2(x: 0.5f, y: 1);
            horizontalPrototypeLine.rectTransform.anchorMin = new Vector2(x: 0, y: 1);
            horizontalPrototypeLine.rectTransform.anchorMax = new Vector2(x: 1, y: 1);
            horizontalPrototypeLine.rectTransform.sizeDelta = new Vector2(x: 0, y: GridLineWidth);

            Image verticalPrototypeLine = Instantiate(original: prototypeLine, parent: _marginBox);
            verticalPrototypeLine.gameObject.name = "VerticalLine";
            verticalPrototypeLine.rectTransform.pivot = new Vector2(x: 0, y: 0.5f);
            verticalPrototypeLine.rectTransform.anchorMin = new Vector2(x: 0, y: 0);
            verticalPrototypeLine.rectTransform.anchorMax = new Vector2(x: 0, y: 1);
            verticalPrototypeLine.rectTransform.sizeDelta = new Vector2(x: GridLineWidth, y: 0);

            try
            {
                // draw lines
                Image top = Instantiate(original: prototypeLine, parent: _marginBox);
                top.gameObject.name = "Margin (Top)";
                top.rectTransform.pivot = new Vector2(x: 0.5f, y: 0);
                top.rectTransform.anchorMin = new Vector2(x: 0, y: 1);
                top.rectTransform.anchorMax = new Vector2(x: 1, y: 1);
                top.rectTransform.sizeDelta = new Vector2(x: GridLineWidth * 2, y: GridLineWidth);

                Image bottom = Instantiate(original: prototypeLine, parent: _marginBox);
                bottom.gameObject.name = "Margin (Bottom)";
                bottom.rectTransform.pivot = new Vector2(x: 0.5f, y: 1);
                bottom.rectTransform.anchorMin = new Vector2(x: 0, y: 0);
                bottom.rectTransform.anchorMax = new Vector2(x: 1, y: 0);
                bottom.rectTransform.sizeDelta = new Vector2(x: GridLineWidth * 2, y: GridLineWidth);

                Image left = Instantiate(original: prototypeLine, parent: _marginBox);
                left.gameObject.name = "Margin (Left)";
                left.rectTransform.pivot = new Vector2(x: 1, y: 0.5f);
                left.rectTransform.anchorMin = new Vector2(x: 0, y: 0);
                left.rectTransform.anchorMax = new Vector2(x: 0, y: 1);
                left.rectTransform.sizeDelta = new Vector2(x: GridLineWidth, y: GridLineWidth * 2);

                Image right = Instantiate(original: prototypeLine, parent: _marginBox);
                right.gameObject.name = "Margin (Right)";
                right.rectTransform.pivot = new Vector2(x: 0, y: 0.5f);
                right.rectTransform.anchorMin = new Vector2(x: 1, y: 0);
                right.rectTransform.anchorMax = new Vector2(x: 1, y: 1);
                right.rectTransform.sizeDelta = new Vector2(x: GridLineWidth, y: GridLineWidth * 2);

                var yScan = 0f;
                var initialField = true;
                foreach (float fieldHeight in FieldHeights)
                {
                    Image line;
                    if (!initialField)
                    {
                        yScan -= GutterHeight;
                        line = Instantiate(original: horizontalPrototypeLine, parent: _marginBox);
                        line.rectTransform.pivot = new Vector2(x: 0.5f, y: 0);
                        line.rectTransform.anchoredPosition = new Vector2(x: 0, y: yScan);
                    }

                    yScan -= fieldHeight;
                    line = Instantiate(original: horizontalPrototypeLine, parent: _marginBox);
                    line.rectTransform.anchoredPosition = new Vector2(x: 0, y: yScan);
                    initialField = false;
                }

                var xScan = 0f;
                initialField = true;
                foreach (float fieldWidth in FieldWidths)
                {
                    Image line;
                    if (!initialField)
                    {
                        xScan += GutterWidth;
                        line = Instantiate(original: verticalPrototypeLine, parent: _marginBox);
                        line.rectTransform.pivot = new Vector2(x: 1, y: 0.5f);
                        line.rectTransform.anchoredPosition = new Vector2(x: xScan, y: 0);
                    }

                    xScan += fieldWidth;
                    line = Instantiate(original: verticalPrototypeLine, parent: _marginBox);
                    line.rectTransform.anchoredPosition = new Vector2(x: xScan, y: 0);
                    initialField = false;
                }
            }
            finally
            {
                DestroyImmediate(prototypeLine.gameObject);
                DestroyImmediate(horizontalPrototypeLine.gameObject);
                DestroyImmediate(verticalPrototypeLine.gameObject);
            }
        }

        private IEnumerable<string> GetSortingLayers()
        {
            return SortingLayer.layers.Select(l => l.name);
        }

        [Serializable]
        public struct MarginsType
        {
            [BoxGroup("Margins")]
            [SerializeField, HorizontalGroup("Margins/Row"), LabelText("T"), LabelWidth(10)] public float Top;
            [SerializeField, HorizontalGroup("Margins/Row"), LabelText("R"), LabelWidth(10)] public float Right;
            [SerializeField, HorizontalGroup("Margins/Row"), LabelText("B"), LabelWidth(10)] public float Bottom;
            [SerializeField, HorizontalGroup("Margins/Row"), LabelText("L"), LabelWidth(10)] public float Left;
        }
#else // IF NOT THE UNITY EDITOR
        private void Awake()
        {
            Destroy(gameObject);
        }
#endif
    }
}
