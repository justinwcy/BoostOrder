using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

using BoostOrder.DbContexts;
using BoostOrder.HttpClients;
using BoostOrder.Models;

using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;

namespace BoostOrder.Stores
{
    public class ProductStore
    {
        private List<Product> _products;
        private BoostOrderHttpClient _boostOrderHttpClient;
        private Lazy<Task> _initializeLazy;
        private IConfiguration _configuration;
        private readonly BoostOrderDbContextFactory _contextFactory;
        public IEnumerable<Product> Products => _products;

        public ProductStore(
            BoostOrderHttpClient boostOrderHttpClient, 
            IConfiguration configuration, 
            BoostOrderDbContextFactory contextFactory)
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
            await using var dbContext = _contextFactory.CreateDbContext();
            if (allProducts.Any())
            {
                // save it into database
                dbContext.Products.RemoveRange(dbContext.Products);
                dbContext.Products.AddRange(allProducts);

                await dbContext.SaveChangesAsync();
            }
            else
            {
                allProducts = dbContext.Products.ToList();
            }

            _products.Clear();
            _products.AddRange(allProducts);
        }
    }
}
