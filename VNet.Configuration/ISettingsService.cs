using System.Reflection;

namespace VNet.Configuration
{
    public class SettingsService : ISettingsService
    {
        public ISettings? LoadSettings()
        {
            var settingsAssembly = Assembly.LoadFrom("VNet.ProceduralGeneration.dll");

            var settingsType = settingsAssembly.GetType("VNet.ProceduralGeneration.Cosmological.Configuration.Settings");
            if (settingsType is null) return null;

            var settingsInstance = Activator.CreateInstance(settingsType);

            return settingsInstance as ISettings ?? throw new InvalidOperationException();
        }
    }
}