using System;
using System.Collections.Generic;
using FinancialEntries.Models;
using FinancialEntries.Services.Cache;
using FinancialEntries.Services.FinancialEntry;
using FinancialEntries.Services.Firestore;
using FinancialEntries.Services.Statement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace FinancialEntries.Controllers
{
    [ApiController]
    [Route("/statements")]
    [Produces("application/json")]
    public class StatementController : ControllerBase
    {
        private Repository _repository;

        private IMemoryCache _cache;

        public StatementController(IDatabase database, IMemoryCache cache)
        {
            _repository = new Repository(database);
            _cache = cache;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ConsolidatedFinancialEntry>> Index()
        {
            var cached = _cache.GetOrCreate<IEnumerable<ConsolidatedFinancialEntry>>(
                CacheKey.ConsolidatedFinancialEntries,
                context => 
                {
                    context.SetAbsoluteExpiration(TimeSpan.FromDays(7));
                    context.SetPriority(CacheItemPriority.High);
                    
                    return new ConsolidatedStatement().Consolidate(_repository);
                });

            return Ok(cached);
        }
    }
}