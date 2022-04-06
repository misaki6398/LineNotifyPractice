using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LineNotifyPractice.Models
{
    public class LineNotifyConfig
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUrl { get; set; }
        public string AuthUrl { get; set; }
        public string TokenUrl { get; set; }
        public string RevokeUrl { get; set; }
        public string NotifyUrl { get; set; }
    }
}