using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows.Input;

using BoostOrder.Commands;
using BoostOrder.HttpClients;
using BoostOrder.Models;
using BoostOrder.Services;
using BoostOrder.Stores;

namespace BoostOrder.ViewModels
{
    public class CartViewModel : ViewModelBase
    {
        private readonly CartStore _cartStore;

        private ObservableCollection<CartProductViewModel> _cartProductViewModels;

        public ObservableCollection<CartProductViewModel> CartProductViewModels
        {
            get => _cartProductViewModels;
            set
            {
                _cartProductViewModels = value;
                OnPropertyChanged(nameof(GrandTotal));
                OnPropertyChanged(nameof(CartProductViewModels));
                OnPropertyChanged(nameof(TotalItems));
                OnPropertyChanged(nameof(TotalBarVisible));
            }
        }

        private bool _isLoading { get; set; }
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        public ICommand LoadCartCommand { get; }
        public ICommand CheckoutCommand { get; }
        public HeaderViewModel<CatalogViewModel> HeaderViewModel { get; }

        public string GrandTotal => $"RM {CartProductViewModels
            .Sum(cartProductViewModel => cartProductViewModel.Cart.Quantity * cartProductViewModel.Cart.Product.RegularPrice)
            .ToString("N2", CultureInfo.InvariantCulture)}";

        public string TotalItems => $"Total ({CartProductViewModels.Count()})";
        public bool TotalBarVisible => CartProductViewModels.Any();

        private readonly BoostOrderHttpClient _boostOrderHttpClient;

        public CartViewModel(
            Guid userId,
            CartStore cartStore,
            NavigationService<CatalogViewModel> catalogViewNavigationService,
            BoostOrderHttpClient boostOrderHttpClient)
        {
            _cartStore = cartStore;
            _cartStore.CartsDeleted += OnCartsDeleted;
            _cartStore.CartsAdded += OnCartsAdded;
            _boostOrderHttpClient = boostOrderHttpClient;
            LoadCartCommand = new LoadCartCommand(_cartStore, userId);
            HeaderViewModel = new HeaderViewModel<CatalogViewModel>(
                "Cart", catalogViewNavigationService, userId, cartStore);
            CheckoutCommand = new CheckoutCommand();
        }

        private void OnCartsAdded(IEnumerable<Cart> cartsAdded)
        {
            foreach (var cartAdded in cartsAdded)
            {
                var cartProductViewModel = CartProductViewModels.FirstOrDefault(cart => cart.Id == cartAdded.Id);
                if (cartProductViewModel != null)
                {
                    cartProductViewModel.Cart.Quantity = cartAdded.Quantity;
                }
                else
                {
                    cartProductViewModel = new CartProductViewModel(
                        CartProductViewModels.Count() + 1,
                        cartAdded,
                        _boostOrderHttpClient,
                        _cartStore
                    );
                    CartProductViewModels.Add(cartProductViewModel);
                }
            }

            OnPropertyChanged(nameof(GrandTotal));
            OnPropertyChanged(nameof(CartProductViewModels));
            OnPropertyChanged(nameof(TotalItems));
            OnPropertyChanged(nameof(TotalBarVisible));
        }

        private void OnCartsDeleted(IEnumerable<Cart> cartsDeleted)
        {
            var deletedIds = cartsDeleted.Select(c => c.Id).ToHashSet();
            var viewModelsToRemove = CartProductViewModels
                .Where(viewModel => deletedIds.Contains(viewModel.Id))
                .ToList();

            foreach (var viewModel in viewModelsToRemove)
            {
                CartProductViewModels.Remove(viewModel);
            }

            OnPropertyChanged(nameof(GrandTotal));
            OnPropertyChanged(nameof(CartProductViewModels));
            OnPropertyChanged(nameof(TotalItems));
            OnPropertyChanged(nameof(TotalBarVisible));
        }

        public static CartViewModel LoadViewModel(
            Guid userId,
            CartStore cartStore,
            NavigationService<CatalogViewModel> catalogViewNavigationService,
            BoostOrderHttpClient boostOrderHttpClient)
        {
            var viewModel = new CartViewModel(
                userId, 
                cartStore, 
                catalogViewNavigationService, 
                boostOrderHttpClient);
            viewModel.LoadCartCommand.Execute(null);
            viewModel.UpdateCarts(cartStore.Carts.Where(cart => cart.UserId == userId));
            return viewModel;
        }

        public void UpdateCarts(IEnumerable<Cart> carts)
        {
            CartProductViewModels = new ObservableCollection<CartProductViewModel>(
                carts
                    .Select((cart, index) => 
                        new CartProductViewModel(index, cart, _boostOrderHttpClient, _cartStore)));
        }
    }
}
