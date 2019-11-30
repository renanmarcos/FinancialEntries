using FinancialEntries.Services.ValidationAttributes;
using Google.Cloud.Firestore;

namespace FinancialEntries.Models
{
    [FirestoreData]
    public abstract class Model
    {
        [FirestoreDocumentId]
        [SwaggerExcludeAttribute]
        public string Id { get; set; }
    }
}