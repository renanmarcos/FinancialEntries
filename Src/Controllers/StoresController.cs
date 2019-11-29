using System.Collections.Generic;
using FinancialEntries.Models;
using FinancialEntries.Services.Store;
using FinancialEntries.Services.Firestore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace FinancialEntries.Controllers
{
    [ApiController]
    [Route("/stores")]
    [Produces("application/json")]
    public class StoresController : ControllerBase
    {
        private readonly Repository _repository;

        public StoresController(IDatabase database)
        {
            _repository = new Repository(database);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<Store>> Index()
        {
            return Ok(_repository.Index());
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        public ActionResult<Store> Get(string id)
        {
            Store store = _repository.Get(id);
            if (store == null) return NotFound();

            return Ok(store);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<Store> Store([FromBody] Store store)
        {
            var model = _repository.Insert(store);
            return Created($"/stores/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<Store> Update(string id, [FromBody] Store store)
        {
            store.Id = id;
            return Ok(_repository.Update(store));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ActionResult), StatusCodes.Status422UnprocessableEntity)]
        public IActionResult Delete(string id)
        {
            var cannotDeleteReason = _repository.GetCannotDeleteReason(id);

            if (cannotDeleteReason != null) 
                return UnprocessableEntity(cannotDeleteReason.GetCause());

            if (_repository.Delete(id)) 
                return NoContent();

            return NotFound();
        }
    }
}
