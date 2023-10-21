namespace VNet.Configuration.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class NotASettingAttribute : Attribute
    {
        public NotASettingAttribute()
        {
        }
    }
}