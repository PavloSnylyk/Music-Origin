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
        void SaveLastSongAndPosition(string songPath, double position);

    }
}
