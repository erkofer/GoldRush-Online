using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Caroline.Areas.Api.Models
{
    public class AccountViewModel
    {
        public string UserName { get; set; }
        public bool Anonymous { get; set; }
        public bool SignedIn { get; set; }
    }

    public class AccountActionModel
    {
        public bool Banned { get; set; }
        public string EndsAt { get; set; }
    }
}