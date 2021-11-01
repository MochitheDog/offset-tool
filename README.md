# Offset Tool
Offset Tool is a GUI application to easily edit the music offsets of StepMania chart files (.sm and .ssc). Entire folders and subfolders of files can be edited, as well as single files.
Offset Tool will also automatically create backups of your files in case of mistakes.

For an explanation of music offsets and sync in StepMania, see this link: https://wiki.clubfantastic.dance/Sync#ITG%20Offset%20And%20The%209ms%20Bias

Written in C# using .NET Core 3.1.

# How to Use
1. Click File -> Open Folder. Select the folder where your .sm and/or .ssc files are located.
	- Click File -> Open File for single files.
3. A list of your .sm and .ssc files will appear in the window. Click to select the files to apply your offset to, or click the "Select All" to select everything.
4. Enter the desired offset in the text box next to "Offset to Add".
	- If editing from **null/+0ms** to **ITG/+9ms**, enter **0.009**.
	- If editing from **ITG/9ms** to **null/+0ms**, enter **-0.009**.
5. Click "Apply Offset" button.
