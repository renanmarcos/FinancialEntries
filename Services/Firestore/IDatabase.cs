using Google.Cloud.Firestore;

namespace FinancialEntries.Services.Firestore 
{
    public interface IDatabase 
    {
        public FirestoreDb GetInstance();
    }
}