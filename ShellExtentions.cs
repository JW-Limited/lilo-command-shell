using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LILO.Shell;
public class ShellExtentions
{
    public static void ShowError(string message)
    {
        MessageBox.Show($"Sorry, something went wrong : {message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);       
    }

    public class MediaExtentions
    {
        public static void EncryptAudioFile(string path, string key)
        {
            try
            {
                // Create a FileStream to read the audio file
                using (var sourceFile = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    // Create an AES instance for encrypting the audio file
                    using (AesCryptoServiceProvider encryptor = new AesCryptoServiceProvider())
                    {
                        // Create a password and salt from the key 
                        encryptor.Key = new Rfc2898DeriveBytes(key, 16).GetBytes(32);

                        // Create a destination file where the encrypted bytes will be stored
                        using (var destinationFile = new FileStream($"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}.laf", FileMode.Create, FileAccess.Write))
                        {
                            Console.WriteLine($"({$"{Path.GetFileNameWithoutExtension(path)}.laf"}) : Encrypting the audio file...");
                            // Create crypto streams for reading and writing
                            using (var cryptoStream = new CryptoStream(destinationFile, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                sourceFile.CopyTo(cryptoStream);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }

        // DecryptAndUncompressAudioFile method
        public static void DecryptAudioFile(string path, string key)
        {
            try
            {
                // Create an AES instance for decrypting the audio file
                using (AesCryptoServiceProvider decryptor = new AesCryptoServiceProvider())
                {
                    // Create a password and salt from the key 
                    decryptor.Key = new Rfc2898DeriveBytes(key, 16).GetBytes(32);

                    // Create a source file where the encrypted bytes are stored
                    using (var sourceFile = new FileStream(path, FileMode.Open, FileAccess.Read))
                    {
                        // Create destination file where the decrypted bytes will be stored
                        using (var destinationFile = new FileStream($"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}{Path.GetExtension(path).Replace("laf", "mp3")}", FileMode.CreateNew, FileAccess.Write))
                        {
                            Console.WriteLine($"({$"{Path.GetFileNameWithoutExtension(path)}.laf"}) : Decrypting the audio file...");

                            // Create crypto streams for reading and writing
                            using (var cryptoStream = new CryptoStream(sourceFile, decryptor.CreateDecryptor(), CryptoStreamMode.Read))
                            {
                                cryptoStream.CopyTo(destinationFile);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ShowError(e.Message);
            }
        }
    }
}
