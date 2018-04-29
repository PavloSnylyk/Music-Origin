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
using MusicOrigin.ViewModel;
using MusicOrigin.Services;
using System.Globalization;

namespace MusicOrigin.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MusicOriginViewModel viewModel = new MusicOriginViewModel(new DialogService(), new SongFileService());
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
        }
    }
}

