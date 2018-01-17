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
using System.Windows.Threading;

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


        MediaState mediastate = MediaState.Stop;
        DispatcherTimer timer = new DispatcherTimer();

        public int SelectedIndex { get; set; }
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
            // Selected first item if any item not selected
            if (listBox.SelectedIndex == -1)
            {
                listBox.SelectedIndex = 0;
            }
            // If music play and selected index equals music what play
            if (!mediaEl.IsEnabled && listBox.SelectedIndex == SelectedIndex)
            {
                mediaEl.Pause();
                mediaEl.IsEnabled = true;
            }
            else
            {
                // If listbox have at least one item and selected index not equals music what play now
                if (!listBox.Items.IsEmpty && listBox.SelectedIndex != SelectedIndex)
                {
                    PlaySelectedItem();
                    mediaEl.IsEnabled = false;
                }
                // Resume music
                else
                {
                    mediaEl.Play();
                    mediaEl.IsEnabled = false;
                }
            }
        }

        private void NextSongButton(object sender, RoutedEventArgs e)
        {

            int listBoxIndexCount = listBox.Items.Count - 1;
            int selectedIndex = listBox.SelectedIndex;

            // If choosen element and song not last
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

            // If choosen element and song not first
            if (selectedIndex != -1 && selectedIndex <= listBoxIndexCount && selectedIndex != 0)
            {
                listBox.SelectedIndex--;
                PlaySelectedItem();
            }
        }

        private void RandomButton(object sender, RoutedEventArgs e)
        {
            int listBoxIndexCount = listBox.Items.Count - 1;

            // If listBox have at least one element
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
            SelectedIndex = listBox.SelectedIndex;

            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;

            FileInfo fi = listBox.SelectedItem as FileInfo;
            mediaEl.Source = new Uri(fi.FullName, UriKind.Relative);
    //        IsPlaying = true;
            mediaEl.Play();
            mediastate = MediaState.Play;
            timer.Start();
            mediaEl.IsEnabled = false;
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (mediastate == MediaState.Play)
                sliderMusic.Value = mediaEl.Position.TotalSeconds;
        }

        private void mediaEl_MediaOpened(object sender, RoutedEventArgs e)
        {
            sliderMusic.Maximum = mediaEl.NaturalDuration.TimeSpan.TotalSeconds;
        }

        private void sliderMusic_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            mediaEl.Pause();
            mediaEl.Position = TimeSpan.FromSeconds(sliderMusic.Value);
            mediaEl.Play();
        }

        public bool IsPlaying { get { return true; } }

    }
}
