using Google.Cloud.Firestore;

namespace FinancialEntries.Models
{
    [FirestoreData]
    public abstract class Model
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
    }
}