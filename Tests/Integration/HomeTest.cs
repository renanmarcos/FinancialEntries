using FinancialEntries;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Tests.Integration
{
    public class HomeTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private HttpClient Client;

        public HomeTest(WebApplicationFactory<Startup> factory)
        {
            Client = factory.CreateClient();
        }

        [Fact]
        public async Task TestGetStockItemsAsync()
        {
            var request = "/";

            var response = await Client.GetAsync(request);

            response.EnsureSuccessStatusCode();
            Assert.Equal("text/html", response.Content.Headers.ContentType.ToString());
        }
    }
}
