using Microsoft.EntityFrameworkCore;

namespace BoostOrder.DbContexts
{
    public class BoostOrderDbContextFactory(string connectionString) : IDbContextFactory<BoostOrderDbContext>
    {
        public BoostOrderDbContext CreateDbContext()
        {
            var dbContextOptionBuilder = new DbContextOptionsBuilder()
                .UseSqlServer(connectionString)
                .Options;

            return new BoostOrderDbContext(dbContextOptionBuilder);
        }
    }
}
