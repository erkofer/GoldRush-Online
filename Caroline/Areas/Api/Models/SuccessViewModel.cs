using System.Collections.Generic;

namespace Caroline.Areas.Api.Models
{
    public class SuccessViewModel
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; }
    }
}