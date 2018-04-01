using MusicOrigin.Interfaces;
using MusicOrigin.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicOrigin.Services
{
    public class SongFileService : IFileService
    {
        // Return list of songs in listbox
        public IList<SongModel> Open(string path)
        {
            IList<SongModel> songs = new List<SongModel>();

            DirectoryInfo directoryInfo = new DirectoryInfo(path);

            var files = directoryInfo.GetFiles().Where(s => s.Extension == ".mp3");

            foreach (var file in files)
            {
                songs.Add(new SongModel
                {
                    Title = file.Name,
                    Path = file.FullName
                });
            }
            return songs;
        }
    }
}
