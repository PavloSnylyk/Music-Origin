using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using MediaPlayer.Model;
using MediaPlayer.Interfaces;


namespace MediaPlayer.ViewModel
{
    public class MusicOriginViewModel : INotifyPropertyChanged
    {
        IFileService fileService;
        IDialogService dialogService;
        System.Windows.Media.MediaPlayer player;
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
        // Play music
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

                        player = new System.Windows.Media.MediaPlayer();
                        player.Open(new Uri(SelectedSong.Path));
                        player.Play();
                    }
                    catch (Exception ex)
                    {
                        dialogService.ShowMessage(ex.Message);
                    }
                }));
            }
        }

        // Return current music
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
        public MusicOriginViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;
            Songs = new ObservableCollection<SongModel> { };
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
