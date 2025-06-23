using System.Windows;

using BoostOrder.Constants;
using BoostOrder.DbContexts;
using BoostOrder.HttpClients;
using BoostOrder.Services;
using BoostOrder.Stores;
using BoostOrder.ViewModels;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace BoostOrder
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost _host;
        public App()
        {
            var builder = Host.CreateDefaultBuilder()
                .ConfigureServices((hostContext, services) =>
                {
                    var connectionString = hostContext.Configuration.GetConnectionString("Default");
                    services.AddDbContextFactory<BoostOrderDbContext>(
                        options => options.UseSqlServer(connectionString),
                        ServiceLifetime.Transient);
                    services.AddScoped<BoostOrderDbContextFactory>(x =>
                        new BoostOrderDbContextFactory(connectionString));

                    services.AddTransient<CatalogViewModel>(CreateProductViewModel);
                    services.AddSingleton<Func<CatalogViewModel>>((s) => s.GetRequiredService<CatalogViewModel>);

                    services.AddTransient<CartViewModel>(CreateCartViewModel);
                    services.AddSingleton<Func<CartViewModel>>((s) => s.GetRequiredService<CartViewModel>);

                    services.AddTransient<HeaderViewModel<CartViewModel>>();
                    services.AddTransient<HeaderViewModel<CatalogViewModel>>();
                    services.AddTransient<ProductViewModel>();

                    services.AddSingleton<NavigationStore>();

                    services.AddSingleton<MainViewModel>();
                    services.AddSingleton(services => new MainWindow()
                    {
                        DataContext = services.GetRequiredService<MainViewModel>()
                    });

                    services.AddSingleton<NavigationService<CartViewModel>>();
                    services.AddSingleton<NavigationService<CatalogViewModel>>();

                    services.AddSingleton<ProductStore>();
                    services.AddSingleton<CartStore>();

                    services.AddHttpClient<BoostOrderHttpClient>();
                });

            _host = builder.Build();
        }

        private CatalogViewModel CreateProductViewModel(IServiceProvider serviceProvider)
        {
            return CatalogViewModel.LoadViewModel(
                serviceProvider.GetRequiredService<ProductStore>(),
                serviceProvider.GetRequiredService<CartStore>(),
                serviceProvider.GetRequiredService<NavigationService<CartViewModel>>(),
                serviceProvider.GetRequiredService<BoostOrderHttpClient>(),
                AppConstants.UserId,
                serviceProvider.GetRequiredService<BoostOrderDbContextFactory>()
                );
        }

        private CartViewModel CreateCartViewModel(IServiceProvider serviceProvider)
        {
            return CartViewModel.LoadViewModel(
                AppConstants.UserId,
                serviceProvider.GetRequiredService<CartStore>(),
                serviceProvider.GetRequiredService<NavigationService<CatalogViewModel>>(),
                serviceProvider.GetRequiredService<BoostOrderHttpClient>()
                );
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _host.Start();
            var dbContextFactory = _host.Services.GetRequiredService<BoostOrderDbContextFactory>();
            using (var dbContext = dbContextFactory.CreateDbContext())
            {
                dbContext.Database.Migrate();
            }

            var navigationService = _host.Services.GetRequiredService<NavigationService<CatalogViewModel>>();
            navigationService.Navigate();

            MainWindow = _host.Services.GetRequiredService<MainWindow>();
            MainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host.Dispose();
            base.OnExit(e);
        }
    }

}
