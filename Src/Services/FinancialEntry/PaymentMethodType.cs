namespace FinancialEntries.Services.FinancialEntry
{
    public class PaymentMethodType
    {
        public string Value { get; }

        private PaymentMethodType(string value) => Value = value;

        public static PaymentMethodType Credit() => new PaymentMethodType("Credit");

        public static PaymentMethodType Debit() => new PaymentMethodType("Debit");
    }
}
