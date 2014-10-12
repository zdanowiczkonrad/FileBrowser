using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PrzegladarkaPlikow
{
    public enum FileType : int
    {
        Directory = 1,
        File = 2
    }
    public class ExplorerFileType
    {
        public List<string> extensions {get;set;}
        public FileType fileType { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
        public string smallIcon
        {
            get
            {
                return "Small icons/" + icon;
            }
        }
        public string largeIcon
        {
            get
            {
                return "Large icons/" + icon;
            }
        }
        public ExplorerFileType() { }
        public ExplorerFileType(List<string> _extensions, FileType _fileType, string _description, string _icon)
        {
            extensions = new List<string>(_extensions);
            fileType = _fileType;
            description = _description;
            icon = _icon;
        }
        
        public override string ToString()
        {
            string text;
            text = description;
            return text;
        }


    }

}
