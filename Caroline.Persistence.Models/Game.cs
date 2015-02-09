using System;
using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class Game : IIdentifiableEntity<byte[]>
    {
        public long Id { get; set; }

        byte[] IIdentifiableEntity<Byte[]>.Id
        {
            get { return VarintBitConverter.GetVarintBytes(Id); }
            set { Id = VarintBitConverter.ToInt64(value); }
        }
    }
}