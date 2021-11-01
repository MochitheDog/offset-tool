using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace OffsetEditor
{
    // For chart info display in the future
    public struct Chart
    {
        public string Title {
            get; set;
        }
        public decimal Offset {
            get; set;
        }
        public string FilePath {
            get; set;
        }
    }

    public class ChartFileHandler
    {
        string offsetPattern = @"(#OFFSET:)(\s*)([-+]?\d*\.\d*)(\s*)(;)";
        string titlePattern = @"(#TITLE:)([^\n]+)(;)";
        string offsetInnerPattern = @"([-+]?\d*\.\d*)";

        // Apply the given offset to each file in the list of files
        // Copy the original file to a backup
        // Return number of files successfully edited
        public List<string> ApplyOffset(List<string> files, decimal offsetToAdd)
        {
            List<string> failedFiles = new List<string>();
            foreach (string path in files)
            {
                if (File.Exists(path) && (Path.GetExtension(path).ToLower() == ".sm" || Path.GetExtension(path).ToLower() == ".ssc"))
                {
                    decimal oldOffset = 0.0M;
                    // Reads the whole file as a string, replaces stuff in it then writes it again. Easiest/simplest way to handle it,
                    // Stepmania files aren't that big anyway. Or shouldn't be, at least
                    string textFile;
                    try
                    {
                        textFile = File.ReadAllText(path);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Failed to read file " + path + ". Exception: " + e.Message);
                        failedFiles.Add(path);
                        continue;
                    }
                    
                    // Copy original file as backup
                    File.Copy(path, string.Format("{0}.OLDOFFSET", path));

                    // Offset parsing - read file
                    // SSCs may have multiple #OFFSET:'s
                    MatchCollection matchOffsets = Regex.Matches(textFile, offsetPattern);

                    foreach (Match capturedOffset in matchOffsets)
                    {
                        // Extract offset value from matched string
                        // Doing this because something like '#OFFSET:      0.01     ;' is still valid SM
                        string offsetString = capturedOffset.Value; // #OFFSET:0.1234;
                        oldOffset = decimal.Parse(Regex.Match(offsetString, offsetInnerPattern).Value); // Just the number now

                        // Unlikely, but possible that oldoffset needs rounding to 3 decimal places
                        // Offsets *should* only be 3 decimal places to begin with, so rounding before or after adding
                        // is kind of an arbitrary design decision
                        decimal newOffset = decimal.Round(oldOffset, 3) + offsetToAdd;
                        textFile = textFile.Replace(offsetString, string.Format("#OFFSET:{0};", newOffset.ToString())); // #OFFSET:newOffset
                    }
                    try
                    {
                        File.WriteAllText(path, textFile);
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine("Failed to write file " + path + ". Exception: " + e.Message);
                        failedFiles.Add(path);
                        continue;
                    }
                }
            }
            return failedFiles;
        }

        // This could be used for displaying chart info, at some later point
        public Chart ParseForInfo(string path)
        {
            decimal oldOffset = 0.0M;
            string title = "";
            if (File.Exists(path) && Path.GetExtension(path).ToLower() == ".sm")
            {
                string textFile = File.ReadAllText(path);

                // Offset parsing
                Match m = Regex.Match(textFile, offsetPattern);
                if (m.Success)
                {
                    // Extract offset value from matched string
                    // Doing this because something like '#OFFSET:      0.01     ;' is still valid SM
                    string match = m.Value;
                    Match matchOffsetInner = Regex.Match(match, offsetInnerPattern);
                    if (matchOffsetInner.Success)
                    {
                        oldOffset = decimal.Parse(matchOffsetInner.Value);
                    }
                }

                // Title parsing
                Match titleMatch = Regex.Match(textFile, titlePattern);
                if (m.Success)
                {
                    string match = titleMatch.Value;
                    // Remove '#TITLE:' and semicolon
                    title = match.Replace("#TITLE:", "").Replace(";", "");
                }
            }
            else
            {
                // TODO: handle file error
            }

            Chart chartInfo = new Chart
            {
                Title = title,
                Offset = oldOffset,
                FilePath = path,
            };
            return chartInfo;
        }
    }
}
