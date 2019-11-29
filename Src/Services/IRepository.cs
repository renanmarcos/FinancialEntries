using System.Collections.Generic;
using FinancialEntries.Models;

namespace FinancialEntries.Services 
{
    public interface IRepository<T> where T : Model 
    {
        public T Get(string id);
        public IEnumerable<T> Index();
        public T Insert(T model);
        public bool Delete(string id);
        public T Update(T model);
    }
}