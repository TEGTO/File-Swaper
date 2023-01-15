using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;
using DragEventArgs = System.Windows.DragEventArgs;

namespace File_Swaper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string firstFilePath, secondFilePath;
        public MainWindow()
        {
            InitializeComponent();
            settingsRead();
        }
        void AutoReplaceButton(object sender, RoutedEventArgs e)
        {
            if (RememberFiles.IsChecked)
            {
                if (!AutoReplace.IsCheckable)
                {
                    AutoReplace.IsCheckable = true;
                    AutoReplace.IsChecked = !AutoReplace.IsChecked  ;
                }
               
                AutoReplace.Opacity = AutoReplace.IsChecked ? 1 : 0.5;
            }
            else
            {
                AutoReplace.Opacity = 0.5;
                AutoReplace.IsChecked = false;
            }
        }
        void RememberFilesButton(object sender, RoutedEventArgs e)
        {
            if (!RememberFiles.IsChecked)
            {
                AutoReplace.IsCheckable = false;
                AutoReplace.IsChecked = false;
                AutoReplace.Opacity = 0.5;
            }
        }
        void settingsRead()
        {
            if (File.Exists("filesReplacerSettings.txt"))
            {
                using (StreamReader reader = new StreamReader("filesReplacerSettings.txt"))
                {
                    bool r = bool.TryParse(reader.ReadLine(), out r) ? r : true;
                    RememberFiles.IsChecked = r;
                    if (RememberFiles.IsChecked)
                    {
                        AutoReplace.Opacity = 1;
                        AutoReplace.IsCheckable = true;
                        r = bool.TryParse(reader.ReadLine(), out r) ? r : false;
                        AutoReplace.IsChecked = r;
                        FirstFileText.Text = reader.ReadLine();
                        SecondFileText.Text = reader.ReadLine();
                        if (AutoReplace.IsChecked)
                        {
                            ReplaceFiles();
                        }
                      
                       

                    }
                }
            }
        }
        private void dragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    if (defineSender(sender) == "FirstPanel")
                    {

                        FirstFileText.Text = files[0];
                    }
                    else
                    {

                        SecondFileText.Text = files[0];
                    }



                }

                layout2_DragLeave(sender, e);


            }
            catch (Exception ex)
            {

                ResultLabel.Content = ex.Message;
            }

        }
        private void openFileDialog(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "All files (*.*)|*.*";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                if (openFileDialog.ShowDialog() == true)
                {
                    if (defineSender(sender) == "FirstFileButton")
                    {
                        FirstFileText.Text = openFileDialog.FileName;
                    }
                    else
                    {
                        SecondFileText.Text = openFileDialog.FileName;
                    }
                }

            }
            catch (Exception ex)
            {
                ResultLabel.Content = ex.Message;

            }

        }
        private string defineSender(object sender)
        {
            DependencyObject obj = sender as DependencyObject;
            string name = obj.GetValue(NameProperty) as string;
            return name;
        }
        private void layout2_DragEnter(object sender, DragEventArgs e)
        {


          
            if (defineSender(sender) == "FirstPanel")
            {
                ResultLabel.Content = "First file. Drop it!";
                FirstPanel.Opacity = 0.5;
                FirstFileButton.Opacity = 0.5;

                SecondPanel.Opacity = 1;
                SecondFileButton.Opacity = 1;
            }
            else
            {
                ResultLabel.Content = "Second file. Drop it!";
                FirstPanel.Opacity = 1;
                FirstFileButton.Opacity = 1;
                SecondPanel.Opacity = 0.5;
                SecondFileButton.Opacity = 0.5;
            }
        }

        private void ReplaceFilesButton_Click(object sender, RoutedEventArgs e)
        {
            ReplaceFiles();



        }
        void ReplaceFiles()
        {
            try
            {
                firstFilePath = FirstFileText.Text;
                secondFilePath = SecondFileText.Text;
                if (firstFilePath == null || firstFilePath?.Length <= 0 || secondFilePath == null || secondFilePath?.Length <= 0)
                {
                    ResultLabel.Content = "All files paths are not entered";
                    return;
                }


                File.Copy(firstFilePath, secondFilePath, true);


                ResultLabel.Content = "Files are replaced";

            }
            catch (Exception ex)
            {

                ResultLabel.Content = ex.Message;
            }

        }
        private void layout2_DragLeave(object sender, DragEventArgs e)
        {
            FirstPanel.Opacity = 1;
            FirstFileButton.Opacity = 1;
            SecondPanel.Opacity = 1;
            SecondFileButton.Opacity = 1;
            ResultLabel.Content = "";
        }
        private void NewFilesButton (object sender, RoutedEventArgs e)
        {
            FirstFileText.Text = "";
            SecondFileText.Text = "";
            ResultLabel.Content = "";

        }
        bool closingApp(object sender)
        {
           
               
            

            using (StreamWriter writer = new StreamWriter("filesReplacerSettings.txt", false))
            {
                writer.WriteLine(RememberFiles.IsChecked.ToString());
                if (RememberFiles.IsChecked)
                {
                    writer.WriteLine(AutoReplace.IsChecked.ToString());
                    writer.WriteLine(FirstFileText.Text);
                    writer.WriteLine(SecondFileText.Text);
                }

            }

            System.Windows.Application.Current.Shutdown();
            return true;
        }
        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            closingApp(e);
           


        }
        private void exitFromApplication(object sender, RoutedEventArgs e) => closingApp(sender);
    }
}
