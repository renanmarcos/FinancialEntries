namespace FinancialEntries.Services.Store
{
    public class StoreType
    {
        public string Value { get; }

        private StoreType(string value) => Value = value;

        public static StoreType Food() => new StoreType("Food");

        public static StoreType Leisure() => new StoreType("Leisure");

        public static StoreType Bills() => new StoreType("Bills");

        public static StoreType Clothing() => new StoreType("Clothing");

        public static StoreType Others() => new StoreType("Others");
    }
}
