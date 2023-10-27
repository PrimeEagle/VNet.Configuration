namespace VNet.Configuration;

public interface IConfigurationService
{
    T GetConfiguration<T>() where T : class, new();
}