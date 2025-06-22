using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using BoostOrder.Commands;
using BoostOrder.Models;
using BoostOrder.Stores;
using BoostOrder.ViewModels;

namespace BoostOrder.Views
{
    public partial class CartProductView : UserControl
    {
        private Point _initialMousePosition;
        private bool _isDragging = false;
        public CartProductView()
        {
            InitializeComponent();
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                _initialMousePosition = e.GetPosition((UIElement)sender);
                _isDragging = true;
                ((UIElement)sender).CaptureMouse();
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_isDragging)
            {
                var finalMousePosition = e.GetPosition((UIElement)sender);
                var deltaX = finalMousePosition.X - _initialMousePosition.X;

                var minSwipeDistance = 50;
                if (Math.Abs(deltaX) > minSwipeDistance)
                {
                    var cartProductViewModel = (CartProductViewModel)DataContext;
                    cartProductViewModel.RemoveCartCommand.Execute(null);
                }

                _isDragging = false;
                ((UIElement)sender).ReleaseMouseCapture();
            }
        }
    }
}
