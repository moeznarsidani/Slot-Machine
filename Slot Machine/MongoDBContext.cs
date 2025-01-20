using MongoDB.Driver;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;
using System.Numerics;

namespace Slot_Machine
{
    public class MongoDBContext
    {
        private readonly IMongoDatabase _database;

        public MongoDBContext(IConfiguration configuration)
        {
            var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            var dbname = configuration["SlotMachineDB"];
            _database = client.GetDatabase(dbname);
        }

        public IMongoCollection<GameConfig> GameConfigs => _database.GetCollection<GameConfig>("game-config");
        public IMongoCollection<Players> Players => _database.GetCollection<Players>("players");
    }
}
