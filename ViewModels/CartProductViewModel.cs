using System.Globalization;
using System.Windows.Input;

using BoostOrder.Commands;
using BoostOrder.Constants;
using BoostOrder.HttpClients;
using BoostOrder.Models;
using BoostOrder.Stores;

namespace BoostOrder.ViewModels
{
    public class CartProductViewModel : ViewModelBase
    {
        private readonly BoostOrderHttpClient _boostOrderHttpClient;
        private readonly int _index;
        public string Index => $"{_index}.";
        public Cart Cart { get; set; }
        public Guid Id => Cart.Id;
        public ProductVariation ProductVariation => Cart.Product.Variations.First(v => v.Sku == Cart.Sku);
        public string Sku => ProductVariation.Sku;
        public string Name => Cart.Product.Name;
        public string StockQuantity => $"{ProductVariation.StockQuantity} in stock";
        public string RegularPrice => 
            $"RM {ProductVariation.RegularPrice
                .ToString("N2", CultureInfo.InvariantCulture)}";
        public string Quantity => $"{Cart.Quantity} UNIT >";
        public string TotalPrice =>
            $"RM {(Cart.Quantity * ProductVariation.RegularPrice)
                .ToString("N2", CultureInfo.InvariantCulture)}";

        private string _imageBase64String;
        public string ImageBase64String
        {
            get => _imageBase64String;
            set
            {
                _imageBase64String = value;
                OnPropertyChanged(nameof(ImageBase64String));
            }
        }

        public ICommand RemoveCartCommand { get;  }

        public CartProductViewModel(
            int index, 
            Cart cart, 
            BoostOrderHttpClient boostOrderHttpClient,
            CartStore cartStore)
        {
            _index = index;
            Cart = cart;
            _boostOrderHttpClient = boostOrderHttpClient;
            RemoveCartCommand = new RemoveCartCommand(cart, cartStore);
            _ = LoadProductImageAsync();
        }

        private async Task LoadProductImageAsync()
        {
            try
            {
                var productImage = Cart.Product.Images.FirstOrDefault();

                if (productImage != null)
                {
                    ImageBase64String
                        = await _boostOrderHttpClient.GetProductImageBase64String(productImage.Src);
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
