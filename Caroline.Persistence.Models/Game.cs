using System;
using System.Globalization;
using Caroline.Persistence.Redis;
using Caroline.Persistence.Redis.Extensions;

namespace Caroline.Persistence.Models
{
    public partial class Game : IIdentifiableEntity<string>
    {
        public long Id { get; set; }

        string IIdentifiableEntity<string>.Id
        {
            get { return Id.ToStringInvariant(); }
            set { Id = long.Parse(value, CultureInfo.InvariantCulture); }
        }
    }
}