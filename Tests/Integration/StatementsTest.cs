using FinancialEntries;
using FinancialEntries.Models;
using FinancialEntries.Services;
using FinancialEntries.Services.Firestore;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using FinancialEntriesRepository = FinancialEntries.Services.FinancialEntry.Repository;
using StoresRepository = FinancialEntries.Services.Store.Repository;

namespace Tests.Integration
{
    public class StatementsTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly FinancialEntriesRepository _financialEntriesRepository;
        private readonly StoresRepository _storesRepository;

        public StatementsTest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            var db = StaticServiceProvider.Provider.GetService(typeof(IDatabase));
            _financialEntriesRepository = new FinancialEntriesRepository(db as IDatabase);
            _storesRepository = new StoresRepository(db as IDatabase);
        }

        [Fact]
        public async Task TestIndex()
        {
            var insertedStore = _storesRepository.Insert(new Store()
            {
                Name = "Testing store",
                Type = "Leisure"
            });

            var insertedFinancialEntry = _financialEntriesRepository.Insert(new FinancialEntry()
            {
                Amount = 22.3f,
                PaymentMethod = "Credit",
                ReferenceDate = DateTime.Now.ToUniversalTime(),
                StoreId = insertedStore.Id
            });

            var response = await _client.GetAsync("/statements");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ConsolidatedFinancialEntry>>(json);
            Assert.True(list.Count > 0);

            _financialEntriesRepository.Delete(insertedFinancialEntry.Id);
            _storesRepository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestConsolidationIsCorrectly()
        {
            var insertedFinancialEntries = new List<FinancialEntry>();
            var insertedStores = new List<Store>();

            var insertedStore = _storesRepository.Insert(new Store()
            {
                Name = "Testing store",
                Type = "Leisure"
            });
            insertedStores.Add(insertedStore);

            insertedFinancialEntries.Add(_financialEntriesRepository.Insert(new FinancialEntry()
            {
                Amount = 15,
                PaymentMethod = "Credit",
                ReferenceDate = DateTime.Now.ToUniversalTime(),
                StoreId = insertedStore.Id
            }));

            insertedStore = _storesRepository.Insert(new Store()
            {
                Name = "Testing store 2",
                Type = "Bills"
            });
            insertedStores.Add(insertedStore);

            insertedFinancialEntries.Add(_financialEntriesRepository.Insert(new FinancialEntry()
            {
                Amount = 10,
                PaymentMethod = "Credit",
                ReferenceDate = DateTime.Now.ToUniversalTime(),
                StoreId = insertedStore.Id
            }));

            insertedStore = _storesRepository.Insert(new Store()
            {
                Name = "Testing store 3",
                Type = "Bills"
            });
            insertedStores.Add(insertedStore);

            insertedFinancialEntries.Add(_financialEntriesRepository.Insert(new FinancialEntry()
            {
                Amount = 10,
                PaymentMethod = "Credit",
                ReferenceDate = DateTime.Now.ToUniversalTime(),
                StoreId = insertedStore.Id
            }));

            var response = await _client.GetAsync("/statements");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<ConsolidatedFinancialEntry>>(json);
            Assert.True(list.Count > 0);

            var match = list
                .FirstOrDefault(consolidated => 
                {
                    return consolidated.Amount == 15 && 
                           consolidated.PaymentMethod == "Credit" &&
                           consolidated.Type == "Leisure";
                });
            Assert.NotNull(match);

            match = list
                .FirstOrDefault(consolidated =>
                {
                    return consolidated.Amount == 20 &&
                           consolidated.PaymentMethod == "Credit" &&
                           consolidated.Type == "Bills";
                });
            Assert.NotNull(match);

            foreach (Store store in insertedStores)
            {
                _storesRepository.Delete(store.Id);
            }

            foreach (FinancialEntry entry in insertedFinancialEntries)
            {
                _financialEntriesRepository.Delete(entry.Id);
            }
        }
    }
}
