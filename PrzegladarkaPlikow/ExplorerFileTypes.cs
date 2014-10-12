using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrzegladarkaPlikow
{
    public class ExplorerFileTypes
    {
        public List<ExplorerFileType> fileTypes;

        public ExplorerFileTypes()
        {
            fileTypes = new List<ExplorerFileType>();
            List<string> extensions = new List<string>();

            extensions.Clear();
            extensions.Add(".doc");
            extensions.Add(".docx");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Dokument tekstowy Word", "word.png"));

            extensions.Clear();
            extensions.Add(".jpg");
            extensions.Add(".gif");
            extensions.Add(".jpeg");
            extensions.Add(".png");
            extensions.Add(".bmp");
            extensions.Add(".tiff");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Plik obrazu", "image.png"));

            extensions.Clear();
            extensions.Add(".zip");
            extensions.Add(".rar");
            extensions.Add(".tar.gz");
            extensions.Add(".gz");
            extensions.Add(".tar");
            extensions.Add(".sfx");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Archiwum", "compressed.png"));

            extensions.Clear();
            extensions.Add(".pdf");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Dokument", "pdf.png"));

            extensions.Clear();
            extensions.Add(".exe");
            extensions.Add(".bat");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Plik wykonywalny", "exe.png"));

            extensions.Clear();
            extensions.Add(".dll");
            extensions.Add(".sys");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Plik systemowy", "developer.png"));

            extensions.Clear();
            extensions.Add(".mp3");
            extensions.Add(".wav");
            extensions.Add(".wma");
            extensions.Add(".midi");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Plik dźwiękowy", "music.png"));

            extensions.Clear();
            extensions.Add(".txt");
            extensions.Add(".ini");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Plik tekstowy", "text.png"));

            extensions.Clear();
            extensions.Add("");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.Directory, "Katalog plików", "folder.png"));

            extensions.Clear();
            extensions.Add("");
            fileTypes.Add(new ExplorerFileType(extensions, FileType.File, "Plik", "file.png"));            
        }

        public ExplorerFileType GetType(FileType fileType)
        {
            var foundObjects =
            from item in fileTypes
            where item.extensions.Contains("") && item.fileType == fileType
            select item;
            ExplorerFileType foundElement = new ExplorerFileType();
            foreach (var result in foundObjects)
            {
                foundElement = result;
            }
            return foundElement;
        }
        public ExplorerFileType GetType(string extension)
        {
            ExplorerFileType foundElement = GetType(FileType.File);
            var foundObjects =
            from item in fileTypes
            where item.extensions.Contains(extension) && item.fileType == FileType.File
            select item;
           
            foreach (var result in foundObjects)
            {
                foundElement = result;
            }
            return foundElement;
        }
        
    }
}
