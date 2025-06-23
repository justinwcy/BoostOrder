using BoostOrder.Stores;
using BoostOrder.ViewModels;

namespace BoostOrder.Commands
{
    public class LoadProductsCommand(CatalogViewModel catalogViewModel, ProductStore productStore)
        : AsyncCommandBase
    {
        public override async Task ExecuteAsync(object? parameter)
        {
            catalogViewModel.IsLoading = true;
            try
            {
                // load all from API / database
                await productStore.Load();
                // requirement wants to load only products with type being the word "variable"
                catalogViewModel.SetProducts(productStore.Products
                    .Where(product=>product.Type=="variable"));
            }
            finally
            {
                catalogViewModel.IsLoading = false;
            }
        }
    }
}
