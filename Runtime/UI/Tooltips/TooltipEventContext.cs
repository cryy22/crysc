namespace Crysc.UI.Tooltips
{
    public readonly struct TooltipEventContext
    {
        public ITooltipTargetProvider TargetProvider { get; }
        public object Content { get; }

        public TooltipEventContext(ITooltipTargetProvider targetProvider, object content)
        {
            TargetProvider = targetProvider;
            Content = content;
        }
    }
}
