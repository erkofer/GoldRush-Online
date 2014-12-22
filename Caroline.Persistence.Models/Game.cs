namespace Caroline.Persistence.Models
{
    public class Game : IIdentifiableEntity
    {
        public int Id { get; set; }
        public string SaveData { get; set; }

        int IIdentifiableEntity<int>.EntityId
        {
            get { return Id; }
            set { Id = value; }
        }
    }
}
