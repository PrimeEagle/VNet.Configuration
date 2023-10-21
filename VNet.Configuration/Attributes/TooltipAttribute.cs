namespace VNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class TooltipAttribute : Attribute
    {
        public string TooltipText { get; }

        public TooltipAttribute(string tooltipText)
        {
            TooltipText = tooltipText;
        }
    }
}