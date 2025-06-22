using BoostOrder.Models;
using BoostOrder.Stores;
using BoostOrder.ViewModels;

namespace BoostOrder.Commands
{
    public class RemoveCartCommand(Cart cartToRemove, CartStore cartStore)
        : AsyncCommandBase
    {
        public override async Task ExecuteAsync(object? parameter)
        {
            await cartStore.RemoveCart(cartToRemove);
        }
    }
}