using FinancialEntries;
using FinancialEntries.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using Tests.Helpers;
using Xunit;
using System.Net;
using FinancialEntries.Services;
using FinancialEntries.Services.Store;
using FinancialEntries.Services.Firestore;
using System.Collections.Generic;
using FinancialEntryRepository = FinancialEntries.Services.FinancialEntry.Repository;
using System;

namespace Tests.Integration
{
    public class StoresTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly Repository _repository;
        private readonly FinancialEntryRepository _financialEntryRepository;

        public StoresTest(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
            var db = StaticServiceProvider.Provider.GetService(typeof(IDatabase));
            _repository = new Repository(db as IDatabase);
            _financialEntryRepository = new FinancialEntryRepository(db as IDatabase);
        }

        [Fact]
        public async Task TestIndex()
        {
            var insertedStore = _repository.Insert(new Store()
            {
                Name = "Testing",
                Type = "Food"
            });

            var response = await _client.GetAsync("/stores");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var list = JsonConvert.DeserializeObject<List<Store>>(json);
            Assert.True(list.Count > 0);

            _repository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestShow()
        {
            var insertedStore = _repository.Insert(new Store()
            {
                Name = "Testing",
                Type = "Food"
            });

            var response = await _client.GetAsync($"/stores/{insertedStore.Id}");
            
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(
                "application/json; charset=utf-8", 
                response.Content.Headers.ContentType.ToString());

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<Store>(json);
            Assert.Equal(insertedStore.Name, model.Name);
            Assert.Equal(insertedStore.Type, model.Type);

            _repository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestShowFails()
        {
            var response = await _client.GetAsync($"/stores/wrongId");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task TestStoreSuccess()
        {
            var request = new
            {
                Url = "/stores",
                Body = new 
                {
                    Name = "Testing awesome store",
                    Type = "Bills"
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
            var model = JsonConvert.DeserializeObject<Store>(json);
            Assert.Equal(request.Body.Name, model.Name);
            Assert.Equal(request.Body.Type, model.Type);

            _repository.Delete(model.Id);
        }

        [Fact]
        public async Task TestStoreFails()
        {
            var request = new
            {
                Url = "/stores",
                Body = new 
                {
                    Name = "Testing invalid store",
                    Type = "InvalidType"
                }
            };

            var response = await _client.PostAsync(
                request.Url, 
                ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task TestUpdateSuccess()
        {
            var inserted = _repository.Insert(new Store()
            {
                Name = "Testing",
                Type = "Leisure"
            });

            var request = new
            {
                Url = $"/stores/{inserted.Id}",
                Body = new 
                {
                    Name = "New name for this",
                    Type = "Bills"
                }
            };

            var response = await _client.PutAsync(
                request.Url, 
                ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var json = await response.Content.ReadAsStringAsync();
            var model = JsonConvert.DeserializeObject<Store>(json);
            Assert.Equal(request.Body.Name, model.Name);
            Assert.Equal(request.Body.Type, model.Type);

            _repository.Delete(model.Id);
        }

        [Fact]
        public async Task TestUpdateWithWrongTypeMustFail()
        {
            var inserted = _repository.Insert(new Store()
            {
                Name = "Testing",
                Type = "Leisure"
            });

            var request = new
            {
                Url = $"/stores/{inserted.Id}",
                Body = new 
                {
                    Name = "New name for this",
                    Type = "Invalid"
                }
            };

            var response = await _client.PutAsync(
                request.Url, 
                ContentHelper.GetStringContent(request.Body));

            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            _repository.Delete(inserted.Id);
        }

        [Fact]
        public async Task TestDeleteSuccess()
        {
            var inserted = _repository.Insert(new Store()
            {
                Name = "Testing",
                Type = "Leisure"
            });

            var response = await _client.DeleteAsync($"/stores/{inserted.Id}");

            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            Assert.Null(_repository.Get(inserted.Id));
        }

        [Fact]
        public async Task TestShouldNotDeleteWhenAssociatedWithFinancialEntry()
        {
            var insertedStore = _repository.Insert(new Store()
            {
                Name = "Testing",
                Type = "Leisure"
            });

            var insertedFinancialEntry = _financialEntryRepository.Insert(new FinancialEntry()
            {
                Amount = 1,
                PaymentMethod = "Credit",
                ReferenceDate = DateTime.Now.ToUniversalTime(),
                StoreId = insertedStore.Id
            });

            var response = await _client.DeleteAsync($"/stores/{insertedStore.Id}");

            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            Assert.NotNull(_repository.Get(insertedStore.Id));

            _financialEntryRepository.Delete(insertedFinancialEntry.Id);
            _repository.Delete(insertedStore.Id);
        }

        [Fact]
        public async Task TestDeleteFails()
        {
            var response = await _client.DeleteAsync("/stores/wrongId");
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
