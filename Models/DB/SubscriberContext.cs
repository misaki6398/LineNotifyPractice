using Microsoft.EntityFrameworkCore;

namespace LineNotifyPractice.Models.DB
{
       public class SubscriberContext : DbContext
    {
        public SubscriberContext(DbContextOptions options): base(options) {}

        public DbSet<Subscriber> Subscribers { get; set; }
    }
}