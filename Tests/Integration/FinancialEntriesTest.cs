using FinancialEntries;
using FinancialEntries.Models;
using FinancialEntries.Services;
using FinancialEntriesRepository = FinancialEntries.Services.FinancialEntry.Repository;
using StoresRepository = FinancialEntries.Services.Store.Repository;
using FinancialEntries.Services.Firestore;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using Tests.Helpers;

namespace Tests.Integration
{
    public class FinancialEntriesTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly FinancialEntriesRepository _financialEntriesRepository;
        private readonly StoresRepository _storesRepository;

        public FinancialEntriesTest(WebApplicationFactory<Startup> factory)
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

            var response = await _client.GetAsync("/financial-entries");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<FinancialEntry>>(json);
            Assert.True(list.Count > 0);

            _financialEntriesRepository.Delete(insertedFinancialEntry.Id);
            _storesRepository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestShow()
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

            var response = await _client.GetAsync($"/financial-entries/{insertedFinancialEntry.Id}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<FinancialEntry>(json);
            Assert.Equal(insertedFinancialEntry.Amount, model.Amount);
            Assert.Equal(insertedFinancialEntry.PaymentMethod, model.PaymentMethod);

            _financialEntriesRepository.Delete(insertedFinancialEntry.Id);
            _storesRepository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestShowFails()
        {
            var response = await _client.GetAsync($"/financial-entries/wrongId");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestStoreSuccess()
        {
            var insertedStore = _storesRepository.Insert(new Store()
            {
                Name = "Testing store",
                Type = "Leisure"
            });

            var request = new
            {
                Url = "/financial-entries",
                Body = new
                {
                    Amount = 15.3f,
                    PaymentMethod = "Debit",
                    ReferenceDate = DateTime.Now.ToUniversalTime(),
                    StoreId = insertedStore.Id
                }
            };

            var response = await _client.PostAsync(
                request.Url,
                ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<FinancialEntry>(json);
            Assert.Equal(request.Body.Amount, model.Amount);
            Assert.Equal(request.Body.PaymentMethod, model.PaymentMethod);

            _financialEntriesRepository.Delete(model.Id);
            _storesRepository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestStoreWithNegativeAmountShouldBeZero()
        {
            var insertedStore = _storesRepository.Insert(new Store()
            {
                Name = "Testing store",
                Type = "Leisure"
            });

            var request = new
            {
                Url = "/financial-entries",
                Body = new
                {
                    Amount = -91.4f,
                    PaymentMethod = "Credit",
                    ReferenceDate = DateTime.Now.ToUniversalTime(),
                    StoreId = insertedStore.Id
                }
            };

            var response = await _client.PostAsync(
                request.Url,
                ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<FinancialEntry>(json);
            Assert.Equal(0, model.Amount);
            Assert.Equal(request.Body.PaymentMethod, model.PaymentMethod);

            _financialEntriesRepository.Delete(model.Id);
            _storesRepository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestStoreFails()
        {
            var request = new
            {
                Url = "/financial-entries",
                Body = new
                {
                    Amount = -91.4f,
                    PaymentMethod = "Credit",
                    ReferenceDate = DateTime.Now.ToUniversalTime(),
                    StoreId = "invalidStoreId"
                }
            };

            var response = await _client.PostAsync(
                request.Url,
                ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task TestUpdateSuccess()
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

            var request = new
            {
                Url = $"/financial-entries/{insertedFinancialEntry.Id}",
                Body = new
                {
                    Amount = 4,
                    PaymentMethod = "Credit",
                    ReferenceDate = DateTime.Now.ToUniversalTime(),
                    StoreId = insertedStore.Id
                }
            };

            var response = await _client.PutAsync(
                request.Url,
                ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<FinancialEntry>(json);
            Assert.Equal(request.Body.Amount, model.Amount);
            Assert.Equal(insertedFinancialEntry.PaymentMethod, model.PaymentMethod);

            _financialEntriesRepository.Delete(insertedFinancialEntry.Id);
            _storesRepository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestDeleteSuccess()
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

            var response = await _client.DeleteAsync($"/financial-entries/{insertedFinancialEntry.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Null(_storesRepository.Get(insertedFinancialEntry.Id));

            _storesRepository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestDeleteFails()
        {
            var response = await _client.DeleteAsync("/financial-entries/wrongId");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
