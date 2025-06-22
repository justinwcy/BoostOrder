using System.Windows.Input;

using BoostOrder.Commands;
using BoostOrder.Services;
using BoostOrder.Stores;

namespace BoostOrder.ViewModels
{
    public class HeaderViewModel<TViewModel> : ViewModelBase where TViewModel : ViewModelBase
    {
        public ICommand PageCommand { get; }
        public ICommand ClearCartCommand { get; }
        public string Title { get; }

        public HeaderViewModel(
            string title, 
            NavigationService<TViewModel> navigationService,
            Guid userId,
            CartStore cartStore)
        {
            Title = title;
            PageCommand = new NavigateCommand<TViewModel>(navigationService);
            ClearCartCommand = new ClearCartCommand(userId, cartStore);
        }
    }
}
