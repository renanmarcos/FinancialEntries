using System.Collections.Generic;
using FinancialEntries.Models;
using FinancialEntries.Services.Store;
using FinancialEntries.Services.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace FinancialEntries.Controllers
{
    [ApiController]
    [Route("/stores")]
    [Produces("application/json")]
    public class StoresController : ControllerBase
    {
        private readonly Repository _repository;
        private IDatabase _database;

        public StoresController(IDatabase database)
        {
            _repository = new Repository(database);
            _database = database;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Store>> Index()
        {
            return Ok(_repository.Index());
        }

        [HttpGet("{id}")]
        public ActionResult<Store> Get(string id)
        {
            Store store = _repository.Get(id);
            if (store == null) return NotFound();

            return Ok(store);
        }

        [HttpPost]
        public ActionResult<Store> Store([FromBody] Store store)
        {
            var model = _repository.Insert(store);
            return Created($"/stores/{model.Id}", model);
        }

        [HttpPut("{id}")]
        [HttpPatch("{id}")]
        public ActionResult<Store> Update(string id, [FromBody] Store store)
        {
            store.Id = id;
            return Ok(_repository.Update(store));
        }

        [HttpDelete("{id}")]
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
