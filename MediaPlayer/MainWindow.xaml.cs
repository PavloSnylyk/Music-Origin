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

        private void AddMusicButton(object sender, RoutedEventArgs e)
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

        private void PlayButton(object sender, RoutedEventArgs e)
        {

            if (mediaEl.Source == null)
            {
                listBox.SelectedIndex++;
            }

            if (!listBox.Items.IsEmpty)
            {
                PlaySelectedItem();
            }
        }

        private void PauseButton(object sender, RoutedEventArgs e)
        {
            mediaEl.Pause();
        }

        private void NextSongButton(object sender, RoutedEventArgs e)
        {

            int listBoxIndexCount = listBox.Items.Count - 1;
            int selectedIndex = listBox.SelectedIndex;

            // if choosen element and song not last
            if (selectedIndex != -1 && selectedIndex < listBoxIndexCount)
            {
                listBox.SelectedIndex++;
                PlaySelectedItem();
            }
        }

        private void PreviousSongButton(object sender, RoutedEventArgs e)
        {
            int listBoxIndexCount = listBox.Items.Count - 1;
            int selectedIndex = listBox.SelectedIndex;

            // if choosen element and song not first
            if (selectedIndex != -1 && selectedIndex <= listBoxIndexCount && selectedIndex != 0)
            {
                listBox.SelectedIndex--;
                PlaySelectedItem();
            }
        }

        private void RandomButton(object sender, RoutedEventArgs e)
        {
            int listBoxIndexCount = listBox.Items.Count - 1;

            // if listBox have at least one element
            if (listBoxIndexCount != -1)
            {
                listBox.SelectedIndex = new Random().Next(0, listBoxIndexCount);
                PlaySelectedItem();
            }

        }
        private void MusicListBox(object sender, MouseButtonEventArgs e)
        {

            if (listBox.SelectedIndex != -1)
            {
                PlaySelectedItem();
                listBox.SelectedIndex = -1;
            }
        }
        private void PlaySelectedItem()
        {
            FileInfo fi = listBox.SelectedItem as FileInfo;
            mediaEl.Source = new Uri(fi.FullName, UriKind.Relative);
            mediaEl.Play();
        }
    }
}
