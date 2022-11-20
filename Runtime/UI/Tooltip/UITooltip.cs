using System;
using UnityEngine;

namespace Crysc.UI.Tooltip
{
    public abstract class UITooltip<T> : MonoBehaviour where T : Component
    {
        [SerializeField] private TooltipRegistry<T> Registry;
        [SerializeField] private GameObject Container;

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

        protected virtual void HideTooltip() { Container.SetActive(false); }

        private void HoveredEventHandler(object sender, EventArgs _) { ShowTooltip(sender as T); }

        private void UnhoveredEventHandler(object sender, EventArgs _) { HideTooltip(); }
    }
}
