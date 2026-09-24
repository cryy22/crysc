#region

using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace Crysc.UI.Tooltips
{
    public class TooltipHoverReporter : MonoBehaviour,
        ITooltipTargetProvider,
        IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private bool IgnoreRaycastBlockingInput = true;

        public bool IsHovered { get; private set; }
        public bool IgnoreRaycastBlocking => IgnoreRaycastBlockingInput;

        private static TooltipPublisher Publisher => TooltipPublisher.I;

        private ITooltipContentProvider _contentProvider;

        private void Start()
        {
            _contentProvider = GetComponent<ITooltipContentProvider>();
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

        public object GetTooltipContent()
        {
            return _contentProvider.GetContent();
        }
    }
}
