using System.Collections.Generic;
using System.Linq;
using FinancialEntries.Models;
using FinancialEntries.Services.FinancialEntry;

namespace FinancialEntries.Services.Statement
{
    public class ConsolidatedStatement
    {
        public IEnumerable<ConsolidatedFinancialEntry> Consolidate(Repository repository)
        {
            var financialEntries = repository.Index();

            return financialEntries.GroupBy(financialEntry => new
            {
                financialEntry.ReferenceDate.Date,
                financialEntry.PaymentMethod,
                financialEntry.Store.Type
            }).Select(group => new ConsolidatedFinancialEntry()
            {
                PaymentMethod = group.Key.PaymentMethod,
                ReferenceDate = group.Key.Date.ToShortDateString(),
                Type = group.Key.Type,
                Amount = group.Sum(financialEntry => financialEntry.Amount)
            });
        }
    }
}