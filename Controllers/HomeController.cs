using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinancialEntries.Controllers
{
    [ApiController]
    [Route("/")]
    [Produces("text/html")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(ContentResult), StatusCodes.Status200OK)]
        public ContentResult Get()
        {
            return new ContentResult
            {
                ContentType = "text/html",
                StatusCode = StatusCodes.Status200OK,
                Content = "<html><body>Awesome API! <a href='/docs'>See docs</a></body></html>"
            };
        }
    }
}
