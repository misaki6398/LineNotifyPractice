using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LineNotifyPractice.Models.DB
{
    public class Subscriber
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string? Photo { get; set; }
        public string LINEUserId { get; set; }
        public string LINELoginAccessToken { get; set; }
        public string LINELoginRefreshToken { get; set; }
        
        public string? LINENotifyAccessToken { get; set; }
    }
}