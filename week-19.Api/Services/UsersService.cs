using Microsoft.Extensions.Options;
using MongoDB.Driver;
using week_19.Api.Models;
using week_19.Api.Models.Dto;
using week_19.Api.Models.Settings;

namespace week_19.Api.Services
{
    public class UsersService
    {
        private readonly IMongoCollection<User> _userCollection;

        public UsersService(
            IOptions<UserDatabaseSettings> userDatabaseSettings)
        {
            var mongoClient = new MongoClient(userDatabaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(userDatabaseSettings.Value.DatabaseName);
            _userCollection = mongoDatabase.GetCollection<User>(userDatabaseSettings.Value.UsersCollection);
        }

        public async Task<List<User>> GetAsync() =>
            await _userCollection.Find(_ => true).ToListAsync();

        public async Task<List<User>> SearchAsync(string searchString) =>
            await _userCollection.Find(x => x.Email.Contains(searchString)).ToListAsync();

        public async Task<User> ValidateUserAsync(LoginDto loginDto) =>
            await _userCollection.Find(x => x.Email == loginDto.Email && x.Password == loginDto.Password).FirstOrDefaultAsync();

        public async Task<User?> GetAsync(string email) =>
            await _userCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(User newUser) =>
            await _userCollection.InsertOneAsync(newUser);

        public async Task UpdateAsync(string id, User updatedUser) =>
            await _userCollection.ReplaceOneAsync(x => x.Id == id, updatedUser);

        public async Task<bool> DeleteAsync(string userId)
        {
            var result = await _userCollection.DeleteOneAsync(x => x.Id == userId);

            if (result.IsAcknowledged)
                return true;

            return false;
        }
    }
}
