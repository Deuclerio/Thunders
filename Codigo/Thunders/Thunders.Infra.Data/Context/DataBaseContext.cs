using Thunders.Domain.Entities;
using Thunders.Infra.Data.Mappings;
using Microsoft.EntityFrameworkCore;

namespace Thunders.Infra.Data.Context
{
    public class DatabaseContext: DbContext
    {

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<Produto> Produtos { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>(new ProdutoMap().Configure);

            base.OnModelCreating(modelBuilder);            
        }
    }
}
