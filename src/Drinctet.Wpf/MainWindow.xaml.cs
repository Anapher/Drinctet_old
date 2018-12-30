using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Drinctet.ViewModels;
using Drinctet.Wpf.ViewModels;

namespace Drinctet.Wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            KeyDown += OnKeyDown;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = (MainViewModel) DataContext;
            if (!viewModel.IsGameStarted)
                return;
            if (viewModel.IsSettingsOpen)
                return;

            var gameViewModel = (GameViewModel) viewModel.ViewModel;

            if (e.Key == Key.Back)
                gameViewModel.GoBackCommand.Execute(null);
            else gameViewModel.NextSlideCommand.Execute(null);
        }
    }
}
