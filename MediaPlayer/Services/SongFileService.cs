using MediaPlayer.Properties;
using MusicOrigin.Interfaces;
using MusicOrigin.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System;
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
        public void SaveLastSongAndPosition(string path, double position)
        {
            string xmlFilePath = Settings.Default.XmlPath;
            XDocument xDocument;

            if (new FileInfo(xmlFilePath).Exists)
            {
                xDocument = XDocument.Load(xmlFilePath);
                XElement root = xDocument.Element("Player");

                foreach (XElement xElement in root.Elements("Song").ToList())
                {
                    xElement.Element("Path").Value = path;
                    xElement.Element("Position").Value = position.ToString();
                }
            }
            else
            {
                xDocument = new XDocument(new XElement("Player",
                   new XElement("Song",
                   new XElement("Path", path),
                   new XElement("Position", position)
                   )));
            }
            xDocument.Save(xmlFilePath);
        }

        public (string, double) LoadLastSongAndPosition()
        {

            string xmlFilePath = Settings.Default.XmlPath;
            string path = "";
            double position = 0;

            if (new FileInfo(xmlFilePath).Exists)
            {
                XDocument xDocument = XDocument.Load(xmlFilePath);
                XElement root = xDocument.Element("Player");
                foreach (XElement xElement in root.Elements("Song").ToList())
                {
                    path = xElement.Element("Path").Value;
                    double.TryParse(xElement.Element("Position").Value, out position);
                }
            }
            return (path, position);

        }
    }
}