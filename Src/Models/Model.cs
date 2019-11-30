using Google.Cloud.Firestore;

namespace FinancialEntries.Models
{
    [FirestoreData]
    public abstract class Model
    {
        [FirestoreDocumentId]
        // LEMBRAR:  id fica aparecendo nas docs agr
        public string Id { get; set; }
    }
}