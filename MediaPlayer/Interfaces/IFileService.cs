using MediaPlayer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayer.Interfaces
{
    public interface IFileService
    {
        IList<SongModel> Open(string filename);
    }
}
