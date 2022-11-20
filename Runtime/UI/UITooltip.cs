using System;
using Crysc.Registries;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.UI
{
    public abstract class UITooltip<T> : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
        where T : Component
    {
        [SerializeField] private Registry<T> Registry;
        [SerializeField] protected GameObject Container;

        [SerializeField] private bool PersistsOnTooltipHover;

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

        private void HoveredEventHandler(object sender, EventArgs _) { ShowTooltip(sender as T); }

        private void UnhoveredEventHandler(object sender, EventArgs _) { HideTooltip(); }

        public void OnPointerEnter(PointerEventData _)
        {
            if (PersistsOnTooltipHover) Container.SetActive(true);
        }

        public void OnPointerExit(PointerEventData _) { HideTooltip(); }
    }
}
