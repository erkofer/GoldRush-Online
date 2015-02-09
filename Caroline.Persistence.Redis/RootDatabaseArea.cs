using StackExchange.Redis;

namespace Caroline.Persistence.Redis
{
    public class RootDatabaseArea : IDatabaseArea
    {
        readonly IDatabase _db;
        readonly CarolineScriptsRepo _scripts;

        internal RootDatabaseArea(IDatabase db, CarolineScriptsRepo scripts)
        {
            _db = db;
            _scripts = scripts;
        }

        public IDatabaseArea CreateSubArea(RedisKey area)
        {
            return new DatabaseWrapper(_db, area, Scripts);
        }

        public IDatabase Area
        {
            get { return _db; }
        }

        public CarolineScriptsRepo Scripts
        {
            get { return _scripts; }
        }
    }
}
