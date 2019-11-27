using System;
using System.Collections.Generic;
using System.Linq;
using FinancialEntries.Extensions;
using FinancialEntries.Models;
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

        public StatementController(IDatabase database)
        {
            _database = database;
        }

        // Temporary saving logic
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FinancialEntry>> Index([FromQuery] DateTime? date)
        {
            // Date.FromDateTime(date.Value);
            var database = _database.GetInstance();
            var financialEntriesReference = database.Collection("FinancialEntries");

        //    var q= financialEntriesReference
        //         .OrderBy("ReferenceDate")
        //         .OrderBy("PaymentMethod")
        //         .GetSnapshotAsync();

            // q.Wait();

        //    return Ok( q.Result.Select(d => d.ConvertTo<FinancialEntry>()).ToList() );

           

            var query = financialEntriesReference
                .WhereGreaterThanOrEqualTo("ReferenceDate", date.Value.ToUniversalTime().AbsoluteStart())
                .WhereLessThanOrEqualTo("ReferenceDate", date.Value.ToUniversalTime().AbsoluteEnd())
                .GetSnapshotAsync();
            query.Wait();

            return Ok(query.Result.First().ConvertTo<FinancialEntry>());
        }
    }
}