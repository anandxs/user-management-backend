using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace week_19.Api.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public string Role { get; set; }

        public string ImageUrl { get; set; }
    }
}
