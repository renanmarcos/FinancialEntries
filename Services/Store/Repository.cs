using System;
using System.Collections.Generic;
using System.Linq;
using FinancialEntries.Services.Firestore;
using Google.Cloud.Firestore;
using StoreModel = FinancialEntries.Models.Store;

namespace FinancialEntries.Services.Store
{
    public class Repository : IRepository<StoreModel>
    {
        private IDatabase _database;

        private const string Collection = "Stores";

        public Repository(IDatabase database) 
        {
            _database = database;
        }

        public RecuperableCause GetCannotDeleteReason(string id) 
        {
            var database = _database.GetInstance();
            var financialEntriesReference = database.Collection("FinancialEntries");
            var storeReference = database.Collection(Collection).Document(id);

            var query = financialEntriesReference
                .WhereEqualTo("Store", storeReference)
                .GetSnapshotAsync();
            query.Wait();

            if (query.Result.Count > 0) 
                return new RecuperableCause(
                    "Cannot delete this store because it's associated with " + 
                        "one or more FinancialEntries."
                );

            return null;
        }

        public bool Delete(string id)
        {
            var task = _database.GetInstance()
                .Collection(Collection)
                .Document(id)
                .DeleteAsync(Precondition.MustExist);

            try
            {
                task.Wait();
            } catch (Exception)
            {
                return false;
            }

            return true;
        }

        public StoreModel Update(StoreModel model)
        {
            _database.GetInstance()
                .Collection(Collection)
                .Document(model.Id)
                .SetAsync(model, SetOptions.Overwrite);

            return model;
        }

        public StoreModel Get(string id)
        {
            var snapshot = _database.GetInstance()
                .Collection(Collection)
                .Document(id)
                .GetSnapshotAsync();
            snapshot.Wait();

            if (!snapshot.Result.Exists)
            {
                return null;
            }

            return snapshot.Result.ConvertTo<StoreModel>();
        }

        public IEnumerable<StoreModel> Index()
        {
            var snapshot = _database.GetInstance()
                .Collection(Collection)
                .GetSnapshotAsync();
            snapshot.Wait();

            return snapshot.Result.Select(document => 
            {
                return document.ConvertTo<StoreModel>();
            }).ToList();
        }

        public StoreModel Insert(StoreModel model)
        {
            var document = _database.GetInstance()
                .Collection(Collection)
                .AddAsync(model);
            document.Wait();
            model.Id = document.Result.Id;

            return model;
        }
    }
}