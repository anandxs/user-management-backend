namespace week_19.Api.Models.Settings
{
    public class UserDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string UsersCollection { get; set; } = null!;
    }
}
