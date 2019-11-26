using System.Collections.Generic;
using FinancialEntries.Models;
using FinancialEntries.Services.FinancialEntry;
using FinancialEntries.Services.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace FinancialEntries.Controllers
{
    [ApiController]
    [Route("/financial-entries")]
    [Produces("application/json")]
    public class FinancialEntryController : ControllerBase
    {
        private readonly Repository _repository;

        public FinancialEntryController(IDatabase database)
        {
            _repository = new Repository(database);
        }

        [HttpGet]
        public ActionResult<IEnumerable<FinancialEntry>> Index()
        {
            return Ok(_repository.Index());
        }

        [HttpGet("{id}")]
        public ActionResult<FinancialEntry> Get(string id)
        {
            FinancialEntry financialEntry = _repository.Get(id);
            if (financialEntry == null) return NotFound();

            return Ok(financialEntry);
        }

        [HttpPost]
        public ActionResult<FinancialEntry> Store(
            [FromBody] FinancialEntry financialEntry)
        {
            var cannotStoreReason = _repository.GetCannotStoreReason(financialEntry);

            if (cannotStoreReason != null) 
                return UnprocessableEntity(cannotStoreReason.GetCause());

            var model = _repository.Insert(financialEntry);
            return Created($"/stores/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [HttpPatch("{id}")]
        public ActionResult<FinancialEntry> Update(
            string id, 
            [FromBody] FinancialEntry financialEntry)
        {
            var cannotUpdateReason = _repository.GetCannotStoreReason(financialEntry);

            if (cannotUpdateReason != null) 
                return UnprocessableEntity(cannotUpdateReason.GetCause());

            financialEntry.Id = id;
            return Ok(_repository.Update(financialEntry));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            if (_repository.Delete(id)) return NoContent();
            return NotFound();
        }
    }
}
