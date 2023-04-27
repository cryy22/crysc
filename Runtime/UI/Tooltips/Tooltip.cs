using System.Collections;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;

namespace Crysc.UI.Tooltips
{
    [RequireComponent(typeof(TooltipPositioner))]
    public abstract class Tooltip<T> : MonoBehaviour
    {
        private const float _hoverLockTime = 1f;
        private const float _unhoverUnlockTime = 0.05f;

        [SerializeField] private TooltipHoverPublisher PublisherInput;
        [SerializeField] protected RectTransform Container;
        [SerializeField] private bool LocksOnExtendedHover = true;
        [SerializeField] private bool SetInactiveWhenDismissed = true;

        private TooltipHoverPublisher Publisher => PublisherInput;
        private TooltipPositioner _positioner;
        private TooltipPositioner Positioner => _positioner ??= GetComponent<TooltipPositioner>();

        private bool _isActive = true;
        private bool _isLocked;
        private ITooltipTargetProvider _currentTarget;

        private CryRoutine _tooltipPersistenceRoutine;

        private void Awake() { DismissTooltip(); }

        private void OnEnable() { Publisher.Hovered += TargetHoveredEventHandler; }
        private void OnDisable() { Publisher.Hovered -= TargetHoveredEventHandler; }

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

            _tooltipPersistenceRoutine?.Stop();
            _currentTarget = null;
        }

        private void TargetHoveredEventHandler(object sender, TooltipHoverEventArgs e)
        {
            if (_isLocked) return;

            T[] contents = e.TooltipContent.Where(c => c is T).Cast<T>().ToArray();
            if (contents.Length == 0) return;
            if (ShouldPresentTooltip(contents) == false) return;

            PresentTooltip(contents);
            _currentTarget = e.TargetProvider;
            Positioner.UpdateTooltipPosition(targetDimensions: e.Dimensions);

            _tooltipPersistenceRoutine?.Stop();
            _tooltipPersistenceRoutine = new CryRoutine(enumerator: RunTooltipPersistence(), behaviour: this);
        }

        private IEnumerator RunTooltipPersistence()
        {
            _isLocked = false;
            var time = 0f;

            while (_isLocked == false)
            {
                yield return null;
                if (IsTargetHovered(_currentTarget) == false)
                {
                    DismissTooltip();
                    yield break;
                }

                if (LocksOnExtendedHover == false) continue;

                time += Time.deltaTime;
                if (time >= _hoverLockTime) _isLocked = true;
            }

            while (_isLocked)
            {
                yield return null;
                if (IsTargetHovered(_currentTarget) || Positioner.IsMouseOverTooltip())
                {
                    time = 0f;
                    continue;
                }

                time += Time.deltaTime;
                if (time >= _unhoverUnlockTime) _isLocked = false;
            }

            DismissTooltip();
        }

        private static bool IsTargetHovered(ITooltipTargetProvider target)
        {
            if (target.IsHovered) return true;
            return target.IgnoreRaycastBlocking && target.GetSize().IsScreenPointWithin(Input.mousePosition);
        }
    }
}
