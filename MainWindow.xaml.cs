using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;


namespace OffsetEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ChartFileHandler chartFileHandler = new ChartFileHandler();
        public MainWindow()
        {
            InitializeComponent();
        }

        // Open folder button
        private void OpenFolderButtonClick(object sender, RoutedEventArgs e)
        {
            // Open a file dialog to select folder
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                FileList.Items.Clear();
                string folder = dialog.FileName;
                List<string> files = FileIOHandler.ReadFilesFromDirectory(folder);

                // Display list of files to screen
                foreach (string f in files)
                {
                    FileList.Items.Add(f);
                }
            }
        }

        private void OpenFileButtonClick(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            CommonFileDialogFilter filter = new CommonFileDialogFilter("Stepmania Chart Files", "sm,ssc");
            dialog.Filters.Add(filter);
            CommonFileDialogResult result = dialog.ShowDialog();
            if (result == CommonFileDialogResult.Ok)
            {
                FileList.Items.Clear();
                string file = dialog.FileName;
                FileList.Items.Add(file);
            }
        }

        private void ApplyOffsetButtonClick(object sender, RoutedEventArgs e)
        {
            //textboxOffset.Text
            // Verify if what's in the text box is actually a valid decimal
            bool isValid = decimal.TryParse(textboxOffset.Text, out _);
            if (isValid)
            {
                if (FileList.SelectedItems.Count > 0)
                {
                    String confirmMsg = String.Format("A value of {0} will be ADDED to the offset of {1} files. " +
                            "A backup of the original file will be created. Apply the offset?",
                        decimal.Parse(textboxOffset.Text),
                        FileList.SelectedItems.Count
                    );
                    MessageBoxResult res = MessageBox.Show(confirmMsg, "Confirm apply offset", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (res == MessageBoxResult.OK)
                    {
                        List<string> listOfFiles = FileList.SelectedItems.Cast<string>().ToList();
                        decimal offset = decimal.Parse(textboxOffset.Text);
                        List<string> failedFiles = chartFileHandler.ApplyOffset(listOfFiles, offset);
                        MessageBox.Show(string.Format("Offset applied. Failed to apply offset to {0} files.", failedFiles.Count));
                    }
                }
                else
                {
                    MessageBox.Show("No files were selected. Please select at least 1 file to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            }
            else
            {
                MessageBox.Show("Invalid input for Offset to Add. Please check that your input is a valid number.", "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        // Ensure the user can only enter valid decimal values in offset box. Cancels the input if not valid
        private void OffsetInputValidation(object sender, TextCompositionEventArgs e)
        {
            string text = (sender as TextBox).Text.Insert((sender as TextBox).SelectionStart, e.Text);
            if (!decimal.TryParse(text, out _))
            {
                e.Handled = true; // I think this makes it think the event was already handled so input is thrown out?
            }
            else
            {
                // Check if more than 3 decimal places
                decimal valueDecimal = decimal.Parse(text);
                if (Decimal.Round(valueDecimal, 3) != valueDecimal)
                {
                    // Too many decimals- not valid
                    e.Handled = true;
                }
            }
        }

        private void SelectAllButtonClick(object sender, RoutedEventArgs e)
        {
            FileList.SelectAll();
        }

        private void SelectNoneButtonClick(object sender, RoutedEventArgs e)
        {
            FileList.SelectedItems.Clear();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            MessageBoxResult res = MessageBox.Show(
                "Do you want to exit the application?",
                "Confirm exit",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );
            if (res == MessageBoxResult.Yes)
            {
                App.Current.Shutdown();
            }
        }

        private void AboutButtonClick(object sender, RoutedEventArgs e)
        {
            AboutWindow subWindow = new AboutWindow();
            subWindow.Show();
        }

        private void HelpButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
