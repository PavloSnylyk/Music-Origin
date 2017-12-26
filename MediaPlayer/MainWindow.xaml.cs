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
using System.Windows.Forms;
using System.IO;

namespace MediaPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void addMusicButton_Click(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                listBox.Items.Clear();

                DirectoryInfo di = new DirectoryInfo(fbd.SelectedPath);

                foreach (var item in di.GetFiles())
                {
                    if (item.Extension == ".mp3")
                    {
                        listBox.Items.Add(item);
                    }
                }
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaEl.Source == null)
            {
                FileInfo fi = listBox.SelectedItem as FileInfo;
                mediaEl.Source = new Uri(fi.FullName, UriKind.Relative);
                mediaEl.Play();
            }

            else
                mediaEl.Play();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaEl.Pause();
        }

        private void nextSongButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void previousSongButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void randomButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            FileInfo fi = listBox.SelectedItem as FileInfo;

            if (listBox.SelectedIndex != -1)
            {
                mediaEl.Source = new Uri(fi.FullName, UriKind.Relative);
                mediaEl.Play();
                listBox.SelectedIndex = -1;
            }
        }
    }
}
