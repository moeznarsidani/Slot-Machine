using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Slot_Machine.Collections
{
    public class Players
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }  // The player's unique ID (MongoDB _id)

        [BsonElement("playerId")]  // Maps 'playerId' from MongoDB to the PlayerId property
        public string PlayerId { get; set; }  // The unique player ID (player1, etc.)

        [BsonElement("name")]  // Maps 'name' from MongoDB to the Name property
        public string Name { get; set; }  // The player's name (e.g., Test Player)

        [BsonElement("balance")]  // Maps 'balance' from MongoDB to the Balance property
        public decimal Balance { get; set; }  // The player's current balance (e.g., 1000)

        [BsonElement("createdAt")]  // Maps 'createdAt' from MongoDB to the CreatedAt property
        public DateTime CreatedAt { get; set; }  // The creation date of the player (e.g., 2025-01-18T03:47:05.484Z)
    }

    public class UpdateBalanceRequest
    {
        public string PlayerId { get; set; }
        public decimal Amount { get; set; }
    }
}
