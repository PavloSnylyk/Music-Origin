using MediaPlayer.Model;
using MusicOrigin.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicOrigin.Interfaces
{
    public interface IFileService
    {
        IList<SongModel> Open(string filename);
        void SaveLastSongAndPosition(PlayerXmlSaveModel playerXmlSaveModel);
        PlayerXmlSaveModel LoadLastSongAndPosition();

    }
}
