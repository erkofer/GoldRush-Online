namespace GoldRush.APIs
{
    public class KomodoSessionFactory
    {
        public IKomodoSession Create()
        {
            return new KomodoSession();
        }
    }
}
