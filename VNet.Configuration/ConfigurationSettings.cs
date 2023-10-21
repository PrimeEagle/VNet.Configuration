// ReSharper disable ClassNeverInstantiated.Global
namespace VNet.Configuration
{
    public class ConfigurationSettings<T> where T : class, new()
    {
        private static readonly Lazy<T> _instance = new(() => new T(), isThreadSafe: true);

        public static T AppSettings => _instance.Value;
    }
}