using System;
using System.IO;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Grpc.Core;
using Microsoft.Extensions.Configuration;

namespace FinancialEntries.Services.Firestore 
{
    public class Database : IDatabase 
    {
        private FirestoreDb database;
        private IConfiguration _configuration;

        public Database(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public FirestoreDb GetInstance()
        {
            if (database == null) 
            {
                GoogleCredential cred = GoogleCredential
                    .FromFile(Path.Combine(AppContext.BaseDirectory + "/credentials.json"));
                Channel channel = new Channel(
                    FirestoreClient.DefaultEndpoint.Host,
                    FirestoreClient.DefaultEndpoint.Port,
                    cred.ToChannelCredentials());
                FirestoreClient client = FirestoreClient.Create(channel);
                database = FirestoreDb.Create(_configuration["ProjectId"], client);
            }

            return database;
        }
    }
}