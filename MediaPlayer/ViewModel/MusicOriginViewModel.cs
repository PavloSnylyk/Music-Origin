using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MusicOrigin.Model;
using MusicOrigin.Interfaces;
using System.Windows.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MusicOrigin.ViewModel
{
    public class MusicOriginViewModel : INotifyPropertyChanged
    {
        IFileService fileService;
        IDialogService dialogService;
        MediaPlayer player;
        public ObservableCollection<SongModel> Songs { get; set; }

        // Open File Dialog
        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ?? (openCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              var songs = fileService.Open(dialogService.FilePath);
                              Songs.Clear();
                              foreach (var song in songs)
                                  Songs.Add(song);
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }
        // Play song
        private RelayCommand playCommand;
        public RelayCommand PlayCommand
        {
            get
            {
                return playCommand ?? (playCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        if (player != null)
                            player.Close();

                        player.Open(new Uri(SelectedSong.Path));
                        player.Volume = VolumeValue / 100;
                        while (!player.NaturalDuration.HasTimeSpan)
                        {
                            Thread.Sleep(700);
                        }
                        Duration = player.NaturalDuration.TimeSpan.TotalSeconds; ;
                        player.Play();
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }
        // Play next song
        private RelayCommand nextSongCommand;
        public RelayCommand NextSongCommand
        {
            get
            {
                return nextSongCommand ?? (nextSongCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        for (int i = 0; i < Songs.Count; i++)
                        {
                            if (Songs[i].Equals(SelectedSong))
                            {
                                if (i == Songs.Count - 1)
                                {
                                    SelectedSong = Songs[0];
                                    break;
                                }
                                SelectedSong = Songs[i + 1];
                                break;
                            }
                        }
                        this.PlayCommand.Execute(this);
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }
        // Play previous song
        private RelayCommand previousSongCommand;
        public RelayCommand PreviousSongCommand
        {
            get
            {
                return previousSongCommand ?? (previousSongCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        for (int i = 0; i < Songs.Count; i++)
                        {
                            if (Songs[i].Equals(SelectedSong))
                            {
                                if (i == 0)
                                {
                                    SelectedSong = Songs[Songs.Count - 1];
                                    break;
                                }
                                SelectedSong = Songs[i - 1];
                                break;
                            }
                        }
                        this.PlayCommand.Execute(this);
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }
        // Return current song
        private SongModel selectedSong;
        public SongModel SelectedSong
        {
            get { return selectedSong; }
            set
            {
                selectedSong = value;
                OnPropertyChanged("SelectedSong");
            }
        }
        // Return song volume value
        private double volumeValue = 50;
        public double VolumeValue
        {
            get { return volumeValue; }
            set
            {
                volumeValue = value;
                OnPropertyChanged("VolumeValue");
                player.Volume = value / 100;
            }
        }
        // Current song position
        private double songPosition;
        public double SongPosition
        {
            get { return player.Position.TotalSeconds; }
            set
            {
                songPosition = value;
                player.Position = TimeSpan.FromSeconds(songPosition);
                OnPropertyChanged("SongPosition");
            }
        }
        // Length of song in seconds
        private double duration = 1;
        public double Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged("Duration");
            }
        }

        public MusicOriginViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
            Songs = new ObservableCollection<SongModel>();
            player = new MediaPlayer();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
