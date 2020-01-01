﻿/*
Datascraper for Google drive, after making all the files in you drive load, press F12, in the opened window 
go to elements and copy the the whole html body tag of the web page and save it in a txt file
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

            static string bodyTagString; //body tag as stringk
            static char[] bodyTag; //body tag as array of characters
            static int fileNameStart; //start of the name of a file
            static int fileNameEnd; //end of the name of a file
            static int fileNameLength; //length of the name of a file
            static string fileName = ""; //name of a file
            static int latestPos = 0; //latest position of the found file name
            static StringBuilder sb = new StringBuilder();
        static void Main(string[] args)
        {
            string txtFilePath;
            Console.Write("Drop the txt containing the body tag here or type its path: ");
            txtFilePath = Console.ReadLine();
        
            //takes data from txt file and decodes it using the HttpUtility(or else for example '&' would be shown as '&amp;')
            using(StreamReader sr = new StreamReader(File.OpenRead(txtFilePath)))
            {
                bodyTagString = sr.ReadToEnd();
                bodyTagString = HttpUtility.HtmlDecode(bodyTagString);
                bodyTag = bodyTagString.ToCharArray();
            }
            
            Console.Write("Scraping data...");      
            #region Scraping process      
            List<String> fileNames = new List<string>();
            Thread thr = new Thread(() => 
            {
                fileNames = FindFilesName();
            });
            thr.IsBackground = true;
            thr.Start();
            while(thr.IsAlive)
            {
                Console.Write(".");
                Thread.Sleep(200);
            }
            #endregion
            
            Console.WriteLine("\nFiles:");
            int fileIndex = 1;
            foreach(var name in fileNames)
            {
                Console.WriteLine($"{fileIndex}) {name}");
                fileIndex++;
            }

            Console.WriteLine("\nDo you want to export the list of the files in a txt file? It'll be saved on the desktop (Press enter if yes, Ctrl+C to exit)");
            Console.ReadLine();
            string fileNamesPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.Desktop)}\\GoogleDriveFiles.txt";
            using(StreamWriter sw = File.CreateText(fileNamesPath))
            {
                fileIndex = 1;
                foreach(var name in fileNames)
                {
                    sw.WriteLine($"{fileIndex}) {name}");
                    fileIndex++;
                }
            }
            Process.Start(@"cmd.exe ", $"/c {Path.GetFullPath(fileNamesPath)}");
        }
   
        private static List<String> FindFilesName()
        {
            List<String> fileNames = new List<string>();
            for (int i=latestPos; i<=bodyTag.Length-1; i++)
            {
                sb.Clear();
                if(
                bodyTag[i] == '"' && bodyTag[i+1] == ' ' && bodyTag[i+2] == 'd' && bodyTag[i+3] == 'a' && bodyTag[i+4] == 't'
                && bodyTag[i+5] == 'a'&& bodyTag[i+6] == '-'&& bodyTag[i+7] == 't'&& bodyTag[i+8] == 'o'&& bodyTag[i+9] == 'o'
                && bodyTag[i+10] == 'l'&& bodyTag[i+11] == 't'&& bodyTag[i+12] == 'i'&& bodyTag[i+13] == 'p'&& bodyTag[i+14] == '-'
                && bodyTag[i+15] == 'u'&& bodyTag[i+16] == 'n'&& bodyTag[i+17] == 'h'&& bodyTag[i+18] == 'o'&& bodyTag[i+19] == 'v'
                && bodyTag[i+20] == 'e'&& bodyTag[i+21] == 'r'&& bodyTag[i+22] == 'a'&& bodyTag[i+23] == 'b'&& bodyTag[i+24] == 'l'
                && bodyTag[i+25] == 'e' && bodyTag[i+26] == '='&& bodyTag[i+27] == '"'&& bodyTag[i+28] == 't' && bodyTag[i+29] == 'r'
                && bodyTag[i+30] == 'u' && bodyTag[i+31] == 'e' && bodyTag[i+32] == '"' && bodyTag[i+33] == '>' && bodyTag[i+34] != '<')
                {
                    fileNameStart = i+34;
                    for(int ii = fileNameStart; ii<=bodyTag.Length-1;ii++)
                    {
                        if(bodyTag[ii] == '<' && bodyTag[ii+1] == '/' && bodyTag[ii+2] == 'd' && bodyTag[ii+3] == 'i' && bodyTag[ii+4] == 'v' && bodyTag[ii+5] == '>')
                        {
                            fileNameEnd = ii-1;
                            fileNameLength = fileNameEnd-fileNameStart;
                            for (int iii = fileNameStart; iii<=fileNameEnd; iii++)
                            {
                                sb.Append(bodyTag[iii]);
                                fileName = sb.ToString();
                            }
                            fileNames.Add(fileName);
                            fileName = String.Empty;
                            sb.Clear();
                            latestPos = ii+4;
                            break;
                        }
                    }
                }
            }
            return fileNames;
        }
    }
}
