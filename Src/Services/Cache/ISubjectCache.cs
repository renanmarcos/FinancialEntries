using FinancialEntries.Services.Observers;

namespace FinancialEntries.Services.Cache
{
    public interface ISubjectCache
    {
        public Subject GetInstance();
    }
}
