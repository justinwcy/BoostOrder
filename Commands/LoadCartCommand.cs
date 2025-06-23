using BoostOrder.Stores;

namespace BoostOrder.Commands
{
    public class LoadCartCommand(CartStore cartStore, Guid userId)
        : AsyncCommandBase
    {
        public override async Task ExecuteAsync(object? parameter)
        {
            await cartStore.Load();
        }
    }
}
