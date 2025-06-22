using System.Globalization;
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

        public IEnumerable<CartProductViewModel> CartProductViewModels
        {
            get
            {
                return _cartStore.Carts
                    .Select((cart, index) => new CartProductViewModel(index, cart, _boostOrderHttpClient, _cartStore));
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

        private IEnumerable<Cart> _carts;

        public IEnumerable<Cart> Carts
        {
            get => _carts;
            set
            {
                _carts = value;
                OnPropertyChanged(nameof(Carts));
                OnPropertyChanged(GrandTotal);
                OnPropertyChanged(nameof(CartProductViewModels));
                OnPropertyChanged(nameof(TotalItems));
                OnPropertyChanged(nameof(TotalBarVisible));
            }
        }

        public string GrandTotal => $"RM {Carts
            .Sum(cart => cart.Quantity * cart.Product.RegularPrice)
            .ToString("N2", CultureInfo.InvariantCulture)}";

        public string TotalItems => $"Total ({Carts.Count()})";
        public bool TotalBarVisible => Carts.Any();

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
            LoadCartCommand = new LoadCartCommand(this, _cartStore, userId);
            HeaderViewModel = new HeaderViewModel<CatalogViewModel>(
                "Cart", catalogViewNavigationService);
            CheckoutCommand = new CheckoutCommand();
        }

        private void OnCartsAdded(IEnumerable<Cart> cartsAdded)
        {
            foreach (var cartAdded in cartsAdded)
            {
                var cart = _carts.FirstOrDefault(cart => cart.Id == cartAdded.Id);
                if (cart != null)
                {
                    cart.Quantity = cartAdded.Quantity;
                }
                else
                {
                    var cartList = _carts.ToList();
                    cartList.Add(cartAdded);
                    Carts = cartList;
                }
            }
        }

        private void OnCartsDeleted(IEnumerable<Cart> cartsDeleted)
        {
            var deletedCarts = cartsDeleted.ToList();
            var cartsNotDeleted = _carts
                .Where(cart => !deletedCarts.Contains(cart));
            Carts = cartsNotDeleted;
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

            return viewModel;
        }

        public void UpdateCarts(IEnumerable<Cart> carts)
        {
            Carts = carts;
        }
    }
}
