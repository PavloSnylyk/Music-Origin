﻿using System;
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
using System.Collections;
using System.Collections.Generic;
using MediaPlayer.Model;
using System.Linq;
using System.Globalization;
using System.Windows.Controls;

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
        private RelayCommand shuffleSongsCommand;
        private SongModel selectedSong;
        private double volumeValue = 50;
        private double songPosition = 0;
        private double duration = 1;
        private bool isPause = true;
        private bool isShuffle;
        private System.Timers.Timer moveSliderTimer;
        public event PropertyChangedEventHandler PropertyChanged;
        public ObservableCollection<SongModel> Songs { get; set; }
        private DispatcherTimer beginTimer;
        private TimeSpan beginTimerPosition;
        public ObservableCollection<MenuItem> MenuItems { get; set; }
        private List<int> shuffleIndexSongList;

        public MusicOriginViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
            Songs = new ObservableCollection<SongModel>();
            MenuItems = new ObservableCollection<MenuItem>();
            player = new System.Windows.Media.MediaPlayer();

            beginTimer = new DispatcherTimer();
            beginTimer.Interval = TimeSpan.FromSeconds(1);
            beginTimer.Tick += BeginTimer_Tick;
            InitializeLanguage();
            InitializeXml(fileService);
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
                            if (SongPosition == 0 || SelectedSong.Path != player.Source.LocalPath)
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
                                    Duration = player.NaturalDuration.TimeSpan.TotalSeconds;
                                    SongPosition = player.Position.TotalSeconds;
                                    SwitchOnAutoMoveSlider();
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
                        //      SongPosition = 0;
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

        // Add list of songs in collection that fill list in view
        private void FillSongList(string path)
        {
            var songs = fileService.Open(path);
            Songs.Clear();
            foreach (var song in songs)
            {
                Songs.Add(song);
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
                              FillSongList(dialogService.FilePath);
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
                        if (IsShuffle)
                        {
                            for (int i = 0; i < Songs.Count; i++)
                            {

                                if (Songs[shuffleIndexSongList[i]].Path.Equals(SelectedSong.Path))
                                {
                                    if (i == Songs.Count - 1)
                                    {
                                        SelectedSong = Songs[shuffleIndexSongList[0]];
                                        break;
                                    }
                                    SelectedSong = Songs[shuffleIndexSongList[i + 1]];
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < Songs.Count; i++)
                            {
                                if (Songs[i].Path.Equals(SelectedSong.Path))
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
                        if (IsShuffle)
                        {
                            for (int i = 0; i < Songs.Count; i++)
                            {

                                if (Songs[shuffleIndexSongList[i]].Path.Equals(SelectedSong.Path))
                                {
                                    if (i == 0)
                                    {
                                        SelectedSong = Songs[shuffleIndexSongList[Songs.Count - 1]];
                                        break;
                                    }
                                    SelectedSong = Songs[shuffleIndexSongList[i - 1]];
                                    break;
                                }
                            }
                        }
                        else
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
        // Shuffle song or not
        public bool IsShuffle
        {
            get { return isShuffle; }
            set
            {
                isShuffle = value;
                OnPropertyChanged("IsShuffle");
            }
        }

        // Shuffle Songs
        public RelayCommand ShuffleSongsCommand
        {
            get
            {
                return shuffleSongsCommand ?? (shuffleSongsCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        IsShuffle = !IsShuffle;
                        shuffleIndexSongList.Clear();

                        for (int i = 0; i < Songs.Count; i++)
                        {
                            shuffleIndexSongList.Add(i);
                        }
                        shuffleIndexSongList = shuffleIndexSongList.OrderBy(a => Guid.NewGuid()).ToList();
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
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
        // Initialize languae
        private void InitializeLanguage()
        {
            App.LanguageChanged += LanguageChanged;
            foreach (var lang in App.Languages)
            {
                MenuItem menuLang = new MenuItem();
                menuLang.Header = lang.DisplayName;
                menuLang.Tag = lang;
                menuLang.IsChecked = lang.Equals(App.Language);
                menuLang.Click += ChangeLanguageClick;
                MenuItems.Add(menuLang);
            }
        }
        // Initialize Xml
        private void InitializeXml(IFileService fileService)
        {
            if (fileService.LoadLastSongAndPosition() != null)
            {
                var playerXmlSaveModel = fileService.LoadLastSongAndPosition();
                FillSongList(playerXmlSaveModel.FolderPath);
                foreach (var item in Songs)
                {
                    if (item.Path == playerXmlSaveModel.FolderPath + "\\" + playerXmlSaveModel.SongName)
                    {
                        SelectedSong = item;
                        break;
                    }
                }
                player.Open(new Uri(SelectedSong.Path));
                Thread.Sleep(1000);
                player.Position = TimeSpan.FromSeconds(playerXmlSaveModel.SongPosition);
                SongPosition = player.Position.TotalSeconds;
                Duration = player.NaturalDuration.TimeSpan.TotalSeconds;
                shuffleIndexSongList = new List<int>();
                SwitchOnAutoMoveSlider();
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
            get { return songPosition; }
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
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    SongPosition = player.Position.TotalSeconds;
                });
            }
            catch (NullReferenceException)
            {

            }
        }

        // Save song position before close application
        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var playerXmlSaveModel = new PlayerXmlSaveModel
            {
                FolderPath = dialogService.FilePath,
                SongName = SelectedSong == null ? "" : SelectedSong.Title,
                SongPosition = SongPosition
            };
            if (playerXmlSaveModel.FolderPath != null)
            {
                fileService.SaveLastSongAndPosition(playerXmlSaveModel);
            }
            else if (SelectedSong != null)
            {
                playerXmlSaveModel.FolderPath = fileService.LoadLastSongAndPosition().FolderPath;

                fileService.SaveLastSongAndPosition(playerXmlSaveModel);

            }
        }

        // Settings when changed language
        private void LanguageChanged(Object sender, EventArgs e)
        {
            foreach (MenuItem menuItem in MenuItems)
            {
                CultureInfo cultureInfo = menuItem.Tag as CultureInfo;
                menuItem.IsChecked = cultureInfo != null && cultureInfo.Equals(App.Language);
            }
        }

        // Choose language
        private void ChangeLanguageClick(Object sender, EventArgs e)
        {
            MenuItem menuItem = sender as MenuItem;
            if (menuItem != null)
            {
                CultureInfo lang = menuItem.Tag as CultureInfo;
                if (lang != null)
                {
                    App.Language = lang;
                }
            }
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
