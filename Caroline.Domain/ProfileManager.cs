using System.Threading.Tasks;
using Caroline.Domain.Models;
using Caroline.Persistence;

namespace Caroline.Domain
{
    public class ProfileManager
    {
        CarolineDb _db;
        ProfileManager(CarolineDb db) { }
        public static async Task<ProfileManager> CreateAsync()
        {
            var db = await CarolineDb.CreateAsync();
            return new ProfileManager(db);
        }

        public async Task<UserProfile> GetProfile(long id)
        {
            return new UserProfile
            {
                ScoreboardRank = await _db.Redis.HighScores.Get("lb", id)
            };
        }
    }
}
