using FinancialEntries.Services.Cache;
using FinancialEntries.Services.Firestore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialEntries.Extensions
{
    public static class DependencyInjection
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddSingleton<IDatabase, Database>();
            services.AddSingleton<ISubjectCache, SubjectCache>();
        }
    }
}
