using FinancialEntries.Services.FinancialEntry;
using FinancialEntries.Services.Store;
using FinancialEntries.Services.ValidationAttributes;
using Google.Cloud.Firestore;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace FinancialEntries.Models
{
    [FirestoreData]
    public class FinancialEntry : Model
    {
        private float _amount;

        [FirestoreProperty]
        [Required]
        public float Amount 
        { 
            get { return _amount; }
            set 
            {
                if (value >= 0) _amount = value;
                else _amount = 0;
            } 
        }
        
        [FirestoreProperty]
        [Required]
        [ValidateValue(typeof(PaymentMethodType))]
        public string PaymentMethod { get; set; }
    
        [FirestoreProperty]
        [Required]
        public DateTime ReferenceDate { get; set; }

        [FirestoreProperty("Store")]
        [JsonIgnore]
        internal DocumentReference StoreReference { get; set; }

        [SwaggerExcludeAttribute]
        public Store Store { get; internal set; }

        [Required]
        public string StoreId { internal get; set; }
    }
}