using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace VNet.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IServiceProvider _serviceProvider;

        public ConfigurationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T GetConfiguration<T>() where T : class, new()
        {
            using var serviceScope = _serviceProvider.CreateScope();
            var options = serviceScope.ServiceProvider.GetRequiredService<IOptions<T>>();

            return options.Value;
        }
    }
}