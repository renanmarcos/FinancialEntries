using System;

namespace FinancialEntries.Models
{
    public class ConsolidatedFinancialEntry
    {
        public string ReferenceDate { get; set; }
        
        public string PaymentMethod { get; set; }

        public string Type { get; set; }

        public float Amount { get; set; }
    }
}
