using FinancialEntries.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace Tests.Unit
{
    public class StoreTest
    {
        [Fact]
        public void TestValidateTypes()
        {
            var results = new List<ValidationResult>();

            var store = new Store() 
            {
                Name = "Test",
                Type = "Food"
            };
            var context = new ValidationContext(store);
            var isValid = Validator.TryValidateObject(store, context, results, true);
            Assert.True(isValid);

            store = new Store()
            {
                Name = "Test",
                Type = "Leisure"
            };
            context = new ValidationContext(store);
            isValid = Validator.TryValidateObject(store, context, results, true);
            Assert.True(isValid);

            store = new Store()
            {
                Name = "Test",
                Type = "Bills"
            };
            context = new ValidationContext(store);
            isValid = Validator.TryValidateObject(store, context, results, true);
            Assert.True(isValid);

            store = new Store()
            {
                Name = "Test",
                Type = "Clothing"
            };
            context = new ValidationContext(store);
            isValid = Validator.TryValidateObject(store, context, results, true);
            Assert.True(isValid);

            store = new Store()
            {
                Name = "Test",
                Type = "Others"
            };
            context = new ValidationContext(store);
            isValid = Validator.TryValidateObject(store, context, results, true);
            Assert.True(isValid);

            store = new Store()
            {
                Name = "Test",
                Type = "Invalid"
            };
            context = new ValidationContext(store);
            isValid = Validator.TryValidateObject(store, context, results, true);
            Assert.False(isValid);
        }
    }
}