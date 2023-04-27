using System;
using System.Linq;
using UnityEngine;

namespace Crysc.UI.Tooltips
{
    [RequireComponent(typeof(TooltipPositioner))]
    public abstract class Tooltip<T> : MonoBehaviour
    {
        [SerializeField] private TooltipHoverPublisher PublisherInput;
        [SerializeField] protected RectTransform Container;
        [SerializeField] private bool SetInactiveWhenDismissed = true;

        private TooltipPositioner _positioner;
        private TooltipPositioner Positioner => _positioner ??= GetComponent<TooltipPositioner>();

        private TooltipHoverPublisher Publisher => PublisherInput;
        private ITooltipTargetProvider _currentTarget;
        private bool _isActive = true;

        private void Awake() { DismissTooltip(); }

        private void OnEnable() { Publisher.Hovered += HoveredEventHandler; }
        private void OnDisable() { Publisher.Hovered -= HoveredEventHandler; }

        public virtual void SetActive(bool active)
        {
            _isActive = active;
            if (!_isActive) DismissTooltip();
        }

        protected virtual void PresentTooltip(T[] contents) { Container.gameObject.SetActive(true); }

        protected virtual bool ShouldPresentTooltip(T[] contents) { return _isActive; }

        protected virtual void DismissTooltip()
        {
            Positioner.HaltPositioning();
            if (SetInactiveWhenDismissed) Container.gameObject.SetActive(false);

            if (_currentTarget == null) return;
            _currentTarget.Unhovered -= UnhoveredEventHandler;
            _currentTarget = null;
        }

        private void HoveredEventHandler(object sender, TooltipHoverEventArgs e)
        {
            T[] contents = e.TooltipContent.Where(c => c is T).Cast<T>().ToArray();
            if (contents.Length == 0) return;
            if (ShouldPresentTooltip(contents) == false) return;

            PresentTooltip(contents);
            UpdateCurrentTarget(e.TargetProvider);
            Positioner.UpdateTooltipPosition(targetBounds: e.Bounds);
        }

        private void UpdateCurrentTarget(ITooltipTargetProvider target)
        {
            if (_currentTarget != null) _currentTarget.Unhovered -= UnhoveredEventHandler;
            _currentTarget = target;
            _currentTarget.Unhovered += UnhoveredEventHandler;
        }

        private void UnhoveredEventHandler(object sender, EventArgs _) { DismissTooltip(); }
    }
}
