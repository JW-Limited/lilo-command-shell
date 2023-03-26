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

        public void GiveCommands(ShellMode mod)
        {
            Console.WriteLine($"Welcome to the \"{initMode.ToString()}\" LILO Shell!\n");
            switch (mod)
            {
                case ShellMode.Local:
                    Console.WriteLine("You can use this mode to manage youre Local installed LILO Apps:\r\n\nExample :\r\n\t- lghcon –-config          : \t\tOpens a configer in the Console\r\n\t- lilo install <package>   :\t\tInstalls a Package\r\n\nFull List on github\r\n"); break;
                case ShellMode.Media:
                    Console.WriteLine("You can use this mode to deal with any kind of data on your Local/Cloud/Company Server.\r\nCommands:\r\n\t- pull\r\n\t- push\r\n\t- erase\r\n\t- search\r\nExamples and code integration through API on GitHub.\r\n"); break;
            }
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

                            Console.Clear();
                            switch (initMode)
                            {
                                case ShellMode.Local:
                                    GiveCommands(ShellMode.Local); break;
                                case ShellMode.Company:
                                    GiveCommands(ShellMode.Company); break;
                                case ShellMode.Media:   
                                    GiveCommands(ShellMode.Media); break;
                            }
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
                Console.ResetColor();
                for (int i = 0; i < filesListInDir.Count; i++)
                {
                    Thread.Sleep(rnd.Next(10, 250));
                    Console.WriteLine(i + ": " + filesListInDir[i].Replace("C:\\LILO\\req",""));
                }
                Console.ForegroundColor = ConsoleColor.Green; 
                Console.WriteLine("Please enter the number of the file you wish to select:");
                Console.ResetColor();
                int userInput = Convert.ToInt32(Console.ReadLine());
                if(userInput >= 0 && userInput < filesListInDir.Count)
                {
                    string filePath = filesListInDir[userInput];
                    Console.WriteLine($"Opening {filePath.Replace("C:\\LILO\\req", "")}...");   
                    if(filePath.EndsWith(".laf")){
                        ShellExtentions.MediaExtentions.DecryptAudioFile(dirMedia + "\\" +fileName, "liloDev420_audio");
                        Process.Start("explorer.exe", dirMedia + "\\" + fileName.Replace(".laf",".mp3"));
                    }
                    else{
                        Process.Start("explorer.exe", filePath);
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red; 
                    Console.WriteLine($"Invalid selection.");
                    Console.ResetColor();
                }
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
                    ProcessStartInfo startInfo = new ProcessStartInfo("powershell.exe", command);
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
                            try{
                                ShellExtentions.MediaExtentions.DecryptAudioFile(dirMedia + fileName, "liloDev420_audio");
                            }
                            catch(Exception ex)
                            {  
                                ShellExtentions.MediaExtentionsv2.DecryptAudioFormat(dirMedia + fileName, "liloDev420_audio");
                            }
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
                        Console.ResetColor();
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
                            File.Copy(fileName, dirImp + "\\" + Path.GetFileName(fileName));
                            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"File succesfully loaded up.(You can access this File from {"imported\\" + fileName})");
                            Console.ResetColor();
                        }
                        else
                        {
                            
                            try{
                                ShellExtentions.MediaExtentions.EncryptAudioFile(fileName, "liloDev420_audio");
                            }
                            catch{
                                ShellExtentions.MediaExtentionsv2.CreateAudioFormat(fileName, "liloDev420_audio");
                            }
                            File.Copy(fileName.Replace(".mp3",".laf"), dirMedia + "\\" + Path.GetFileName(fileName.Replace(".mp3",".laf")));
                            Console.ForegroundColor = ConsoleColor.Cyan; Console.WriteLine($"File succesfully loaded up.(You can access this File from {"media\\" + fileName.Replace(".mp3", ".laf")})");
                            Console.ResetColor();
                        }


                    }
                    break;
                case ShellMode.Company:
                    if(!UserCredentialDialog())Console.WriteLine("This mode is not supported on you´re device.");
                    else{
                        ProcessStartInfo startInfo2 = new ProcessStartInfo("powershell.exe", command);
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