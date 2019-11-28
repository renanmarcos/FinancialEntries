using System.Collections.Generic;
using System.Linq;
using FinancialEntries.Models;
using FinancialEntries.Services.FinancialEntry;
using FinancialEntries.Services.Firestore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FinancialEntries.Controllers
{
    [ApiController]
    [Route("/statement")]
    [Produces("application/json")]
    public class StatementController : ControllerBase
    {
        private IDatabase _database;

        private Repository _repository;

        public StatementController(IDatabase database)
        {
            _database = database;
            _repository = new Repository(database);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ConsolidatedFinancialEntry>> Index()
        {
            var financialEntries = _repository.Index();

            var consolidated = financialEntries.GroupBy(financialEntry => new
            {
                financialEntry.ReferenceDate.Date,
                financialEntry.PaymentMethod,
                financialEntry.Store.Type
            }).Select(group => new ConsolidatedFinancialEntry()
            {
                PaymentMethod = group.Key.PaymentMethod,
                ReferenceDate = group.Key.Date,
                Type = group.Key.Type,
                Amount = group.Sum(financialEntry => financialEntry.Amount)
            });

            return Ok(consolidated);
        }
    }
}