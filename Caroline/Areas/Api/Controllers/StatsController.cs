using System.Threading.Tasks;
using System.Web.Mvc;
using Caroline.Domain;
using Caroline.Models;
using Newtonsoft.Json;

namespace Caroline.Areas.Api.Controllers
{
    public class StatsController : Controller
    {
        public async Task<string> LeaderBoard(LeaderboardRequest model)
        {
            long start = model.Lower;
            long end = model.Upper;

            if (start < 1)
                start = 1;
            if (start > end)
                end = start;
            var count = end - start;
            if (count > 20)
                count = 20;
            end = start + count;

            var db = await UserManager.CreateAsync();
            var entries = await db.GetLeaderboardEntries(start - 1, end - 1);
            var ret = new LeaderboardEntry[entries.Length];
            for (var i = 0; i < entries.Length; i++)
                ret[i] = new LeaderboardEntry
                {
                    Rank = start + i,
                    Score = (long)entries[i].Score,
                    UserId = await db.GetUsername(entries[i].UserId) // TODO: log(count) round trips, batch db.GetUsername calls
                };
            return JsonConvert.SerializeObject(ret);
        }
    }
}