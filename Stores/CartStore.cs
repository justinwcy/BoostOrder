using BoostOrder.DbContexts;
using BoostOrder.Models;

using Microsoft.EntityFrameworkCore;

namespace BoostOrder.Stores
{
    public class CartStore
    {
        private readonly List<Cart> _carts;
        private Lazy<Task> _initializeLazy;
        private readonly IDbContextFactory<BoostOrderDbContext> _dbContextFactory;
        public IEnumerable<Cart> Carts => _carts;
        public event Action<IEnumerable<Cart>> CartsDeleted;
        public event Action<IEnumerable<Cart>> CartsAdded;

        public CartStore(IDbContextFactory<BoostOrderDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
            _carts = new List<Cart>();
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
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();

            _carts.Clear();
            _carts.AddRange(dbContext.Carts
                .Include(cart=>cart.Product)
                .ThenInclude(product => product.Images)
                .Include(cart=>cart.Product)
                .ThenInclude(product=>product.Variations)
                .AsNoTracking());
        }

        public async Task RemoveCart(Cart cart)
        {
            await using var dbContext = await _dbContextFactory.CreateDbContextAsync();
            dbContext.Carts.Remove(cart);
            await dbContext.SaveChangesAsync();

            _carts.Remove(cart);

            CartsDeleted?.Invoke([cart]);
        }

        public async Task AddProductToCart(Product product, int quantity, Guid userId, string sku)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var cartFound = await dbContext.Carts
                .Where(cart => cart.UserId == userId)
                .Where(cart => cart.ProductId == product.Id)
                .Where(cart => cart.Sku == product.Sku)
                .FirstOrDefaultAsync();

            Cart cartAdded;
            if (cartFound == null)
            {
                cartAdded = new Cart()
                {
                    UserId = userId,
                    Quantity = quantity,
                    ProductId = product.Id,
                    Sku = sku,
                };

                dbContext.Carts.Add(cartAdded);
                _carts.Add(cartAdded);
            }
            else
            {
                cartFound.Quantity += quantity;
                cartAdded = cartFound;
            }
            await dbContext.SaveChangesAsync();

            // add product back for subscribers
            cartAdded.Product = product;
            CartsAdded?.Invoke([cartAdded]);
        }

        public async Task ClearUserCart(Guid userId)
        {
            await using var dbContext = _dbContextFactory.CreateDbContext();
            var userCarts = dbContext.Carts
                .Where(cart => cart.UserId == userId)
                .ToList();

            dbContext.Carts.RemoveRange(userCarts);
            await dbContext.SaveChangesAsync();

            _carts.RemoveAll(cart => cart.UserId == userId);
            CartsDeleted?.Invoke(userCarts);
        }
    }
}
