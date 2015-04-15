using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caroline.Persistence.Redis;

namespace Caroline.Persistence.Models
{
    public partial class SaveState : IIdentifiableEntity<long>
    {
        public long Id { get; set; }
    }
}
