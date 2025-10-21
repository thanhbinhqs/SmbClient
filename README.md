# SmbClient

A Windows Forms application for browsing and managing files on SMB/CIFS network shares with advanced filtering and search capabilities.

## Overview

SmbClient is a user-friendly Windows application that provides a graphical interface for connecting to SMB (Server Message Block) network shares and managing files remotely. Built with .NET Framework 4.7.2 and the SMBLibrary, it offers comprehensive file operations with advanced filtering options.

## Features

### Connection Management
- Connect to SMB/CIFS network shares using IP address or hostname
- Support for domain and local authentication
- Secure credential handling
- Connection status monitoring

### File Operations
- **Browse**: Navigate through directories and view file listings
- **Read/Write**: Transfer files to and from the SMB share
- **Copy/Move**: Relocate files and directories
- **Delete**: Remove files and directories (including recursive deletion)
- **Create**: Create new directories with recursive path creation
- **Rename**: Rename files and folders

### Advanced Filtering
- **Recursive Browsing**: List files in subdirectories with configurable depth
- **Name Patterns**: Filter by filename using wildcards (*, ?)
- **File Extensions**: Filter by specific file types (.txt, .pdf, etc.)
- **Size Filters**: Filter by minimum and maximum file size
- **Date Filters**: Filter by modification date
- **Type Filters**: Show only files or only directories
- **Custom Sorting**: Sort by name, size, creation time, modification time, or type
- **Ascending/Descending**: Configurable sort order

### User Interface
- Intuitive TreeView display of files and folders
- File/folder icons for easy identification
- File size formatting (B, KB, MB, GB, TB)
- Last modification timestamp display
- Real-time file count statistics
- Progress indicators for long operations

## Requirements

- Windows Operating System
- .NET Framework 4.7.2 or higher
- Network access to SMB shares
- Valid credentials for the target SMB server

## Installation

1. Clone the repository:
   ```bash
   git clone https://github.com/thanhbinhqs/SmbClient.git
   ```

2. Open the solution in Visual Studio:
   ```
   SmbClient.sln
   ```

3. Restore NuGet packages (SMBLibrary should be automatically restored)

4. Build the solution:
   - Press `F6` or go to `Build > Build Solution`

5. Run the application:
   - Press `F5` or go to `Debug > Start Debugging`

## Usage

### Connecting to an SMB Share

1. Launch the application
2. Enter the connection details:
   - **Server**: IP address or hostname (e.g., `192.168.1.100` or `fileserver.local`)
   - **Share Name**: Name of the shared folder (e.g., `SharedDocs`, `Public`)
   - **Username**: Your username for authentication
   - **Password**: Your password
   - **Domain**: Domain name (leave empty for local accounts)
3. Click **Connect**
4. Wait for the connection confirmation

### Browsing Files

1. After connecting, enter a path in the **Path** field:
   - Leave empty to browse the root of the share
   - Enter a relative path (e.g., `Documents/Reports`)
2. Click **Browse** to load the directory contents

### Using Filters

#### Basic Filters
- **Files Only**: Show only files, hide directories
- **Directories Only**: Show only directories, hide files
- **Name Pattern**: Enter a wildcard pattern (e.g., `*.txt`, `report_*.pdf`)
- **Extensions**: Enter comma-separated extensions (e.g., `txt, pdf, docx`)

#### Size Filters
- **Min Size**: Minimum file size in bytes
- **Max Size**: Maximum file size in bytes (0 for no limit)

#### Date Filters
- Check **Modified After** and select a date to show only files modified after that date

#### Recursive Options
- **Recursive**: Enable to search through subdirectories
- **Max Depth**: Set maximum folder depth (0 for unlimited)

#### Sorting
- **Sort By**: Choose sorting criteria (Name, Size, Creation Time, Modified Time, Type)
- **Descending**: Check to reverse sort order

### Clearing Filters

Click **Clear Filter** to reset all filter options and reload the current directory.

### Disconnecting

Click **Disconnect** to safely close the connection to the SMB server.

## Code Examples

### Using the Smb Class Programmatically

