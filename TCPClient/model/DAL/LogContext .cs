using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TCPClient.model.BL;

namespace TCPClient.model.DAL
{
    public class LogContext : DbContext
    {
        public DbSet<Log> Logs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Configure your connection string to the database
            optionsBuilder.UseSqlServer("server=(LocalDB)\\MSSQLLocalDB;Initial Catalog=Logs;Integrated Security=true");
        }
    }

}
