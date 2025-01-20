using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace Slot_Machine.Collections
{
    public class WinLine
    {
        [BsonElement("name")]  // Ensures that 'name' in the document maps to 'Name' in the class
        public string Name { get; set; }

        [BsonElement("path")]  // Ensures that 'path' in the document maps to 'Path' in the class
        public List<List<int>> Path { get; set; }
    }

    public class GameConfig
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("configId")]  // Maps 'configId' from the document to the ConfigId property
        public string ConfigId { get; set; }

        [BsonElement("matrixWidth")]  // Maps 'matrixWidth' from the document to the MatrixWidth property
        public int MatrixWidth { get; set; }

        [BsonElement("matrixHeight")]  // Maps 'matrixHeight' from the document to the MatrixHeight property
        public int MatrixHeight { get; set; }

        [BsonElement("winLines")]  // Maps 'winLines' from the document to the WinLines property
        public List<WinLine> WinLines { get; set; }

        [BsonElement("createdAt")]  // Maps 'createdAt' from the document to the CreatedAt property
        public DateTime CreatedAt { get; set; }
    }

    public class UpdateConfigurationRequest
    {
        public int MatrixWidth { get; set; }
        public int MatrixHeight { get; set; }
        //public List<WinLine> WinLines { get; set; }
    }
}
