using BoostOrder.ViewModels;

namespace BoostOrder.Commands
{
    public class IncrementQuantityCommand(ProductViewModel productViewModel) : CommandBase
    {
        public override void Execute(object? parameter)
        {
            productViewModel.Quantity += 1;
        }
    }
}
