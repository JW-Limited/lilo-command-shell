using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LABLibary;
using System.IO;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SQLitePCL;
using System.Data.SqlClient;
using System.Net;

namespace LILO.Shell
{
    public class Shell
    {
        [TestMethod]
        public void ExecuteCommand_LocalMode_ShouldExecuteCommand()
        {
            string command = "echo 'Hello World'";
            ShellMode initMode = ShellMode.Local;
            ExecuteCommand(command);
            Assert.AreEqual("Hello World", Console.Out.ToString());
        }

        [TestMethod]
        public void ExecuteCommand_RemoteMode_ShouldNotExecuteCommand()
        {
            string command = "echo 'Hello World'";
            ShellMode initMode = ShellMode.Company;
            ExecuteCommand(command);
            Assert.AreNotEqual("Hello World", Console.Out.ToString());
        }

        public ShellMode initMode;
        private Thread thr;
        public string dirMedia = "C:\\LILO\\req\\media";
        public string dirImp = "C:\\LILO\\req\\media\\imported";

        public Shell(ShellMode mode) 
        {
            this.initMode = mode;
            thr = new Thread(Run);
            thr.Start();
        }

        public async void Run()
        {
            Console.Clear();
            Console.WriteLine($"Welcome to the \"{initMode.ToString()}\" LILO Shell!\n");
            while (true)
            {
                Console.Write($"{initMode.ToString()} / $>");
                string command = Console.ReadLine();
                switch (command)
                {
                    case "exit":
                    break;
                    case "":
                    continue;
                    case "new":
                        Process.Start(".\\LILO.exe", "shell");
                    break;
                    case "mode":
                        Console.Write("\nPlease enter the mode you would like to switch to: ");
                        string mode = Console.ReadLine();
                        if (Enum.IsDefined(typeof(ShellMode), mode))
                        {
                            initMode = (ShellMode)Enum.Parse(typeof(ShellMode), mode);
                            Console.ForegroundColor = ConsoleColor.Cyan;
                            Console.WriteLine("Succesfull switched.\n");
                            Console.ResetColor();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Invalid mode entered.\n");
                            Console.ResetColor();
                        }
                    break;
                    default:
                        try
                        {
                            ExecuteCommand(command);
                        }
                        catch (Exception ex)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"Invalid entry.{ex.Message}\n");
                            Console.ResetColor();
                        }
                    break;
                }
            }
        }


        public static bool UserCredentialDialog()
        {
            Console.WriteLine("Please enter your username: ");
            string username = Console.ReadLine();
 
            Console.WriteLine("Please enter your password: ");
            string password = Console.ReadLine();
 
            Console.WriteLine("Authenticating User Credentials...");
 
            if (LoginIsSuccessful(username,password))
            {
                Console.WriteLine("Success! You have been logged in.");
                return true;
            }
            else
            {
                Console.WriteLine("Login failed. Please try again.");
                return false;
            }
        }

        public static bool LoginIsSuccessful(string username, string password) 
        {
            bool isValidUser = false; 
            
            using (SqlConnection con = new SqlConnection("Data Source=.\\user.db"))  
            {
                con.Open(); 
                string qry = "SELECT 1 FROM users WHERE username=@username AND password=@password"; 
                using (SqlCommand cmd = new SqlCommand(qry, con)) 
                {
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@password", password);
                    SqlDataReader sdr = cmd.ExecuteReader(); 
                    if (sdr.HasRows) { 
                        isValidUser = true; 
                    } 
                }
            }
            return isValidUser; 
        }

        public void Search(string command)
        {
            string[] args = command.Split(' ');
            string fileName = args[1];
            string directoryPath = dirMedia;
            var rnd = new Random();

            List<string> filesListInDir = Directory.GetFiles(directoryPath, fileName, SearchOption.AllDirectories).ToList();
            if(filesListInDir.Count == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red; 
                Console.WriteLine($"File not found in the specified directory");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green; 
                Console.WriteLine("The following were found: \n");
                
                for (int i = 0; i < filesListInDir.Count; i++)
                {
                    Thread.Sleep(rnd.Next(1000, 2500));
                    Console.WriteLine(i + ": " + filesListInDir[i].Replace("C:\\LILO\\req\\",""));
                }

                Console.WriteLine("Please enter the number of the file you wish to select:");
                int userInput = Convert.ToInt32(Console.ReadLine());
                if(userInput >= 0 && userInput < filesListInDir.Count)
                {
                    string filePath = filesListInDir[userInput];
                    if(filePath.EndsWith(".laf")){
                        ShellExtentions.MediaExtentions.DecryptAudioFile(dirMedia + fileName, "liloDev420_audio");
                        Process.Start("explorer.exe", dirMedia + fileName.Replace(".laf",".mp3"));
                    }
                    else{
                        Process.Start("explorer.exe", filePath);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red; 
                    Console.WriteLine($"Invalid selection");
                    Console.ResetColor();
                }

                Console.ResetColor();
            }
        }
        public void PushFromURL(string url)
        {
            string fileName = Path.GetFileName(url);

            if (File.Exists(dirImp + "\\" + fileName) || File.Exists(dirMedia + "\\" + fileName))
            {
                Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("File already in the database.");
                Console.ResetColor();
                return;
            }

            WebClient webClient = new WebClient();
            string filePath = dirImp+ "\\" + fileName;
            webClient.DownloadFile(url, filePath);
            Console.ForegroundColor = ConsoleColor.Cyan; 
            Console.WriteLine($"File succesfully loaded up.(You can access this File from {filePath})");
            Console.ResetColor();
        }
        private void ExecuteCommand(string command)
        {
            switch (initMode)
            {
                case ShellMode.Local:
                    ProcessStartInfo startInfo = new ProcessStartInfo("lghcon.exe", command);
                    startInfo.RedirectStandardOutput = true;
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                    using (Process process = Process.Start(startInfo))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            Console.WriteLine(result);
                        }
                    }
                    break;
                case ShellMode.Media:
                    if (command.StartsWith("get"))
                    {
                        string fileName = command.Split(' ')[1];
                        if(File.Exists(dirMedia + fileName))
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"Found \"{fileName}\" on server.");
                            Console.ResetColor();
                            ShellExtentions.MediaExtentions.DecryptAudioFile(dirMedia + fileName, "liloDev420_audio");
                            Process.Start("explorer.exe", dirMedia + fileName.Replace(".laf",".mp3"));
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"FileNotFound on specified server(http://localhost:8080).");
                            Console.ResetColor();
                        }
                    }
                    if (command.StartsWith("search")){
                        Search(command);
                    }
                    else if (command.StartsWith("push"))
                    {
                        string[] args = command.Split(' ');
                        string fileName = args[1];

                        if(File.Exists(dirImp + "\\" +fileName) || File.Exists(dirMedia + "\\" + fileName))
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow; Console.WriteLine("File already in the database.");
                            Console.ResetColor();
                            return;
                        }

                        if(!fileName.EndsWith("mp3"))
                        {
                            File.Copy(fileName, dirImp + "\\" + fileName);
                            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"File succesfully loaded up.(You can access this File from {"imported\\" + fileName})");
                            Console.ResetColor();
                        }
                        else
                        {
                            
                            ShellExtentions.MediaExtentions.EncryptAudioFile(fileName, "liloDev420_audio");
                            File.Copy(fileName.Replace(".mp3",".laf"), dirMedia + "\\" + Path.GetFileName(fileName.Replace(".mp3",".laf")));
                            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"File succesfully loaded up.(You can access this File from {"media\\" + fileName.Replace(".mp3", ".laf")})");
                            Console.ResetColor();
                        }


                    }
                    break;
                case ShellMode.Company:
                    if(!UserCredentialDialog())Console.WriteLine("This mode is not supported on you´re device.");
                    else{
                        ProcessStartInfo startInfo2 = new ProcessStartInfo("lghcon.exe", command);
                    startInfo2.RedirectStandardOutput = true;
                    startInfo2.UseShellExecute = false;
                    startInfo2.CreateNoWindow = true;
                    using (Process process = Process.Start(startInfo2))
                    {
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            Console.WriteLine(result);
                        }
                    }
                }
                    break;
            }
        }
    }
}
