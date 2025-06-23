using System.Windows.Input;

using BoostOrder.Commands;
using BoostOrder.Constants;
using BoostOrder.DbContexts;
using BoostOrder.HttpClients;
using BoostOrder.Models;
using BoostOrder.Services;
using BoostOrder.Stores;

namespace BoostOrder.ViewModels
{
    public class CatalogViewModel : ViewModelBase
    {
        private readonly BoostOrderHttpClient _boostOrderHttpClient;
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

        public IEnumerable<Product> Products { get; set; }

        public ICommand LoadProductsCommand { get; }

        public ICommand LoadCartCommand { get; }

        public ICommand CartPageCommand { get; }

        private Guid _userId { get; set; }

        private BoostOrderDbContextFactory _boostOrderDbContextFactory { get; set; }

        public HeaderViewModel<CartViewModel> HeaderViewModel { get; }
        public NumberBadgeViewModel NumberBadgeVM { get; }

        private IEnumerable<ProductViewModel> _productViewModels;

        public IEnumerable<ProductViewModel> ProductViewModels
        {
            get => _productViewModels;
            set
            {
                _productViewModels = value;
                OnPropertyChanged();
            }
        }

        private string _searchString;
        private readonly CartStore _cartStore;

        public string SearchString
        {
            get => _searchString;
            set
            {
                _searchString = value;
                FilterProductViewModel(_searchString);
                OnPropertyChanged();
                OnPropertyChanged(nameof(ProductViewModels));
            }
        }

        private int _cartCount;
        public int CartCount
        {
            get => _cartCount;
            set
            {
                _cartCount = value;
                NumberBadgeVM.Number = CartCount;
                OnPropertyChanged();
            }
        }

        public CatalogViewModel(
            ProductStore productStore,
            CartStore cartStore,
            NavigationService<CartViewModel> cartViewNavigationService,
            BoostOrderHttpClient boostOrderHttpClient,
            Guid userId,
            BoostOrderDbContextFactory boostOrderDbContextFactory)
        {
            _userId = userId;
            _boostOrderDbContextFactory = boostOrderDbContextFactory;
            _boostOrderHttpClient = boostOrderHttpClient;

            _cartStore = cartStore;
            _cartStore.CartsAdded += UpdateAddCartCount;
            _cartStore.CartsDeleted += UpdateRemoveCartCount;

            LoadCartCommand = new LoadCartCommand(cartStore, userId);
            LoadProductsCommand = new LoadProductsCommand(this, productStore);
            CartPageCommand = new NavigateCommand<CartViewModel>(cartViewNavigationService);

            HeaderViewModel = new HeaderViewModel<CartViewModel>(
                "Category Name", cartViewNavigationService, userId, cartStore);
            NumberBadgeVM = new NumberBadgeViewModel(0);
        }

        public static CatalogViewModel LoadViewModel(
            ProductStore productStore, 
            CartStore cartStore, 
            NavigationService<CartViewModel> cartViewNavigationService,
            BoostOrderHttpClient boostOrderHttpClient,
            Guid userId,
            BoostOrderDbContextFactory boostOrderDbContextFactory)
        {
            var viewModel = new CatalogViewModel(
                productStore,
                cartStore,
                cartViewNavigationService,
                boostOrderHttpClient,
                userId, 
                boostOrderDbContextFactory);
            viewModel.LoadProductsCommand.Execute(null);
            viewModel.LoadCartCommand.Execute(null);
            
            viewModel.CartCount = cartStore.Carts.Count(cart => cart.UserId == userId);
            return viewModel;
        }

        public void UpdateAddCartCount(IEnumerable<Cart> addedCarts)
        {
            CartCount = _cartStore.Carts.Count(cart => cart.UserId == _userId);
        }

        public void UpdateRemoveCartCount(IEnumerable<Cart> removedCarts)
        {
            CartCount = _cartStore.Carts.Count(cart => cart.UserId == _userId);
        }

        public void SetProducts(IEnumerable<Product> products)
        {
            Products = products;
            FilterProductViewModel("");
        }

        private void FilterProductViewModel(string filterString)
        {
            ProductViewModels = Products
                .Where(product => product.Name.Contains(filterString) ||
                                  product.Sku.Contains(filterString) ||
                                  string.IsNullOrEmpty(filterString))
                .Select(product => new ProductViewModel(
                    product,
                    _boostOrderHttpClient,
                    _userId,
                    _boostOrderDbContextFactory,
                    _cartStore
                ));
        }
    }
}
