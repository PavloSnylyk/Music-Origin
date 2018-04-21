using MediaPlayer.Properties;
using MusicOrigin.Interfaces;
using MusicOrigin.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;
using MediaPlayer.Model;

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
        // Save last played song and position that 
        public void SaveLastSongAndPosition(PlayerXmlSaveModel playerXmlSaveModel)
        {

            string xmlFilePath = Settings.Default.XmlPath;
            XDocument xDocument;

            if (new FileInfo(xmlFilePath).Exists)
            {
                xDocument = XDocument.Load(xmlFilePath);
                XElement root = xDocument.Element("Player").Element("Song");

                root.Element("FolderPath").Value = playerXmlSaveModel.FolderPath;
                root.Element("SongName").Value = playerXmlSaveModel.SongName;
                root.Element("SongPosition").Value = playerXmlSaveModel.SongPosition.ToString();

            }
            else
            {
                xDocument = new XDocument(new XElement("Player",
                   new XElement("Song",
                   new XElement("FolderPath", playerXmlSaveModel.FolderPath),
                   new XElement("SongName", playerXmlSaveModel.SongName),
                   new XElement("SongPosition", playerXmlSaveModel.SongPosition)
                   )));
            }
            xDocument.Save(xmlFilePath);
        }

        public PlayerXmlSaveModel LoadLastSongAndPosition()
        {
            try
            {
                string xmlFilePath = Settings.Default.XmlPath;

                if (new FileInfo(xmlFilePath).Exists)
                {
                    XDocument xDocument = XDocument.Load(xmlFilePath);
                    XElement root = xDocument.Element("Player").Element("Song");
                    PlayerXmlSaveModel playerXmlSaveModel = new PlayerXmlSaveModel
                    {
                        SongName = root.Element("SongName").Value,
                        FolderPath = root.Element("FolderPath").Value,
                        SongPosition = double.Parse(root.Element("SongPosition").Value)
                    };
                    return playerXmlSaveModel;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}