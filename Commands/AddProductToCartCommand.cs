using System.ComponentModel;
using BoostOrder.Models;
using BoostOrder.Stores;
using BoostOrder.ViewModels;

using Microsoft.EntityFrameworkCore;

namespace BoostOrder.Commands
{
    public class AddProductToCartCommand : AsyncCommandBase, IDisposable
    {
        private readonly Guid _userId;
        private readonly ProductViewModel _productViewModel;
        private readonly CartStore _cartStore;

        public AddProductToCartCommand(Guid userId,
            ProductViewModel productViewModel,
            CartStore cartStore)
        {
            _userId = userId;
            _productViewModel = productViewModel;
            _cartStore = cartStore;
            productViewModel.PropertyChanged += OnViewModelPropertyChanged;
        }

        public override async Task ExecuteAsync(object? parameter)
        {
            await _cartStore.AddProductToCart(
                _productViewModel.Product, 
                _productViewModel.Quantity, 
                _userId,
                _productViewModel.SelectedVariation.Sku);
        }

        private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ProductViewModel.Quantity))
            {
                OnCanExecuteChanged();
            }
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
