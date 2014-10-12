using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace PrzegladarkaPlikow
{
    public class ExplorerControler
    {
        public string initialPath { get; set; }
        public string dirPath {get;set;}
        public ObservableCollection<ExplorerFile> filesExplored {get;set;}
        public ObservableCollection<ExplorerFile> filesTree {get;set;}
        private ExplorerFileTypes types {get;set;}
        public bool allowGoingUp
        {
            get
            {
                return initialPath != dirPath;
            }
            
        }
        public bool operationFailed = false;
        public string upperPath
        {
            get
            {
                string value=dirPath.Substring(0, dirPath.LastIndexOf('\\'));
                if (value.LastIndexOf('\\') < 0)
                {
                    return value + '\\';
                }
                else return (value.Substring(0, value.LastIndexOf('\\')) + '\\');
            }
        }
        public ExplorerControler() { }
        public ExplorerControler(string path)
        {
            dirPath = path;
            initialPath = path;
            filesExplored = new ObservableCollection<ExplorerFile>();
            types = new ExplorerFileTypes();
        }
        public ObservableCollection<ExplorerFile> RebuildIconsView()
        {
           return RebuildIconsView(dirPath);
        }
        public ObservableCollection<ExplorerFile> RebuildIconsView(string path)
        {
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
                ObservableCollection<ExplorerFile> temp = new ObservableCollection<ExplorerFile>();

                //znajdz wszystkie pliki i foldery
                foreach (System.IO.DirectoryInfo d in dir.GetDirectories("*.*"))
                {
                    temp.Add(new ExplorerFile(d.Name, "", types.GetType(FileType.Directory)));
                }
                foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))
                {
                    temp.Add(new ExplorerFile(f.Name.Replace(f.Extension, ""), f.Extension, types.GetType(f.Extension)));
                }
               
                dirPath = path;
                filesExplored.Clear();
                filesExplored = temp;
            }
            catch (Exception)
            {
                System.Diagnostics.Process.Start(path);
                Console.WriteLine("Brak dostępu do wybranego katalogu lub błędny adres!");
            }
            return filesExplored;
        }

    }
}
