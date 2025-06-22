using BoostOrder.Stores;

namespace BoostOrder.Commands
{
    public class ClearCartCommand(Guid userId, CartStore cartStore)
        : AsyncCommandBase
    {
        public override async Task ExecuteAsync(object? parameter)
        {
            await cartStore.ClearUserCart(userId);
        }
    }
}