```csharp
using (var smb = new Smb())
{
    // Connect to server
    bool connected = await smb.ConnectAsync(
        "192.168.1.100",
        "SharedDocs",
        "username",
        "password",
        "DOMAIN"
    );

    if (connected)
    {
   // List all files in root
    var files = await smb.ListFilesAsync("");
        
        // Read a file
        byte[] data = await smb.ReadFileAsync("Documents/report.pdf");
    
        // Write a file
        byte[] content = Encoding.UTF8.GetBytes("Hello, SMB!");
   await smb.WriteFileAsync("greeting.txt", content);
        
    // Create directory
        await smb.CreateDirectoryAsync("NewFolder");
      
        // List files with filters
        var options = SmbListOptions.ByExtension(".pdf", ".docx");
        options.Recursive = true;
   options.MinSize = 1024 * 1024; // 1 MB minimum
        var filteredFiles = await smb.ListFilesAsync("Documents", options);
        
        // Search for files
      var txtFiles = await smb.SearchFilesAsync("", "*.txt", recursive: true);
      
      // Get directory size
        long size = await smb.GetDirectorySizeAsync("Documents", recursive: true);
      
 // Copy file
        await smb.CopyFileAsync("source.pdf", "backup/source_copy.pdf");
 
        // Delete file
      await smb.DeleteFileAsync("old_file.txt");
    }
}
```

### Advanced Filtering Examples

```csharp
// Find all PDF files larger than 5MB modified in the last week
var options = new SmbListOptions
{
    Extensions = new List<string> { ".pdf" },
    MinSize = 5 * 1024 * 1024,
ModifiedAfterDate = DateTime.Now.AddDays(-7),
    Recursive = true,
    SortBy = SmbSortBy.ModifiedTime,
    SortDescending = true
};
var recentLargePdfs = await smb.ListFilesAsync("Documents", options);

// Find all backup files using a custom filter
var backupOptions = new SmbListOptions
{
    Recursive = true,
    CustomFilter = file => file.Name.Contains("backup") && file.Size > 1024 * 1024
};
var backups = await smb.ListFilesAsync("", backupOptions);
```

## Project Structure

```
SmbClient/
??? Form1.cs  # Main Windows Forms UI
??? Form1.Designer.cs   # UI designer code
??? Smb.cs           # Core SMB client library wrapper
??? Program.cs      # Application entry point
??? Properties/           # Assembly info and resources
??? SmbClient.csproj   # Project file
```

## Dependencies

- **SMBLibrary**: Core SMB protocol implementation
- **.NET Framework 4.7.2**: Runtime framework

## API Reference

### Smb Class

#### Connection Methods
- `Connect(server, shareName, username, password, domain)` - Connect to SMB server
- `ConnectAsync(...)` - Async version of Connect
- `Disconnect()` - Disconnect from server

#### File Operations
- `ReadFile(path)` - Read file contents
- `ReadFileAsync(path)` - Async file read
- `WriteFile(path, data)` - Write file
- `WriteFileAsync(path, data)` - Async file write
- `DeleteFile(path)` - Delete file
- `CopyFile(sourcePath, destinationPath)` - Copy file
- `MoveFile(sourcePath, destinationPath)` - Move/rename file
- `RenameFile(path, newName)` - Rename file

#### Directory Operations
- `ListFiles(path)` - List directory contents
- `ListFiles(path, options)` - List with filters
- `ListFilesAsync(...)` - Async versions
- `ListFilesRecursive(path)` - Recursive listing
- `CreateDirectory(path)` - Create directory
- `CreateDirectoryRecursive(path)` - Create with parent directories
- `DeleteDirectory(path)` - Delete empty directory
- `DeleteDirectoryRecursive(path)` - Delete directory and contents
- `CopyDirectoryRecursive(sourcePath, destinationPath)` - Copy directory tree
- `MoveDirectory(sourcePath, destinationPath)` - Move/rename directory

#### Information Methods
- `Exists(path)` - Check if path exists
- `GetFileInfo(path)` - Get detailed file information
- `GetFileSize(path)` - Get file size
- `IsDirectory(path)` - Check if path is directory
- `GetDirectorySize(path, recursive)` - Calculate total directory size
- `GetFileCount(path, recursive)` - Count files in directory
- `GetDirectoryCount(path, recursive)` - Count subdirectories
- `SearchFiles(path, searchPattern, recursive)` - Search by pattern

### SmbFileInfo Class

Properties:
- `Name` - File/folder name
- `FullPath` - Full relative path
- `IsDirectory` - True if directory
- `Size` - File size in bytes
- `CreationTime` - Creation timestamp
- `LastWriteTime` - Last modification timestamp
- `LastAccessTime` - Last access timestamp
- `FileAttributes` - SMB file attributes

