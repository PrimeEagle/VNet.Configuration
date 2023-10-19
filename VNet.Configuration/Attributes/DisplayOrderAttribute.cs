namespace VNet.Configuration.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public class DisplayOrderAttribute : Attribute
{
    public int Order { get; }

    public DisplayOrderAttribute(int order)
    {
        Order = order;
    }
}