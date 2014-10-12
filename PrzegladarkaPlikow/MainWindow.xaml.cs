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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Collections.ObjectModel;

namespace PrzegladarkaPlikow
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public ExplorerControler explorer;
        public bool mousePressedOnListbox = false;
        public Point pressed;
        public Point current;
        public bool dropToDirectory;

        public MainWindow()
        {
            InitializeComponent();
            explorer = new ExplorerControler(@"D:\Kartoteka\");

            //przypięcie danych do ListBox
            RefreshIconsView();

        }

        //double click event
        private void ListBox_MouseDown(object sender, MouseButtonEventArgs e)
        {

            
 
                foreach (var item in directoryFiles.Items)
                {
                    var itemContainer = directoryFiles.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    itemContainer.IsSelected = false;
                }
                rct_Selection.Visibility = Visibility.Visible;
                // Capture and track the mouse.
                mousePressedOnListbox = true;
                pressed = e.GetPosition(gridFiles);
                //gridFiles.CaptureMouse();

                // Initial placement of the drag selection box.         
                Canvas.SetLeft(rct_Selection, pressed.X);
                Canvas.SetTop(rct_Selection, pressed.Y);
                rct_Selection.Width = 0;
                rct_Selection.Height = 0;
           
        }
        //mouseMove
        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePressedOnListbox)
            {
                // When the mouse is held down, reposition the drag selection box.

                Point mousePos = e.GetPosition(gridFiles);

                if (pressed.X < mousePos.X)
                {
                    Canvas.SetLeft(rct_Selection, pressed.X);
                    rct_Selection.Width = mousePos.X - pressed.X;
                }
                else
                {
                    Canvas.SetLeft(rct_Selection, mousePos.X);
                    rct_Selection.Width = pressed.X - mousePos.X;
                }

                if (pressed.Y < mousePos.Y)
                {
                    Canvas.SetTop(rct_Selection, pressed.Y);
                    rct_Selection.Height = mousePos.Y - pressed.Y;
                }
                else
                {
                    Canvas.SetTop(rct_Selection, mousePos.Y);
                    rct_Selection.Height = pressed.Y - mousePos.Y;
                }

                //select each item
 
                foreach (var item in directoryFiles.Items)
                {
              
                    var itemContainer = directoryFiles.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                    
                    Point P1 = itemContainer.TransformToAncestor(gridFiles).Transform(new Point(0, 0));
                    Point P2 = new Point(P1.X + 230, P1.Y + 60);


                    Point P3 = new Point(Canvas.GetLeft(rct_Selection), Canvas.GetTop(rct_Selection));

                    
                    Point P4 = new Point(P3.X + rct_Selection.Width, P3.Y + rct_Selection.Height);
                    if(! ( P2.Y < P3.Y || P1.Y > P4.Y || P2.X < P3.X || P1.X > P4.X ))
                    {
                        itemContainer.IsSelected = true;
                    }
                    else itemContainer.IsSelected = false;
                   
                }
   

            }

        }

        //mouseRelease
        private void ListBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            mousePressedOnListbox = false;
            rct_Selection.Visibility = Visibility.Collapsed;
            //gridFiles.ReleaseMouseCapture();



            Point mouseUpPos = e.GetPosition(gridFiles);
           
        }

        private void buttonFileUp_Click(object sender, RoutedEventArgs e)
        {
            RefreshIconsView(explorer.upperPath);
        }

        private void RefreshIconsView(string path=null)
        {
            if (path == null)
            {
                directoryFiles.ItemsSource = explorer.RebuildIconsView();
            }
            else directoryFiles.ItemsSource = explorer.RebuildIconsView(path);
            //filePathCurrent.Text = explorer.dirPath;
            buttonFileUp.IsEnabled = explorer.allowGoingUp;
        }

        private void directoryFiles_DragOver(object sender, DragEventArgs e)
        {
            int iter = 0;
            foreach (var item in directoryFiles.Items)
            {
                var itemContainer = directoryFiles.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
                
                Point P1 = itemContainer.TransformToAncestor(gridFiles).Transform(new Point(0, 0));
                Point P2 = new Point(P1.X + 230, P1.Y + 60);

                Point P=e.GetPosition(gridFiles);
             
                if (P.X>P1.X && P.X<P2.X && P.Y>P1.Y && P.Y<P2.Y)
                {

                    if(((ExplorerFile)(directoryFiles.Items[iter])).fileType.fileType==FileType.Directory) itemContainer.IsSelected = true;
                }
                else itemContainer.IsSelected = false;
                iter++;
            }
          
        }

        private void directoryFiles_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
        
            string basePath = files[0].Substring(0, files[0].LastIndexOf('\\') + 1);
            var newWindow = new CopyFilesWindow(explorer.dirPath,basePath,files);
            
            newWindow.Show();
            
            RefreshIconsView();
        }


        void ListItem_MouseDown(object sender, RoutedEventArgs e)
        {

            string newPath = explorer.dirPath + directoryFiles.SelectedItem.ToString() + @"\";

            RefreshIconsView(newPath);
        }

    }
}
