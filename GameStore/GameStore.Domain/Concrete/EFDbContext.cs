using GameStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameStore.Domain.Concrete
{
    class EFDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
    }
}
