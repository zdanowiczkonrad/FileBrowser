using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Windows.Threading;
using System.IO;

namespace PrzegladarkaPlikow
{
    
    public partial class CopyFilesWindow : Window
    {
        
        public string path;
        public string from;
        public string[] to;
        public int filesCount;
        public List<string> filesList;
        public bool _waitForAnswer=false;
        public bool waitForAnswer { get; set; }
        public bool allowToCopy = true;
        private BackgroundWorker backupWorkForce;
        private delegate void UpdateDelegateGrid(bool decision);
        private delegate void UpdateDelegateReplaceText(string filename);
 

        
        public CopyFilesWindow(string dir,string source,string[] elements)
        {
            InitializeComponent();
            backupWorkForce = new BackgroundWorker();
            backupWorkForce.WorkerReportsProgress = true;
            backupWorkForce.WorkerSupportsCancellation = true;
            backupWorkForce.DoWork += workForce_DoWork;
            backupWorkForce.ProgressChanged += workForce_ProgressChanged;
            backupWorkForce.RunWorkerCompleted += workForce_RunWorkerCompleted;

            path = dir;
            from = source;
           
            to = elements;
            filesList = new List<string>();
  

            //start copying
            if (backupWorkForce.IsBusy != true)
            {
                // We actually tell to the background worker to start his work asynchronously.
                backupWorkForce.RunWorkerAsync();
                tbk_Status.Text = "Trwa kopiowanie...";
            }
        }


