using MongoDB.Bson;

namespace Caroline.Domain.Helpers
{
    public static class ObjectIdHelpers
    {
        static readonly log4net.ILog Log = log4net.LogManager.GetLogger
            (System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static bool ParseAndLog(string s, out ObjectId id)
        {
            if (ObjectId.TryParse(s, out id)) return true;

            Log.Error("Failed to parse string ObjectId: " + s);
            return false;
        }
    }
}
