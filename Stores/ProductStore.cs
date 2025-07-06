using BoostOrder.DbContexts;
using BoostOrder.HttpClients;
using BoostOrder.Models;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BoostOrder.Stores
{
    public class ProductStore
    {
        private List<Product> _products;
        private readonly BoostOrderHttpClient _boostOrderHttpClient;
        private Lazy<Task> _initializeLazy;
        private IConfiguration _configuration;
        private readonly IDbContextFactory<BoostOrderDbContext> _contextFactory;
        public IEnumerable<Product> Products => _products;

        public ProductStore(
            BoostOrderHttpClient boostOrderHttpClient, 
            IConfiguration configuration,
            IDbContextFactory<BoostOrderDbContext> contextFactory)
        {
            _boostOrderHttpClient = boostOrderHttpClient;
            _configuration = configuration;
            _contextFactory = contextFactory;
            _products = new List<Product>();
            _initializeLazy = new Lazy<Task>(Initialize);
        }

        public async Task Load()
        {
            try
            {
                await _initializeLazy.Value;

            }
            catch (Exception ex)
            {
                _initializeLazy = new Lazy<Task>(Initialize);
                throw;
            }
        }

        private async Task Initialize()
        {
            // Get products
            var allProducts = await _boostOrderHttpClient.GetProducts();
            await using var dbContext = await _contextFactory.CreateDbContextAsync();
            if (allProducts.Any())
            {
                // save it into database for offline use in case
                // clear existing data and replace
                dbContext.Products.RemoveRange(dbContext.Products);
                dbContext.ProductImages.RemoveRange(dbContext.ProductImages);
                dbContext.ProductVariations.RemoveRange(dbContext.ProductVariations);

                dbContext.Products.AddRange(allProducts);

                await dbContext.SaveChangesAsync();
            }
            else
            {
                allProducts = dbContext.Products
                    .Include(product=>product.Variations)
                    .Include(product=>product.Images)
                    .AsNoTracking()
                    .ToList();
            }

            _products.Clear();
            _products.AddRange(allProducts);
        }
    }
}
