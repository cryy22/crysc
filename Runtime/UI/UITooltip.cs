using System;
using Crysc.Registries;
using UnityEngine;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace Crysc.UI
{
    public abstract class UITooltip<T> : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
        where T : Object
    {
        [SerializeField] private Registry<T> Registry;
        [SerializeField] protected GameObject Container;

        [SerializeField] private bool PersistsOnTooltipHover;
        [SerializeField] private bool MoveToTargetPosition;

        private Camera _camera;

        private void Awake() { _camera = Camera.main; }

        protected virtual void OnEnable()
        {
            Registry.Hovered += HoveredEventHandler;
            Registry.Unhovered += UnhoveredEventHandler;
        }

        protected virtual void OnDisable()
        {
            Registry.Hovered -= HoveredEventHandler;
            Registry.Unhovered -= UnhoveredEventHandler;
        }

        protected virtual void ShowTooltip(T target) { Container.SetActive(true); }
        private void HideTooltip() { Container.SetActive(false); }

        private void HoveredEventHandler(object sender, RegistryEventArgs<T> e)
        {
            ShowTooltip(sender as T);

            if (MoveToTargetPosition && e.Registrar is IMouseEventRegistrar<T> meRegistrar) MoveTooltip(meRegistrar);
        }

        private void MoveTooltip(IMouseEventRegistrar<T> registrar)
        {
            Bounds bounds = registrar.Bounds;
            var worldPoint = new Vector3(
                x: bounds.max.x - (bounds.extents.x / 3),
                y: bounds.max.y - (bounds.extents.y / 3),
                z: bounds.center.z
            );
            Vector3 screenPoint = _camera.WorldToScreenPoint(worldPoint);
            transform.position = new Vector3(
                x: screenPoint.x,
                y: screenPoint.y,
                z: transform.position.z
            );
        }

        private void UnhoveredEventHandler(object sender, EventArgs _) { HideTooltip(); }

        public void OnPointerEnter(PointerEventData _)
        {
            if (PersistsOnTooltipHover) Container.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _) { HideTooltip(); }
    }
}
