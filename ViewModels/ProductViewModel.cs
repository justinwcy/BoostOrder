using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

using BoostOrder.Commands;
using BoostOrder.Constants;
using BoostOrder.DbContexts;
using BoostOrder.HttpClients;
using BoostOrder.Models;
using BoostOrder.Stores;

namespace BoostOrder.ViewModels
{
    public class ProductViewModel: ViewModelBase
    {
        private readonly BoostOrderHttpClient _boostOrderHttpClient;
        public Product Product { get; set; }
        public string Sku => Product.Sku;
        public string Name => Product.Name;
        public string StockQuantity => $"{Product.StockQuantity} in stock";
        public string RegularPrice => 
            $"RM {SelectedVariation.RegularPrice.ToString("N2", CultureInfo.InvariantCulture)}";

        private string _imageBase64String;
        public string ImageBase64String
        {
            get => _imageBase64String;
            set
            {
                _imageBase64String = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ProductVariation> AvailableVariations 
            => new(Product.Variations);

        private ProductVariation _selectedVariation;
        public ProductVariation SelectedVariation
        {
            get => _selectedVariation;
            set
            {
                _selectedVariation = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(RegularPrice));
            }
        }

        public ICommand IncrementQuantityCommand { get; }
        public ICommand DecrementQuantityCommand { get; }
        public ICommand AddProductToCartCommand { get; }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public ProductViewModel(
            Product product, 
            BoostOrderHttpClient boostOrderHttpClient,
            Guid userId,
            BoostOrderDbContextFactory boostOrderDbContextFactory,
            CartStore cartStore
            )
        {
            _boostOrderHttpClient = boostOrderHttpClient;
            Product = product;
            IncrementQuantityCommand = new IncrementQuantityCommand(this);
            DecrementQuantityCommand = new DecrementQuantityCommand(this);
            AddProductToCartCommand = new AddProductToCartCommand(
                userId,
                this, 
                boostOrderDbContextFactory,
                cartStore);
            _ = LoadProductImageAsync();
            SelectedVariation = Product.Variations.FirstOrDefault();
        }

        private async Task LoadProductImageAsync()
        {
            try
            {
                var imageUrl = Product.Images.FirstOrDefault();

                if (imageUrl != null)
                {
                    ImageBase64String
                        = await _boostOrderHttpClient.GetProductImageBase64String(imageUrl.Src);
                }
                else
                {
                    ImageBase64String = AppConstants.ImageNotFoundBase64String;
                }
            }
            catch (Exception exception)
            {
                ImageBase64String = AppConstants.ErrorLoadingImageBase64String;
            }
        }
    }
}
