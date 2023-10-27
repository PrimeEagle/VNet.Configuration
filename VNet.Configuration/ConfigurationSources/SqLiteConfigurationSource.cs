using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VNet.Configuration.ConfigurationProviders;

// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable MemberCanBePrivate.Global
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.


namespace VNet.Configuration.ConfigurationSources
{
    public class SqLiteConfigurationSource : IConfigurationSource
    {
        private readonly ILogger _logger;


        public string ConnectionString { get; set; }
        public string LoadQuery { get; set; }
        public string SaveCommand { get; set; }


        public SqLiteConfigurationSource(ILogger logger)
        {
            _logger = logger;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new SqLiteConfigurationProvider(this, _logger);
        }
    }
}