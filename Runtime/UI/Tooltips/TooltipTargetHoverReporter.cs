using System;
using System.Collections.Generic;
using System.Linq;
using Crysc.Common;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Crysc.UI.Tooltips
{
    public class TooltipTargetHoverReporter : MonoBehaviour,
        ITooltipTargetProvider,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private TooltipHoverPublisher PublisherInput;

        private readonly List<ITooltipContentProvider> _contentProviders = new();
        private GenericSizeCalculator _sizeCalculator;

        private TooltipHoverPublisher Publisher => PublisherInput;

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

        public bool IsHovered { get; private set; }

        public event EventHandler Hovered;
        public event EventHandler Unhovered;

        public object[] GetTooltipContent() { return _contentProviders.SelectMany(p => p.GetContent()).ToArray(); }

        public Bounds GetBounds() { return _sizeCalculator.Calculate().Bounds; }

        private void OnHovered(object sender, EventArgs e) { Publisher.RegisterHover(this); }
    }
}
