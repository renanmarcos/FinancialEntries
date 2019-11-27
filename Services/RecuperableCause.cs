using System.Collections.Generic;

namespace FinancialEntries.Services
{
    public class RecuperableCause
    {
        private readonly string _cause;

        public RecuperableCause(string cause)
        {
            _cause = cause;
        }

        public Dictionary<string, string> GetCause() 
        {
            return new Dictionary<string, string>() 
            {
                {
                    "error",
                    _cause
                }
            };
        }

        public override string ToString()
        {
            return $"ErrorMessage: {_cause}";
        }
    }
}