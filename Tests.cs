using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LILO.Shell.Tests
{
    [TestClass]
    public class ShellTests
    {
        [TestMethod]
        public void SearchFile_ValidInput()
        {
            // Arrange
            var dirMedia = "C:/LILO/req/";
            string command = "search test.laf";
            string fileName = "test.laf";
            int userInput = 0;

            // Act
            var serviceToTest = new TestSearchService(dirMedia);
            serviceToTest.Search(command);
            string outputFromConsole = serviceToTest.GetOutputFromConsole();

            // Assert
            Assert.IsTrue(outputFromConsole.Contains($"The following were found: \n 0: {fileName}"));
            Assert.IsTrue(outputFromConsole.Contains($"Please enter the number of the file you wish to select:"));
            Assert.AreEqual(userInput, serviceToTest.SimulateUserInput());
            Assert.IsTrue(Directory.Exists($"{dirMedia}{fileName.Replace(".laf",".mp3")}"));
        }

        [TestMethod]
        public void SearchFile_InvalidInput()
        {
            // Arrange
            var dirMedia = "C:/LILO/req/";
            string command = "search test.laf";
            string fileName = "test.laf";
            int userInput = 10;

            // Act
            var serviceToTest = new TestSearchService(dirMedia);
            serviceToTest.Search(command);
            string outputFromConsole = serviceToTest.GetOutputFromConsole();

            // Assert
            Assert.IsTrue(outputFromConsole.Contains($"The following were found: \n 0: {fileName}"));
            Assert.IsTrue(outputFromConsole.Contains($"Please enter the number of the file you wish to select:"));
            Assert.AreEqual(userInput, serviceToTest.SimulateUserInput());
            Assert.IsFalse(Directory.Exists($"{dirMedia}{fileName.Replace(".laf",".mp3")}"));
            Assert.IsTrue(outputFromConsole.Contains($"Invalid selection"));
        }

        [TestMethod]
        public void SearchFile_NonexistentInput()
        {
            // Arrange
            var dirMedia = "C:/LILO/req/";
            string command = "search nonexistentfile.laf";

            // Act
            var serviceToTest = new TestSearchService(dirMedia);
            serviceToTest.Search(command);
            string outputFromConsole = serviceToTest.GetOutputFromConsole();

            // Assert
            Assert.IsTrue(outputFromConsole.Contains("File not found in the specified directory"));
        }
    }
    
    [TestClass]
    public class ShellExtentionsTests
    {
        [TestMethod]
        public void ShowError_ShouldShowErrorMessage()
        {
            // Arrange
            string message = "An error occurred";

            // Act
            ShellExtentions.ShowError(message);

            // Assert
            Assert.AreEqual($"Sorry, something went wrong : {message}", message);
        }

        [TestMethod]
        public void MediaExtentions_EncryptAudioFile_ShouldSucceed()
        {
            // Arrange
            string path = "C:\\audiofile.txt";
            string key = "secretkey";

            // Act
            ShellExtentions.MediaExtentions.EncryptAudioFile(path, key);

            //Assert
            string expectedfilename = Path.GetFileNameWithoutExtension(path) + ".laf";
            string actualfilename = Path.Combine(Path.GetDirectoryName(path), expectedfilename);
            FileInfo fileinfo = new FileInfo(actualfilename);
            Assert.IsTrue(fileinfo.Exists);
        }

        [TestMethod]
        public void MediaExtentions_DecryptAudioFile_ShouldSucceed()
        {
            // Arrange
            string path = "C:\\audiofile.laf";
            string key = "secretkey";

            // Act
            ShellExtentions.MediaExtentions.DecryptAudioFile(path, key);
            
            // Assert
            string expectedfilename = Path.GetFileNameWithoutExtension(path) + ".mp3";
            string actualfilename = Path.Combine(Path.GetDirectoryName(path), expectedfilename);
            FileInfo fileinfo = new FileInfo(actualfilename);
            Assert.IsTrue(fileinfo.Exists);
        }
    }
}
