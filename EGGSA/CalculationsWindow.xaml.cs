using System.Windows;
using System.Windows.Input;

namespace EGGSA
{
    public partial class CalculationsWindow : Window
    {
        public CalculationsWindow()
        {
            InitializeComponent();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }
    }
}
