using FinancialEntries.Services.Observers;

namespace FinancialEntries.Services.Cache
{
    public class SubjectCache : ISubjectCache
    {
        private Subject subject;

        public Subject GetInstance()
        {
            if (subject == null)
            {
                subject = new Subject();
                subject.AddObserver(new CacheObserver(CacheKey.ConsolidatedFinancialEntries));
            }

            return subject;
        }
    }
}
