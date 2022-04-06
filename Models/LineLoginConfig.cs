using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineNotifyPractice.Models
{
    public class LineLoginConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string AuthUrl { get; set; }
        public string ProfileUrl { get; set; }
        public string TokenUrl { get; set; }
    }
}