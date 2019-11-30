using System.Collections.Generic;
using FinancialEntries.Models;
using FinancialEntries.Services.FinancialEntry;
using FinancialEntries.Services.Firestore;
using Microsoft.AspNetCore.Http;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<FinancialEntry>> Index()
        {
            return Ok(_repository.Index());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public ActionResult<FinancialEntry> Show(string id)
        {
            FinancialEntry financialEntry = _repository.Get(id);
            if (financialEntry == null) return NotFound();

            return Ok(financialEntry);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status201Created)]
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
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        public IActionResult Delete(string id)
        {
            if (_repository.Delete(id)) return NoContent();
            return NotFound();
        }
    }
}
