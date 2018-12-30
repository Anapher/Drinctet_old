using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Drinctet.ViewModels;

namespace Drinctet.Wpf.Views
{
    /// <summary>
    ///     Interaction logic for GameView.xaml
    /// </summary>
    public partial class GameView : UserControl
    {
        public GameView()
        {
            InitializeComponent();
        }

        private void GameView_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            ((GameViewModel) DataContext).NextSlideCommand.Execute(null);
        }
    }
}