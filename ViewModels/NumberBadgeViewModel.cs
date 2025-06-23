namespace BoostOrder.ViewModels
{
    public class NumberBadgeViewModel(int number) : ViewModelBase
    {
        private int _number = number;
        public int Number
        {
            get => _number;
            set
            {
                _number = value;
                OnPropertyChanged();
            }
        }
    }
}
