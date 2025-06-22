using System.Windows;

namespace BoostOrder.Commands
{
    public class CheckoutCommand : CommandBase
    {
        public CheckoutCommand()
        {
        }

        public override void Execute(object? parameter)
        {
            MessageBox.Show("The checkout page is coming soon", "Coming soon");
        }
    }
}
