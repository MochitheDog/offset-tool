using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace OffsetEditor
{
    static class FileIOHandler
    {
        // Dir should be pack folder level path, not song folder level
        public static List<string> ReadFilesFromDirectory(string dir)
        {
            List<string> allFiles = new List<string>();
            // Read files from dir and recurse into subdirs
            // eventually want to return List<string> of all files (.sm and .ssc)
                
            try
            {
                // Using GetFiles instead of EnumerateFiles because usually stepmania song packs are relatively small (<100 songs)
                allFiles.AddRange(Directory.EnumerateFiles(dir, "*.sm").ToList<string>());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                MessageBox.Show(e.Message, "Open Folder Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            try
            {
                allFiles.AddRange(Directory.EnumerateFiles(dir, "*.ssc").ToList<string>());
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);
                MessageBox.Show(e.Message, "Open Folder Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            // Recurse into subdirectories
            string[] subdirectoryEntries = Directory.GetDirectories(dir);
            if (subdirectoryEntries.Length > 0)
            {
                foreach (string subdirectory in subdirectoryEntries)
                {
                    allFiles.AddRange(ReadFilesFromDirectory(subdirectory));
                }
            }
            // Keeps things in order for user friendliness
            allFiles.Sort();
            return allFiles;
        }
        public static string OpenFile()
        {
            return "";
        }
    }
}
