using FinancialEntries.Services.Firestore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialEntries.Extensions
{
    public static class DependencyInjection
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IDatabase, Database>();
        }
    }
}
