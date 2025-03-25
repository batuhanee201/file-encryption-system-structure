using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Collections.Generic;
using System.Security;
using System.Runtime.InteropServices;

// 🔐 Welcome to the Super Secret Folder Hider! 
// Where folders go to play hide and seek with hackers 🕵️‍♂️
class Program
{
    // Our little black book of secret folders 📒
    private static Dictionary<string, string> encryptedFolders = new Dictionary<string, string>();
    // The key to our kingdom (don't tell anyone!) 🗝️
    private static readonly byte[] DatabaseKey = new byte[32]; 
    // Where we keep track of our hidden treasures 🗺️
    private static string databaseFile = "encrypted_folders.db";

    // Constructor: The first thing we do when we wake up 🌅
    static Program()
    {
        // Generate a random key like picking a random card from a deck 🎴
        using (var rng = new RNGCryptoServiceProvider())
        {
            rng.GetBytes(DatabaseKey);
        }
    }

    // Converts a secure string into bytes (like turning words into secret code) 🔄
    private static byte[] SecureStringToBytes(SecureString secureString)
    {
        // Warning: Here be dragons! Handle with care! 🐉
        IntPtr unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(secureString);
            byte[] bytes = new byte[secureString.Length * 2];
            Marshal.Copy(unmanagedString, bytes, 0, bytes.Length);
            return bytes;
        }
        finally
        {
            // Clean up after ourselves like good ninjas 🥷
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }
    }

    // The main menu - where all the magic begins! ✨
    static void Main(string[] args)
    {
        LoadEncryptedFolders();

        while (true)
        {
            Console.Clear(); // Clear the screen like a ninja smoke bomb 💨
            Console.WriteLine("🔒 Secure Folder Encryption System 🔒");
            Console.WriteLine("1. 📁 Encrypt and Hide Folder");
            Console.WriteLine("2. 📋 Show Encrypted Folders");
            Console.WriteLine("3. 🔓 Access Encrypted Folder");
            Console.WriteLine("4. 👋 Exit");
            Console.Write("\nEnter your choice (1-4): ");

            string choice = Console.ReadLine();
            // Let the user pick their destiny!
            switch (choice)
            {
                case "1":
                    EncryptFolder();
                    break;
                case "2":
                    ListEncryptedFolders();
                    break;
                case "3":
                    AccessEncryptedFolder();
                    break;
                case "4":
                    return;
            }

            Console.WriteLine("\nPress any key to continue the adventure...");
            Console.ReadKey();
        }
    }

    // Time to make a folder disappear! 🎩✨
    static void EncryptFolder()
    {
        // First, we need a folder to encrypt (can't make an omelet without eggs! 🍳)
        Console.Write("\nEnter folder path to encrypt: ");
        string path = Console.ReadLine()?.Trim();

        if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
        {
            Console.WriteLine("Oops! That folder is playing hide and seek! 🙈");
            return;
        }

        // Get the secret password (shhh... don't tell anyone!)
        using (SecureString secureKey = new SecureString())
        {
            Console.Write("Enter your super secret key: ");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (secureKey.Length > 0)
                        secureKey.RemoveAt(secureKey.Length - 1);
                }
                else
                {
                    secureKey.AppendChar(key.KeyChar);
                    Console.Write("*"); // Hide the password like a magician's trick! 🎭
                }
            }
            Console.WriteLine();

            try
            {
                // Create a unique name for our hidden folder (like a secret identity! 🦸‍♂️)
                string encryptedFolderName = Path.Combine(
                    Path.GetDirectoryName(path),
                    "." + Path.GetFileName(path) + "_" + Guid.NewGuid().ToString("N") + "_encrypted"
                );

                // Make the folder invisible (like Harry Potter's cloak! 🧙‍♂️)
                Directory.CreateDirectory(encryptedFolderName);
                File.SetAttributes(encryptedFolderName, FileAttributes.Hidden | FileAttributes.System);

                // Create a special key verifier (like a secret handshake! 🤝)
                byte[] keyVerification = new byte[32];
                using (var rng = new RNGCryptoServiceProvider())
                {
                    rng.GetBytes(keyVerification);
                }
                
                string verificationFile = Path.Combine(encryptedFolderName, ".keyverify");
                File.WriteAllBytes(verificationFile, EncryptBytes(keyVerification, SecureStringToBytes(secureKey), out _));

                // Time to encrypt all the files! (turning them into secret messages 📜)
                foreach (string filePath in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
                {
                    string relativePath = filePath.Substring(path.Length + 1);
                    string targetPath = Path.Combine(encryptedFolderName, relativePath);
                    
                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));
                    
                    byte[] content = File.ReadAllBytes(filePath);
                    byte[] iv;
                    byte[] encrypted = EncryptBytes(content, SecureStringToBytes(secureKey), out iv);
                    
                    // Store the IV and encrypted data (like putting a letter in a secret envelope 📨)
                    using (var fs = new FileStream(targetPath, FileMode.Create))
                    {
                        fs.Write(iv, 0, iv.Length);
                        fs.Write(encrypted, 0, encrypted.Length);
                    }
                }

                // Remember where we hid our treasure! 🗺️
                encryptedFolders[encryptedFolderName] = Path.GetFileName(path);
                SaveEncryptedFolders();

                // Make the original folder vanish! 🪄
                Directory.Delete(path, true);
                Console.WriteLine("Folder has been hidden from prying eyes! 🎉");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Oops! Something went wrong! 😅 Error: {ex.Message}");
            }
        }
    }

    // The super-secret encryption spell! 🧙‍♂️
    static byte[] EncryptBytes(byte[] data, byte[] key, out byte[] iv)
    {
        using (Aes aes = Aes.Create()) // AES: The strongest lock in our magical toolbox! 🔒
        {
            aes.Key = key;
            // Generate a random IV (like a unique magic wand for each spell! ⚡)
            iv = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(iv);
            }
            aes.IV = iv;

            // Time to transform our data into secret code! 🎭
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray(); // Our data is now in disguise! 🎭
            }
        }
    }

    // The magical decryption spell! 🔮
    static byte[] DecryptBytes(byte[] data, byte[] key, byte[] iv)
    {
        using (Aes aes = Aes.Create()) // Time to unlock the secrets! 🗝️
        {
            aes.Key = key;
            aes.IV = iv;

            // Transform our secret code back into normal data! ✨
            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                    cs.FlushFinalBlock();
                }
                return ms.ToArray(); // Ta-da! The secret message is revealed! 🎉
            }
        }
    }

    static void ListEncryptedFolders()
    {
        Console.WriteLine("\nEncrypted Folders:");
        if (encryptedFolders.Count == 0)
        {
            Console.WriteLine("No encrypted folders found.");
            return;
        }

        foreach (var folder in encryptedFolders)
        {
            Console.WriteLine($"- {folder.Value} (Hidden at: {folder.Key})");
        }
    }

    static void AccessEncryptedFolder()
    {
        ListEncryptedFolders();
        if (encryptedFolders.Count == 0) return;

        Console.Write("\nEnter original folder name to access: ");
        string folderName = Console.ReadLine()?.Trim();

        var encryptedFolder = encryptedFolders.FirstOrDefault(f => f.Value == folderName);
        if (string.IsNullOrEmpty(encryptedFolder.Key))
        {
            Console.WriteLine("Folder not found!");
            return;
        }

        using (SecureString secureKey = new SecureString())
        {
            Console.Write("Enter encryption key: ");
            while (true)
            {
                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (secureKey.Length > 0)
                        secureKey.RemoveAt(secureKey.Length - 1);
                }
                else
                {
                    secureKey.AppendChar(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();

            try
            {
                // Verify key first
                string verificationFile = Path.Combine(encryptedFolder.Key, ".keyverify");
                if (!VerifyKey(verificationFile, secureKey))
                {
                    Console.WriteLine("Invalid encryption key!");
                    return;
                }

                string tempViewFolder = Path.Combine(
                    Path.GetDirectoryName(encryptedFolder.Key),
                    Path.GetFileName(encryptedFolder.Key) + "_temp"
                );

                Directory.CreateDirectory(tempViewFolder);

                foreach (string filePath in Directory.GetFiles(encryptedFolder.Key, "*.*", SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(filePath) == ".keyverify") continue;

                    string relativePath = filePath.Substring(encryptedFolder.Key.Length + 1);
                    string targetPath = Path.Combine(tempViewFolder, relativePath);

                    Directory.CreateDirectory(Path.GetDirectoryName(targetPath));

                    // Read IV and encrypted content
                    byte[] fileContent = File.ReadAllBytes(filePath);
                    byte[] iv = new byte[16];
                    byte[] encrypted = new byte[fileContent.Length - 16];

                    Array.Copy(fileContent, iv, 16);
                    Array.Copy(fileContent, 16, encrypted, 0, encrypted.Length);

                    byte[] decrypted = DecryptBytes(encrypted, SecureStringToBytes(secureKey), iv);
                    File.WriteAllBytes(targetPath, decrypted);
                }

                Console.WriteLine($"\nFiles decrypted temporarily at: {tempViewFolder}");
                Console.WriteLine("WARNING: Files will be re-encrypted when you press any key...");
                Console.ReadKey();

                // Securely delete temporary files by overwriting with random data
                foreach (string filePath in Directory.GetFiles(tempViewFolder, "*.*", SearchOption.AllDirectories))
                {
                    SecureDeleteFile(filePath);
                }
                Directory.Delete(tempViewFolder, true);
                Console.WriteLine("Files re-encrypted and temporary folder removed securely.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}. Incorrect key?");
            }
        }
    }

    static bool VerifyKey(string verificationFile, SecureString key)
    {
        try
        {
            byte[] fileContent = File.ReadAllBytes(verificationFile);
            byte[] iv = new byte[16];
            byte[] encrypted = new byte[fileContent.Length - 16];

            Array.Copy(fileContent, iv, 16);
            Array.Copy(fileContent, 16, encrypted, 0, encrypted.Length);

            DecryptBytes(encrypted, SecureStringToBytes(key), iv);
            return true;
        }
        catch
        {
            return false;
        }
    }

    static void SecureDeleteFile(string filePath)
    {
        byte[] buffer = new byte[4096];
        long length = new FileInfo(filePath).Length;
        using (var rng = new RNGCryptoServiceProvider())
        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Write))
        {
            for (long i = 0; i < length; i += buffer.Length)
            {
                rng.GetBytes(buffer);
                fs.Write(buffer, 0, (int)Math.Min(buffer.Length, length - i));
            }
        }
        File.Delete(filePath);
    }

    static void LoadEncryptedFolders()
    {
        try
        {
            if (File.Exists(databaseFile))
            {
                byte[] encrypted = File.ReadAllBytes(databaseFile);
                if (encrypted.Length > 16)
                {
                    byte[] iv = new byte[16];
                    byte[] data = new byte[encrypted.Length - 16];
                    Array.Copy(encrypted, iv, 16);
                    Array.Copy(encrypted, 16, data, 0, data.Length);

                    byte[] decrypted = DecryptBytes(data, DatabaseKey, iv);
                    string content = Encoding.UTF8.GetString(decrypted);
                    
                    foreach (string line in content.Split('\n'))
                    {
                        if (string.IsNullOrWhiteSpace(line)) continue;
                        string[] parts = line.Trim().Split('|');
                        if (parts.Length == 2)
                        {
                            encryptedFolders[parts[0]] = parts[1];
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading database: {ex.Message}");
        }
    }

    static void SaveEncryptedFolders()
    {
        try
        {
            StringBuilder content = new StringBuilder();
            foreach (var folder in encryptedFolders)
            {
                content.AppendLine($"{folder.Key}|{folder.Value}");
            }

            byte[] data = Encoding.UTF8.GetBytes(content.ToString());
            byte[] iv;
            byte[] encrypted = EncryptBytes(data, DatabaseKey, out iv);

            using (var fs = new FileStream(databaseFile, FileMode.Create))
            {
                fs.Write(iv, 0, iv.Length);
                fs.Write(encrypted, 0, encrypted.Length);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving database: {ex.Message}");
        }
    }
}