        public int CountFolder(string path)
        {
            int count = 0;
            try
            {
                System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
               

                //znajdz wszystkie pliki i foldery
                foreach (System.IO.DirectoryInfo d in dir.GetDirectories("*.*"))
                {
                    count += CountFolder(path+@"\"+d.Name);
                }
                foreach (System.IO.FileInfo f in dir.GetFiles("*.*"))
                {
                    filesList.Add(f.FullName);
                    count++;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Brak dostępu do wybranego katalogu lub błędny adres - plik {0}",path);
            }

            return count;

        }
     
        public int FilesCount()
        {
            int count = 0;
            foreach(string item in to)
            {
                if ((File.GetAttributes(item) & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    count += CountFolder(item);
                }
                else
                {
                    count++;
                    filesList.Add(item);
                }
               
            }
            return count;
        }
        
        
        
        private void UpdateGrid(bool show)
        {
            if (show) grid1.Visibility = Visibility.Visible;
            else grid1.Visibility = Visibility.Hidden;
        }
        private void UpdateReplaceText(string text)
        {
            lbl_ReplaceFilename.Content=text;
        }
     
        private void workForce_DoWork(object sender, DoWorkEventArgs e)
        {

            // We know that the sender of the message is the background worker
            BackgroundWorker backupWorker = sender as BackgroundWorker;
            UpdateDelegateGrid updateReplaceBox = new UpdateDelegateGrid(UpdateGrid);
            UpdateDelegateReplaceText updateReplaceText = new UpdateDelegateReplaceText(UpdateReplaceText);
            
            //Count elements
            filesCount = FilesCount();
            int i = 0;
            foreach (string elem in filesList)
            {
                lbl_ReplaceFilename.Dispatcher.BeginInvoke(DispatcherPriority.Normal, updateReplaceText, string.Format("Kopiowanie pliku {0}...", elem));
                if (backupWorker.CancellationPending == true)
                {
                    // The background worker will keep an eye for any intention of the User to cancel the job.
                    e.Cancel = true;
                    break;
                }
                else
                {
                    allowToCopy = true;

                    //przygotowanie adresow
                    string fileName = elem.Substring(elem.LastIndexOf('\\') + 1, (elem.Length - 1 - elem.LastIndexOf('\\')));
                    string localPath = elem.Replace(from, "");

                    string sourcePath = elem.Replace(fileName, "");
                    sourcePath = sourcePath.Substring(0, sourcePath.Length - 1);
                    string targetPath = path + localPath;
                    targetPath = targetPath.Replace(fileName, "");
                    targetPath = targetPath.Substring(0, targetPath.Length - 1);
                    Console.WriteLine("lokalizowanie " + fileName + " " + localPath + " " + sourcePath + " " + targetPath);

                    // Use Path class to manipulate file and directory paths. 
                    string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
                    string destFile = System.IO.Path.Combine(targetPath, fileName);

                    // To copy a folder's contents to a new location: 
                    // Create a new target folder, if necessary. 
                    if (!System.IO.Directory.Exists(targetPath))
                    {
                        System.IO.Directory.CreateDirectory(targetPath);
                    }


                    //jesli jest kolizja plikow
                    if (System.IO.File.Exists(targetPath + @"\\" + fileName))
                    {
                        allowToCopy = false;
                        waitForAnswer = true;
                        grid1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, updateReplaceBox, true);
                        lbl_ReplaceFilename.Dispatcher.BeginInvoke(DispatcherPriority.Normal, updateReplaceText, string.Format("Zastąpić plik {0}?", elem));
                        while (waitForAnswer)
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                        grid1.Dispatcher.BeginInvoke(DispatcherPriority.Normal, updateReplaceBox, false);
                    }

                    if (allowToCopy)
                    {
                        //kopiuj....
                        System.IO.File.Copy(sourceFile, destFile, true);

                        /*
                        if (System.IO.Directory.Exists(sourcePath))
                        {
                            string[] files = System.IO.Directory.GetFiles(sourcePath);

                            // Copy the files and overwrite destination files if they already exist. 
                            foreach (string s in files)
                            {
                                // Use static Path methods to extract only the file name from the path.
                                fileName = System.IO.Path.GetFileName(s);
                                destFile = System.IO.Path.Combine(targetPath, fileName);
                                System.IO.File.Copy(s, destFile, true);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Source path does not exist!");
                        }*/
                    }

                    i++;

                    backupWorker.ReportProgress((int)((double)(i*100)/(double)filesCount));
                    
                }
            }
        }

        private void workForce_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            
            pgb_WorkProgress.Value = e.ProgressPercentage;
            lbl_ProgressValue.Content = e.ProgressPercentage.ToString()+"%";
        }

        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            if (backupWorkForce.WorkerSupportsCancellation == true)
            {
                // Cancel the asynchronous operation.
                backupWorkForce.CancelAsync();
            }
        }

        private void workForce_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            btn_Cancel.Visibility = Visibility.Hidden;
            // We will check for any message from the background worker.
            // The background worker will let us know if the job was cancelled.
            if (e.Cancelled == true)
            {
                tbk_Status.Text = String.Format("Kopiowanie zatrzymano ({0}%)", pgb_WorkProgress.Value.ToString());
            }
            // Or if there where errors during the job operation.
            else if (e.Error != null)
            {
                tbk_Status.Text = "Błąd kopiowania plików: " + e.Error.Message;
            }
            // Otherwise the job was completed with nothing to worry about.
            else
            {
                tbk_Status.Text = "Pliki skopiowano.";
                this.Close();
            }
        }

        private void btn_doWork_Click(object sender, RoutedEventArgs e)
        {
            if (backupWorkForce.IsBusy != true)
            {
                // We actually tell to the background worker to start his work asynchronously.
                backupWorkForce.RunWorkerAsync();
                tbk_Status.Text = "Trwa kopiowanie...";
            }
        }

        private void btn_Reset_Click(object sender, RoutedEventArgs e)
        {
            if (backupWorkForce.IsBusy != true)
            {
                pgb_WorkProgress.Value = 0;
                tbk_Status.Text = "Zresetowano....";
            }
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            waitForAnswer = false;
            allowToCopy = true;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            waitForAnswer = false;
            allowToCopy = false;
        }
    }
}
