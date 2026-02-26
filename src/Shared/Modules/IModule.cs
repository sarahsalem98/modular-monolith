using Microsoft.Extensions.DependencyInjection;

namespace Shared.Modules;

public interface IModule
{
    string Name { get; }
    void RegisterServices(IServiceCollection services);
}
