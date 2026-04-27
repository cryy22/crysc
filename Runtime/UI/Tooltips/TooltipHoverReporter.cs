using System;
using System.Collections.Generic;
using System.Linq;
using Crysc.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.UI.Tooltips
{
    public class TooltipHoverReporter : MonoBehaviour,
        ITooltipTargetProvider,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool IgnoreRaycastBlockingInput = true;

        public bool IsHovered { get; private set; }
        public bool IgnoreRaycastBlocking => IgnoreRaycastBlockingInput;

        protected virtual TooltipPublisher Publisher => TooltipPublisher.I;

        private readonly List<ITooltipContentProvider> _contentProviders = new();

        private void Start()
        {
            _contentProviders.AddRange(GetComponents<ITooltipContentProvider>());
        }
        private void OnDisable()
        {
            OnPointerExit(null);
        }

        public void OnPointerEnter(PointerEventData _)
        {
            IsHovered = true;
            Publisher.RegisterHover(this);
        }

        public void OnPointerExit(PointerEventData _)
        {
            IsHovered = false;
            Publisher.RegisterUnhover(this);
        }

        public object[] GetTooltipContent() { return _contentProviders.SelectMany(p => p.GetContent()).ToArray(); }
    }
}
