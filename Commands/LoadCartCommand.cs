using BoostOrder.Stores;
using BoostOrder.ViewModels;

namespace BoostOrder.Commands
{
    public class LoadCartCommand(CartViewModel cartViewModel, CartStore cartStore, Guid userId)
        : AsyncCommandBase
    {
        public override async Task ExecuteAsync(object? parameter)
        {
            cartViewModel.IsLoading = true;
            try
            {
                await cartStore.Load();
                cartViewModel.UpdateCarts(cartStore.Carts.Where(cart => cart.UserId == userId));
            }
            finally
            {
                cartViewModel.IsLoading = false;
            }
        }
    }
}
