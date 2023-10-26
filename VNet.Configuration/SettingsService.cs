namespace VNet.Configuration
{
    public interface ISettingsService
    {
        ISettings? LoadSettings();
    }
}