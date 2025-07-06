using System.ComponentModel;
using BoostOrder.ViewModels;

namespace BoostOrder.Commands
{
    public class DecrementQuantityCommand : CommandBase, IDisposable
    {
        private readonly ProductViewModel _productViewModel;
        public DecrementQuantityCommand(ProductViewModel productViewModel)
        {
            _productViewModel = productViewModel;
            productViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductViewModel.Quantity))
            {
                OnCanExecuteChanged();
            }
        }

        public override void Execute(object? parameter)
        {
            _productViewModel.Quantity -= 1;
        }

        public override bool CanExecute(object? parameter)
        {
            return _productViewModel.Quantity > 0 && base.CanExecute(parameter);
        }

        public void Dispose()
        {
            _productViewModel.PropertyChanged -= OnViewModelPropertyChanged;
        }
    }
}
