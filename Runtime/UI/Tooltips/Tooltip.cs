using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace Crysc.UI.Tooltips
{
    [RequireComponent(typeof(TooltipPositioner))]
    public abstract class Tooltip<T> : MonoBehaviour
    {
        private const float _hoverLockTime = 0.75f;
        private const float _unhoverUnlockTime = 0.0625f;

        [SerializeField] private TooltipHoverPublisher PublisherInput;
        [SerializeField] protected RectTransform Container;
        [SerializeField] protected Image LockingImageInput;
        [SerializeField] protected Color LockingImageInactiveColor;

        [SerializeField] private bool LocksOnExtendedHover = true;
        [SerializeField] private bool SetInactiveWhenDismissed = true;

        protected virtual IEnumerable<Image> LockingImages =>
            LockingImageInput ? new[] { LockingImageInput } : Enumerable.Empty<Image>();
        private TooltipHoverPublisher Publisher => PublisherInput;
        private TooltipPositioner _positioner;
        private TooltipPositioner Positioner => _positioner ??= GetComponent<TooltipPositioner>();

        private ITooltipTargetProvider _currentTarget;
        private bool _isActive = true;

        private CryRoutine _tooltipPersistenceRoutine;
        private bool _isLocked;

        private void Awake() { DismissTooltip(); }

        private void OnEnable() { Publisher.Hovered += TargetHoveredEventHandler; }
        private void OnDisable() { Publisher.Hovered -= TargetHoveredEventHandler; }

        public virtual void SetActive(bool active)
        {
            _isActive = active;
            if (!_isActive) DismissTooltip();
        }

        protected virtual void PresentTooltip(T[] contents)
        {
            Container.gameObject.SetActive(true);
            foreach (Image image in LockingImages)
            {
                image.color = LockingImageInactiveColor;
                image.gameObject.SetActive(LocksOnExtendedHover);
            }
        }

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
                foreach (Image image in LockingImages) image.fillAmount = time / _hoverLockTime;
            }

            foreach (Image image in LockingImages)
            {
                image.color = Color.white;
                image.fillAmount = 1;
            }

            while (_isLocked)
            {
                yield return null;
                if (IsTargetHovered(_currentTarget) || Positioner.IsMouseOverTooltip())
                {
                    if (Mathf.Approximately(a: time, b: 0) == false)
                    {
                        time = 0f;
                        foreach (Image image in LockingImages) image.fillAmount = 1;
                    }

                    continue;
                }

                time += Time.deltaTime;
                if (time >= _unhoverUnlockTime) _isLocked = false;
                foreach (Image image in LockingImages) image.fillAmount = 1 - (time / _unhoverUnlockTime);
            }

            foreach (Image image in LockingImages) image.fillAmount = 0;
            DismissTooltip();
        }

        private static bool IsTargetHovered(ITooltipTargetProvider target)
        {
            if (target.IsHovered) return true;
            return target.IgnoreRaycastBlocking && target.GetSize().IsScreenPointWithin(Input.mousePosition);
        }
    }
}
