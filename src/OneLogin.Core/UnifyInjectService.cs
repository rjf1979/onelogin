using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace OneLogin.Core
{
    public static class UnifyInjectService
    {
        public static IServiceCollection AddScopedService(this IServiceCollection services, params string[] assemblyNames)
        {
            foreach (var assemblyName in assemblyNames)
            {
                services.AddScopedService(typeof(IBaseInjectService), Assembly.Load(assemblyName));
            }
            return services;
        }
    }
}
