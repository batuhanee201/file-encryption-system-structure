# 🔐 Secure Folder Encryption System

A powerful and user-friendly folder encryption system built with .NET that provides military-grade security with a magical touch! ✨

![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple.svg)
![Security](https://img.shields.io/badge/security-AES256-green.svg)

## ✨ Features

- 🗂️ **Folder Encryption**: Securely encrypt entire folders and their contents
- 🔑 **Secure Key Management**: Advanced password handling using SecureString
- 🎭 **Hidden Storage**: Folders become completely invisible to unauthorized users
- 🛡️ **Military-Grade Security**: Uses AES-256 encryption with secure random IVs
- 🧹 **Secure Cleanup**: Implements secure deletion with data overwriting
- 📝 **Encrypted Database**: Keeps track of encrypted folders securely
- 🔍 **Key Verification**: Prevents unauthorized access attempts
- 🪄 **User-Friendly Interface**: Simple and intuitive command-line interface

## 🚀 Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Windows Operating System

### Installation

1. Clone the repository:
```bash
git clone https://github.com/hardchenry/file-encryption-system-structure.git
```

2. Navigate to the project directory:
```bash
cd file-encryption-system-structure
```

3. Build the project:
```bash
dotnet build
```

4. Run the application:
```bash
dotnet run
```

## 🎯 Usage

The system provides four main operations:

### 1. 📁 Encrypt and Hide Folder
- Select option 1 from the main menu
- Enter the path of the folder you want to encrypt
- Provide a strong encryption key
- The folder will be encrypted and hidden from view

### 2. 📋 Show Encrypted Folders
- Select option 2 from the main menu
- View a list of all encrypted folders

### 3. 🔓 Access Encrypted Folder
- Select option 3 from the main menu
- Choose the folder you want to access
- Enter the correct encryption key
- Access your files temporarily
- Files are automatically re-encrypted when you're done

### 4. 👋 Exit
- Safely exit the application

## 🔒 Security Features

1. **Advanced Encryption**
   - AES-256 encryption
   - Unique random IV for each file
   - Secure key verification system

2. **Memory Protection**
   - Secure string handling for passwords
   - Immediate memory cleanup
   - Protected key storage

3. **File Security**
   - Secure file deletion
   - Hidden folder attributes
   - Encrypted folder tracking

## ⚡ Technical Details

The system employs several security measures:

```csharp
// Encryption using AES-256
using (Aes aes = Aes.Create())
{
    // Unique IV for each encryption
    using (var rng = new RNGCryptoServiceProvider())
    {
        rng.GetBytes(iv);
    }
    // Secure encryption process
    using (CryptoStream cs = new CryptoStream(...))
    {
        // Encryption magic happens here ✨
    }
}
```

## 🤝 Contributing

Contributions are welcome! Here's how you can help:

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Open a Pull Request

## 📜 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🌟 Acknowledgments

- Built with love using .NET
- Inspired by the need for simple yet secure file encryption
- Thanks to all contributors and users!

## 🔐 Security Notice

Always remember:
- Keep your encryption keys safe
- Use strong, unique keys for each folder
- Never share your encryption keys
- Regularly backup your data

## 📞 Contact

For questions or suggestions, please open an issue in the GitHub repository.

---

