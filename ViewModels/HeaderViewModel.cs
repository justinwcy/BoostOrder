using System.Windows.Input;

using BoostOrder.Commands;
using BoostOrder.Services;

namespace BoostOrder.ViewModels
{
    public class HeaderViewModel<TViewModel> : ViewModelBase where TViewModel : ViewModelBase
    {
        public ICommand PageCommand { get; }
        public string Title { get; }

        public HeaderViewModel(string title, NavigationService<TViewModel> navigationService)
        {
            Title = title;
            PageCommand = new NavigateCommand<TViewModel>(navigationService);
        }
    }
}
