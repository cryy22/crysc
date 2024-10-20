using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Crysc.Helpers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Crysc.UI.Tooltips
{
    [RequireComponent(typeof(TooltipPositioner))]
    public abstract class Tooltip<T> : MonoBehaviour, IPointerClickHandler
    {
        private const float _hoverLockTime = 0.75f;
        private const float _unhoverUnlockTime = 0.125f;
        private static TooltipPublisher Publisher => TooltipPublisher.I;

        [SerializeField] protected RectTransform Container;
        [SerializeField] protected CanvasGroup CanvasGroup;
        [SerializeField] protected Image LockingImageInput;
        [SerializeField] protected Color LockingImageInactiveColor;

        [SerializeField] private bool LocksOnExtendedHover = true;
        [SerializeField] private bool SetInactiveWhenDismissed = true;

        protected virtual IEnumerable<Image> LockingImages =>
            LockingImageInput ? new[] { LockingImageInput } : Enumerable.Empty<Image>();

        private TooltipPositioner Positioner { get; set; }

        private ITooltipTargetProvider _currentTarget;
        private bool _isActive = true;

        private CryRoutine _tooltipPersistenceRoutine;
        private bool _isLocked;
        private TooltipEventArgs _eventDuringLock;

        private void Awake()
        {
            Positioner = GetComponent<TooltipPositioner>();
            DismissTooltip();
        }

        private void OnEnable() { Publisher.Hovered += TargetHoveredEventHandler; }
        private void OnDisable() { Publisher.Hovered -= TargetHoveredEventHandler; }

        public void OnPointerClick(PointerEventData _) { Publisher.RegisterClick(_currentTarget); }

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

            if (CanvasGroup) CanvasGroup.alpha = 0.0625f;
        }

        protected virtual bool ShouldPresentTooltip(T[] contents) { return _isActive; }

        protected virtual void DismissTooltip()
        {
            Positioner.HandleDismissal();
            if (SetInactiveWhenDismissed) Container.gameObject.SetActive(false);

            _tooltipPersistenceRoutine?.Stop();
            _currentTarget = null;
            _isLocked = false;
        }

        protected virtual void OnTooltipFullyDisclosed()
        {
            if (CanvasGroup) CanvasGroup.alpha = 1f;
        }

        private void TargetHoveredEventHandler(object sender, TooltipEventArgs e)
        {
            T[] contents = e.TooltipContent.Where(c => c is T).Cast<T>().ToArray();
            if (contents.Length == 0) return;
            if (ShouldPresentTooltip(contents) == false) return;

            if (_isLocked)
            {
                _eventDuringLock = e;
                return;
            }

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

                if ((time >= _hoverLockTime) && !LocksOnExtendedHover) continue;

                time += Time.deltaTime;
                if (CanvasGroup)
                    CanvasGroup.alpha = Mathf.Lerp(
                        a: 0.0625f,
                        b: 0.1875f,
                        t: (float) (Math.Truncate(time * 2 / _hoverLockTime) / 2f)
                    );
                if (LocksOnExtendedHover)
                    foreach (Image image in LockingImages)
                        image.fillAmount = time / _hoverLockTime;

                if (time < _hoverLockTime) continue;

                OnTooltipFullyDisclosed();
                _isLocked = LocksOnExtendedHover;
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
                foreach (Image image in LockingImages) image.fillAmount = 1 - time / _unhoverUnlockTime;
            }

            foreach (Image image in LockingImages) image.fillAmount = 0;
            DismissTooltip();

            if (_eventDuringLock != null) TargetHoveredEventHandler(sender: this, e: _eventDuringLock);
            _eventDuringLock = null;
        }

        private static bool IsTargetHovered(ITooltipTargetProvider target)
        {
            if (target.IsHovered) return true;
            return target.IgnoreRaycastBlocking && target.GetSize().IsScreenPointWithin(Input.mousePosition);
        }
    }
}
