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