### SmbListOptions Class

Advanced filtering options:
- `Recursive` - Enable recursive search
- `MaxDepth` - Maximum folder depth
- `IncludeFilesOnly` - Show only files
- `IncludeDirectoriesOnly` - Show only directories
- `NamePattern` - Wildcard pattern for names
- `Extensions` - List of file extensions to include
- `MinSize` / `MaxSize` - Size range filters
- `CreatedAfter` / `CreatedBefore` - Creation date filters
- `ModifiedAfterDate` / `ModifiedBeforeDate` - Modification date filters
- `SortBy` - Sorting criteria
- `SortDescending` - Sort direction
- `MaxResults` - Limit number of results
- `CustomFilter` - Custom filter function

Factory methods:
- `Default()` - Default options
- `RecursiveAll(maxDepth)` - Recursive with optional depth limit
- `FilesOnly(recursive)` - Files only filter
- `DirectoriesOnly(recursive)` - Directories only filter
- `ByExtension(extensions...)` - Filter by extensions
- `ByPattern(pattern, recursive)` - Filter by name pattern
- `LargerThan(minSizeBytes, recursive)` - Minimum size filter
- `SmallerThan(maxSizeBytes, recursive)` - Maximum size filter
- `ModifiedAfter(date, recursive)` - Modified after date filter
- `SortedBy(sortBy, descending)` - Sorted results

## Error Handling

The `Smb` class provides error information through:
- `LastError` - Human-readable error message
- `LastNTStatus` - SMB NT Status code from last operation

Example:
```csharp
bool success = smb.Connect(...);
if (!success)
{
    Console.WriteLine($"Error: {smb.LastError}");
  Console.WriteLine($"Status: {smb.LastNTStatus}");
}
```

## Troubleshooting

### Connection Issues

**Problem**: Cannot connect to server
- Verify the server IP/hostname is correct
- Check network connectivity (`ping` the server)
- Ensure SMB ports are not blocked by firewall (TCP 445, 139)
- Verify the share name exists on the server
- Check username and password are correct
- For domain accounts, include the correct domain name

**Problem**: "Access Denied" errors
- Verify you have proper permissions on the share
- Check if the account is locked or expired
- Ensure the share has appropriate access control settings

### File Operation Issues

**Problem**: Cannot read/write files
- Check if you have sufficient permissions on specific files
- Verify the file is not locked by another user/process
- Ensure the file path is correct (case-sensitive on some systems)

**Problem**: "Path not found" errors
- Double-check the path syntax (use forward slashes or backslashes consistently)
- Ensure parent directories exist before creating files
- Use `CreateDirectoryRecursive` for deep paths

### Performance Issues

**Problem**: Slow recursive listing
- Limit recursion depth with `MaxDepth` option
- Use filters to reduce the number of results
- Consider network latency and share performance

## Security Considerations

- **Passwords**: Store passwords securely; avoid hardcoding credentials
- **Authentication**: Use domain authentication when available
- **Network**: Use SMB over encrypted channels when possible
- **Permissions**: Follow principle of least privilege
- **Validation**: Always validate user input for paths and filenames

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/YourFeature`)
3. Commit your changes (`git commit -m 'Add some feature'`)
4. Push to the branch (`git push origin feature/YourFeature`)
5. Open a Pull Request

## License

This project is open source. Please check the repository for specific license information.

## Acknowledgments

- Built with [SMBLibrary](https://github.com/TalAloni/SMBLibrary) by Tal Aloni
- Inspired by the need for a simple, user-friendly SMB client

## Support

For issues, questions, or suggestions:
- Open an issue on GitHub: https://github.com/thanhbinhqs/SmbClient/issues
- Check existing issues for known problems and solutions

## Version History

### Current Version
- Full-featured SMB client with GUI
- Advanced filtering and search capabilities
- Recursive directory operations
- Comprehensive file management functions

## Future Enhancements

Potential features for future releases:
- Drag-and-drop file uploads
- Multi-file selection and batch operations
- File preview functionality
- Bookmark/favorite shares
- Transfer progress indicators
- Parallel file transfers
- Configuration file for saved connections
- Support for SMB3 encryption

## Screenshots

*Note: Add screenshots of the application here*

1. Connection screen
2. File browser with TreeView
3. Advanced filter panel
4. File listing results

---

**Made with ?? for the SMB community**
