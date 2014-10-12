using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PrzegladarkaPlikow
{
    public class ExplorerFile
    {
        public string fileName {get;set;}
        public string fileExtension {get;set;}
        public ExplorerFileType fileType {get;set;}
        public string fileDescription
        {
            get
            {
                if(fileExtension.Length>0) return fileType.description + " " + fileExtension.Substring(1, fileExtension.Length-1).ToUpper();
                return fileType.description;
            }
        }

        public ExplorerFile() { }
        public ExplorerFile(string _fileName, string _fileExtension, ExplorerFileType _fileType)
        {
            fileName = _fileName;
            fileExtension = _fileExtension;
            fileType = _fileType;

        }
        public override string ToString()
        {
            return fileName+fileExtension;
 
        }

    }
}
