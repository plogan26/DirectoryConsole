using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.AccessControl;


namespace Directory_Sizes_Book_Example
{
    class Program
    {


        private static double driveTotal;

        public static double setDriveTotal
        {
            get { return driveTotal; }
            set { driveTotal = value; }
        }


        static void Main(string[] args)
        {
            double x = 0;   //x variable is used to collect the cumulative file sizes of the directory or sub currently being analyzed. 
                            //The value stored here is passed to the global variable for calculating the overall size of the drive
            
            double dTotal = 0; //Variable is used to hold and manipulate the size of the drive before display

            string tag = ""; //will be used to display mb or gb, which ever is appropriate
            string totalTag = "mb"; //used for final display
            string startMessage; //Used in each display.
                                 //variable is used so the message can be tailored if no files were found in a given directory or sub                    

            DirectoryInfo dir = new DirectoryInfo(@"C:\"); //Alter path to indicate what directory you want analyzed

            foreach (FileInfo f in dir.EnumerateFiles())    //
            {                                               //This "first pass" is used to calculate the cumulative size
                x += f.Length;                              //of any files stored in the starting directory indicated by the users
            }                                               //

            setDriveTotal += x; //size of files in start directory are passed to the global for display of total drive size

            if (x != 0) //if there were files present in the current directory
            {
                startMessage = "\t[Size: ";
                if (x / Math.Pow(10, 5) < 5000) //if cumulative size is better represented in mb than gb
                {
                    x = x / Math.Pow(1024, 2);
                    tag = "mb]";
                }
                else //or in gb instead of mb
                {
                    x = x / Math.Pow(1024, 3);
                    tag = "gb]";
                }
            }
            else// if no files were present
            {
                startMessage = "\t[No files]";
                tag = "";
            }

            Console.WriteLine(dir.FullName + startMessage + x.ToString("#.##") + tag); //writes results from start directory scan

            foreach (DirectoryInfo subDir in dir.EnumerateDirectories())
            {
                try //try/catch is used simply to remove the scanning of unaccessable directories due to security.
                {   
                    x = 0;
                    x = CalculateDirectorySize(subDir, true);
                    setDriveTotal += x;

                    if (x != 0)
                    {
                        startMessage = "\t[Size: ";
                        if (x / Math.Pow(10, 5) < 5000)
                        {
                            x = x / Math.Pow(1024, 2);
                            tag = "mb]";
                        }
                        else
                        {
                            x = x / Math.Pow(1024, 3);
                            tag = "gb]";
                        }
                    }
                    else
                    {
                        startMessage = "\t[No files]";
                        tag = "";
                    }

                    Console.WriteLine("Dir: " + subDir.FullName + startMessage + x.ToString("#.##") + tag);
                }
                catch
                {
                    Console.WriteLine("[ACCESS DENIED - " + subDir.FullName + "]");
                }
            }

            dTotal = setDriveTotal / Math.Pow(1024, 2);

            if (dTotal > 1000)
            {
                dTotal = dTotal / 1024;
                totalTag = "gb";
            }

            Console.WriteLine("\n\n");
            Console.WriteLine("Drive total size: " + dTotal.ToString("#.##") + totalTag);
        }

        static long CalculateDirectorySize(DirectoryInfo directory, bool includeSubdirectories)
        {
            long totalSize = 0;

            foreach (FileInfo file in directory.EnumerateFiles())
            { totalSize += file.Length; }

            if (includeSubdirectories)
            {
                foreach (DirectoryInfo dir in directory.EnumerateDirectories())
                { totalSize += CalculateDirectorySize(dir, true); }
            }

            return totalSize;
        }
    }
}