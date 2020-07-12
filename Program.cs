/*

MIT License

Copyright (c) 2020 Alessandro Dinardo

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

.__          .     .__             .__     ,       __.                  
[ __ _  _  _ | _   |  \._*.  , _   |  \ _.-+- _.  (__  _ ._ _.._  _ ._
[_./(_)(_)(_]|(/,  |__/[ | \/ (/,  |__/(_] | (_]  .__)(_ [ (_][_)(/,[  
          ._|                                                   |        

    Go to the folder where your files are and set the view by grid,reload the page or copy the link in the url bar 
    and paste that link in another tab, after making the files you want in your final list load(sometimes they'll load 
    only by selecting them) press F12, go to the Elements tab in the opened window, copy the Body element, paste it in 
    a txt file and save it. Finally drop it on the GoogleDriveDataScraper executable or use it normally
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Web;

namespace Google_Drive_data_scraper
{
    class Program
    {
        static List<String> driveFiles;
        static void Main(string[] args)
        {
            #region Print logo
            Console.WriteLine(".__          .     .__             .__     ,       __.                \n[ __ _  _  _ | _   |  \\._*.  , _   |  \\ _.-+- _.  (__  _ ._ _.._  _ ._\n[_./(_)(_)(_]|(/,  |__/[ | \\/ (/,  |__/(_] | (_]  .__)(_ [ (_][_)(/,[  \n          ._|                                                   |        \n");
            for (int i = 1; i <= Console.BufferWidth; i++)
            {
                Console.Write("*");
            }
            #endregion

            string txtFilePath;
            //if program is exectued by command line parse the arguments and set the txtFilePath
            if (args.Length != 0 && args != null)
            {
                ParseAndExecuteArguments(args);
                txtFilePath = args[0];
            }
            //else ask the user for the path
            else
            {
                Console.Write("\nDrop the txt containing the body tag here or type its path: ");
                txtFilePath = Console.ReadLine();

                StartScraping(txtFilePath);
                CompareListsOfFiles();
                SaveList();
            }
            Environment.Exit(0);
        }

        /// <summary>
        /// Parses the arguments passed through the command line and executes them
        /// </summary>
        /// <param name="arguments"> Arguments passed to the program</param>
        static void ParseAndExecuteArguments(string[] arguments)
        {
            if (File.Exists(arguments[0])) //if the txt file has been passed as argument
            {
                string txtFilePath = arguments[0];
                StartScraping(txtFilePath);
                CompareListsOfFiles();
                SaveList();

            }
            else //if the arguments are commands given by command line
            {
                if (arguments[0] == "-scrape")
                {
                    StartScraping(arguments[1]);
                }
                if (arguments.Length > 2)
                {
                    if (arguments[2] == "-save")
                    {
                        SaveList();
                    }
                }
            }
            Environment.Exit(0);
        }


        /// <summary>
        /// Starts the process of scraping the names of the files
        /// </summary>
        /// <param name="txtFilePath"> Path to the txt file which contains the Body tag </param>
        static void StartScraping(string txtFilePath)
        {
            //check if the path leads to a txt file
            if (!txtFilePath.Contains(".txt"))
            {
                PrintError("Invalid path, it doesn't lead to a txt file");
                return;
            }
            //debug code, removes double quotes from input
            if (txtFilePath.Contains('"'))
            {
                string buffTxtFilePath = txtFilePath;
                txtFilePath = buffTxtFilePath.Trim('"');
            }

            Console.Write("Scraping data...");
            #region Scraping process      
            driveFiles = new List<string>();
            Thread thr = new Thread(() =>
            {
                driveFiles = FindFilesName(txtFilePath);
            });
            thr.IsBackground = true;
            thr.Start();
            while (thr.IsAlive)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
            #endregion

            Console.WriteLine("\nFiles:");
            int fileIndex = 1;
            foreach (var name in driveFiles)
            {
                Console.WriteLine($"{fileIndex}) {name}");
                fileIndex++;
            }
        }


        /// <summary>
        /// Finds the files in the user's Drive
        /// </summary>
        /// <param name="txtFilePath"> Path to the txt file which contains the Body tag</param>
        /// <returns> Returns a List of type string which contains all the names of the files found in the Body tag</returns>
        private static List<String> FindFilesName(string txtFilePath)
        {
            int latestPos = 0; //latest position of the found file name
            string bodyTagString; //body tag as string
            char[] bodyTag; //body tag as array of characters
            int fileNameStart; //start of the name of a file
            int fileNameEnd; //end of the name of a file
            int fileNameLength; //length of the name of a file
            string fileName = ""; //name of a file
            StringBuilder sb = new StringBuilder();

            //takes data from txt file and decodes it using the HttpUtility(or else for example '&' would be shown as '&amp;')
            using (StreamReader sr = new StreamReader(File.OpenRead(txtFilePath)))
            {
                bodyTagString = sr.ReadToEnd();
                bodyTagString = HttpUtility.HtmlDecode(bodyTagString);
                bodyTag = bodyTagString.ToCharArray();
            }

            List<String> fileNames = new List<string>();
            for (int i = latestPos; i <= bodyTag.Length - 1; i++)
            {
                sb.Clear();
                if (
                bodyTag[i] == '"' && bodyTag[i + 1] == ' ' && bodyTag[i + 2] == 'd' && bodyTag[i + 3] == 'a' && bodyTag[i + 4] == 't'
                && bodyTag[i + 5] == 'a' && bodyTag[i + 6] == '-' && bodyTag[i + 7] == 't' && bodyTag[i + 8] == 'o' && bodyTag[i + 9] == 'o'
                && bodyTag[i + 10] == 'l' && bodyTag[i + 11] == 't' && bodyTag[i + 12] == 'i' && bodyTag[i + 13] == 'p' && bodyTag[i + 14] == '-'
                && bodyTag[i + 15] == 'u' && bodyTag[i + 16] == 'n' && bodyTag[i + 17] == 'h' && bodyTag[i + 18] == 'o' && bodyTag[i + 19] == 'v'
                && bodyTag[i + 20] == 'e' && bodyTag[i + 21] == 'r' && bodyTag[i + 22] == 'a' && bodyTag[i + 23] == 'b' && bodyTag[i + 24] == 'l'
                && bodyTag[i + 25] == 'e' && bodyTag[i + 26] == '=' && bodyTag[i + 27] == '"' && bodyTag[i + 28] == 't' && bodyTag[i + 29] == 'r'
                && bodyTag[i + 30] == 'u' && bodyTag[i + 31] == 'e' && bodyTag[i + 32] == '"' && bodyTag[i + 33] == '>' && bodyTag[i + 34] != '<')
                {
                    fileNameStart = i + 34;
                    for (int ii = fileNameStart; ii <= bodyTag.Length - 1; ii++)
                    {
                        if (bodyTag[ii] == '<' && bodyTag[ii + 1] == '/' && bodyTag[ii + 2] == 'd' && bodyTag[ii + 3] == 'i' && bodyTag[ii + 4] == 'v' && bodyTag[ii + 5] == '>')
                        {
                            fileNameEnd = ii - 1;
                            fileNameLength = fileNameEnd - fileNameStart;
                            for (int iii = fileNameStart; iii <= fileNameEnd; iii++)
                            {
                                sb.Append(bodyTag[iii]);
                                fileName = sb.ToString();
                            }
                            fileNames.Add(fileName);
                            fileName = String.Empty;
                            sb.Clear();
                            latestPos = ii + 4;
                            break;
                        }
                    }
                }
            }
            return fileNames;
        }


        /// <summary>
        /// Compares files from the list to the files in a folder
        /// </summary>
        /// <param name="folderPath"> Folder that contains the files to compare to the ones in the user's Drive</param>
        /// <param name="driveFiles"> A list which contains the files in the user's Drive. It is equal to the list of type string "fileNames"</param>
        /// <returns> Returns a tuple which contains: 
        /// - List<String> missingFilesFromFolder: files missing from the folder when compared to the drive
        /// - List<String> missingFilesFromDrive: files missing from the drive when compared to the folder
        /// </returns>
        private static void CompareListsOfFiles()
        {
            List<String> buffer = new List<string>();
            List<String> missingFilesFromDrive = new List<string>();
            List<String> missingFilesFromFolder = new List<string>();

AskPath:
            #region Asks if the user wants to compare the drive files with a local folder, if yes asks its path
            Console.Write("\nDo you want to compare the list of files to a local folder? (Type the path of the folder if yes, enter if not) \nPath: ");
            string folderPath = Console.ReadLine();
            if (folderPath != String.Empty)
            {
                if (Path.HasExtension(folderPath) || !Directory.Exists(folderPath))
                {
                    PrintError("Invalid path");
                    Console.WriteLine("Retry?(Y/N)");
                    var prssdKey = Console.ReadKey();
                    if (prssdKey.KeyChar != 'n' || prssdKey.KeyChar != 'N') //Wrote "if not equal to n or N" because the user might start writing the path again without reading the prompt asking to input Y or N
                    {
                        goto AskPath;
                    }
                }
            }
            #endregion

            #region Gets files from the folder
            List<String> folderFiles = new List<string>();
            foreach (var file in Directory.GetFileSystemEntries(folderPath))
            {
                folderFiles.Add(Path.GetFileName(file));
            }
            #endregion

            #region Starts comparing the files in the body tag and the local folder
            buffer = new List<String>(driveFiles);
            foreach (var fileInFolder in folderFiles)
            {
                if (buffer.Contains(fileInFolder))
                {
                    buffer.Remove(fileInFolder);
                }
            }
            missingFilesFromFolder = buffer;

            buffer = new List<String>(folderFiles);
            foreach (var fileInDrive in driveFiles)
            {
                if (buffer.Contains(fileInDrive))
                {
                    buffer.Remove(fileInDrive);
                }
            }
            missingFilesFromDrive = buffer;

            if (missingFilesFromDrive.Count == 0) missingFilesFromDrive = null;
            if (missingFilesFromFolder.Count == 0) missingFilesFromFolder = null;
            #endregion

            #region Writes the files missing from the folder and the drive
            if (missingFilesFromDrive != null)
            {
                Console.WriteLine("\nThese files are missing from your Drive:");
                foreach (var file in missingFilesFromDrive)
                {
                    Console.WriteLine($"{missingFilesFromDrive.IndexOf(file) + 1}) {file}");
                }
            }
            if (missingFilesFromFolder != null)
            {
                Console.WriteLine($"\nThese files are missing from your folder {Path.GetDirectoryName(folderPath)}:");
                foreach (var file in missingFilesFromFolder)
                {
                    Console.WriteLine($"{missingFilesFromFolder.IndexOf(file) + 1}) {file}");
                }
            }
            #endregion
            Console.ReadLine();
        }


        /// <summary>
        /// Saves the list of the files in the user's Drive in a txt file
        /// </summary>
        static void SaveList()
        {
            Console.WriteLine("\nDo you want to export the list of the files in a txt file? It'll be saved on the desktop (Y/N)");
            var resp = Console.ReadKey();
            if (resp.KeyChar != 'y' || resp.KeyChar != 'Y')
                return;

            string filesNameTxtPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\GoogleDriveFiles.txt";
            using (StreamWriter sw = File.CreateText(filesNameTxtPath))
            {
                int fileIndex = 1;
                foreach (var name in driveFiles)
                {
                    sw.WriteLine($"{fileIndex}) {name}");
                    fileIndex++;
                }
            }
            Process.Start(@"cmd.exe ", $"/c {Path.GetFullPath(filesNameTxtPath)}");
        }


        /// <summary>
        /// Prints an error after changing the foreground color to red
        /// </summary>
        /// <param name="errorMessage"> Message to print </param>
        private static void PrintError(String errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: {errorMessage}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadLine();
        }
    }
}
