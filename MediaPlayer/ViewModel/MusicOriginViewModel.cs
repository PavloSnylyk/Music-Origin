using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MusicOrigin.Model;
using MusicOrigin.Interfaces;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Input;

namespace MusicOrigin.ViewModel
{
    public class MusicOriginViewModel : INotifyPropertyChanged
    {
        IFileService fileService;
        IDialogService dialogService;
        System.Windows.Media.MediaPlayer player;
        private RelayCommand openCommand;
        private RelayCommand playCommand;
        private RelayCommand playResumePauseCommand;
        private RelayCommand previousSongCommand;
        private RelayCommand nextSongCommand;
        private RelayCommand listBoxDoubleClickCommand;
        private SongModel selectedSong;
        private double volumeValue = 50;
        private double songPosition = 0;
        private double duration = 1;
        private bool isPause = true;
        private System.Timers.Timer moveSliderTimer;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<SongModel> Songs { get; set; }
        private DispatcherTimer beginTimer;
        private TimeSpan beginTimerPosition;
        public MusicOriginViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
            Songs = new ObservableCollection<SongModel>();
            player = new System.Windows.Media.MediaPlayer();

            //// Temporarily,delete in last version
            //{
            //    var songs = fileService.Open("C:\\Users\\User\\Music");
            //    Songs.Clear();
            //    foreach (var song in songs)
            //        Songs.Add(song);
            //}

            beginTimer = new DispatcherTimer();
            beginTimer.Interval = TimeSpan.FromSeconds(1);
            beginTimer.Tick += BeginTimer_Tick;
        }


        //Return progress timer value in minutes:seconds
        public string BeginTimerCount
        {
            get
            {
                beginTimerPosition = TimeSpan.FromSeconds(SongPosition);
                return String.Format("{0:00}:{1:00}", beginTimerPosition.Minutes, beginTimerPosition.Seconds);
            }
        }

        private void BeginTimer_Tick(object sender, EventArgs e)
        {
            OnPropertyChanged("BeginTimerCount");
        }

        // Play Resume Pause song
        public RelayCommand PlayResumePauseCommand
        {
            get
            {
                return playResumePauseCommand ?? (playResumePauseCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        if (player != null)
                        {
                            if (player.Position.Milliseconds == 0 || SelectedSong.Path != player.Source.LocalPath)
                            {
                                this.PlayCommand.Execute(this);
                            }
                            else
                            {
                                if (IsPause == false)
                                {
                                    player.Pause();
                                    beginTimer.Stop();
                                    IsPause = true;
                                }
                                else
                                {
                                    player.Play();
                                    beginTimer.Start();
                                    IsPause = false;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }
        //  Play song
        public RelayCommand PlayCommand
        {
            get
            {
                return playCommand ?? (playCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        player.Close();

                        player.Open(new Uri(SelectedSong.Path));
                        player.Volume = VolumeValue / 100;
                        while (!player.NaturalDuration.HasTimeSpan)
                        {
                            Thread.Sleep(700);
                        }
                        Duration = player.NaturalDuration.TimeSpan.TotalSeconds;
                        SongPosition = 0;
                        player.Play();
                        beginTimer.Start();
                        SwitchOnAutoMoveSlider();
                        IsPause = false;                       
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }
        // Open File Dialog
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

        // Play next song
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

        public RelayCommand ListBoxDoubleClickCommand
        {
            get
            {
                return listBoxDoubleClickCommand = new RelayCommand((obj) => PlayCommand.Execute(this));
            }
        }

        // Return current song
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

        // Length of song in seconds
        public double Duration
        {
            get { return duration; }
            set
            {
                duration = value;
                OnPropertyChanged("Duration");
            }
        }
        // Playing song or not
        public bool IsPause
        {
            get { return isPause; }
            set
            {
                isPause = value;
                OnPropertyChanged("IsPause");
            }
        }
        // Current song position
        public double SongPosition
        {
            get
            {
                return player.Position.TotalSeconds;
            }
            set
            {
                songPosition = value;
                player.Position = TimeSpan.FromSeconds(songPosition);
                OnPropertyChanged("SongPosition");
            }
        }
        // Settings for timer and start OnTimedEvent for refreshing song position on slider
        private void SwitchOnAutoMoveSlider()
        {
            moveSliderTimer = new System.Timers.Timer();
            moveSliderTimer.Interval = 300;
            moveSliderTimer.Elapsed += OnTimedEvent;
            moveSliderTimer.AutoReset = true;
            moveSliderTimer.Enabled = true;
        }
        // Refreshing SongPosition
        private void OnTimedEvent(object source, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                SongPosition = player.Position.TotalSeconds;
            });
        }
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
    }
}
