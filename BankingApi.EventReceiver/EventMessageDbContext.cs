using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankingApi.EventReceiver
{
    public class EventMessageDbContext : DbContext
    {
        public DbSet<EventMessage> EventMessages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlServer("Data Source=.\\SQLEXPRESS;Initial Catalog=BankingApiTest;Integrated Security=True;TrustServerCertificate=True;");
    }
}
