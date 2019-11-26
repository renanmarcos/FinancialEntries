using FinancialEntries.Services.Store;
using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace FinancialEntries.Models
{
    [FirestoreData]
    public class Store : Model
    {
        [FirestoreProperty]
        [Required]
        public string Name { get; set; }
        
        [FirestoreProperty]
        [Required]
        [ValidateValue(typeof(StoreType))]
        public string Type { get; set; }
    }
}