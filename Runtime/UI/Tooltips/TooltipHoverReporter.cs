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
        [SerializeField] private TooltipHoverPublisher PublisherInput;
        [SerializeField] private bool IgnoreRaycastBlockingInput = true;

        public bool IsHovered { get; private set; }
        public bool IgnoreRaycastBlocking => IgnoreRaycastBlockingInput;

        protected virtual TooltipHoverPublisher Publisher => PublisherInput;

        private readonly List<ITooltipContentProvider> _contentProviders = new();
        private GenericSizeCalculator _sizeCalculator;

        private void Start()
        {
            _contentProviders.AddRange(GetComponents<ITooltipContentProvider>());
            _sizeCalculator = new GenericSizeCalculator(this);

            Hovered += OnHovered;
        }

        public void OnPointerEnter(PointerEventData _)
        {
            IsHovered = true;
            Hovered?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public void OnPointerExit(PointerEventData _)
        {
            IsHovered = false;
            Unhovered?.Invoke(sender: this, e: EventArgs.Empty);
        }

        public object[] GetTooltipContent() { return _contentProviders.SelectMany(p => p.GetContent()).ToArray(); }

        public Dimensions GetSize() { return _sizeCalculator.Calculate(); }

        private void OnHovered(object sender, EventArgs e) { Publisher.RegisterHover(this); }

        public event EventHandler Hovered;
        public event EventHandler Unhovered;
    }
}
