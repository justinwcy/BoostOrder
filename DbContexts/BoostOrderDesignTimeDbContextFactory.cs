using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BoostOrder.DbContexts
{
    public class BoostOrderDesignTimeDbContextFactory : IDesignTimeDbContextFactory<BoostOrderDbContext>
    {
        public BoostOrderDbContext CreateDbContext(string[] args)
        {
            var dbContextOptionBuilder = new DbContextOptionsBuilder()
                .UseSqlServer(@"Server=(localdb)\\MSSQLLocalDB;Database=BoostOrderDb;")
                .Options;

            return new BoostOrderDbContext(dbContextOptionBuilder);
        }
    }
}
