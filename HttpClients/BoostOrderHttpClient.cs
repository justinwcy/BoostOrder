using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;

using BoostOrder.DTOs;
using BoostOrder.Models;

using Microsoft.Extensions.Configuration;

namespace BoostOrder.HttpClients
{
    public class BoostOrderHttpClient
    {
        private readonly HttpClient _httpClient;

        public BoostOrderHttpClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://cloud.boostorder.com/bo-mart/api/v1/wp-json/wc/v1/bo/");

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var username = configuration["BoostOrderUsername"];
            var password = configuration["BoostOrderPassword"];
            var credentials = $"{username}:{password}";
            var encodedCredentials = Convert.ToBase64String(Encoding.ASCII.GetBytes(credentials));
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", encodedCredentials);
        }

        public async Task<string> GetProductImageBase64String(string imageUrl)
        {
            return "";
            var imageBytes = await _httpClient.GetByteArrayAsync(imageUrl);
            var base64String = Convert.ToBase64String(imageBytes);
            return base64String;
        }

        public async Task<List<Product>> GetProducts()
        {
            var requestUri = $"products?page=1";
            var allProducts = new List<Product>();
            return allProducts;
            try
            {
                var response = await _httpClient.GetAsync(requestUri);
                var totalPagesString = response.Headers.GetValues("X-WC-TotalPages").First();
                int.TryParse(totalPagesString, out var totalPages);
                var firstProductPage = await response.Content.ReadFromJsonAsync<ProductsResponseDTO>();

                allProducts.AddRange(firstProductPage.Products);
                for (var index = 2; index <= totalPages; index++)
                {
                    var pageProducts = await GetProducts(index);
                    allProducts.AddRange(pageProducts);
                }

                return allProducts;
            }
            catch (Exception e)
            {
                // if request failed, return empty list
                return allProducts;
            }
        }

        private async Task<List<Product>> GetProducts(int pageIndex)
        {
            var requestUri = $"products?page={pageIndex}";
            var response = await _httpClient.GetAsync(requestUri);

            var totalPagesString = response.Headers.GetValues("X-WC-TotalPages").First();
            int.TryParse(totalPagesString, out var totalPages);

            var firstProductPage = await response.Content.ReadFromJsonAsync<ProductsResponseDTO>();

            return firstProductPage.Products;
        }
    }
}
