# SMB Client Library - Complete Usage Guide

## Table of Contents
- [Installation](#installation)
- [Connection](#connection)
- [File Operations](#file-operations)
- [Directory Operations](#directory-operations)
- [Advanced Listing with Filters](#advanced-listing-with-filters)
- [Error Handling](#error-handling)
- [Best Practices](#best-practices)

---

## Installation

Install the required NuGet package:
```
Install-Package SMBLibrary
```

---

## Connection

### Connect with Domain
```csharp
using (var smb = new Smb())
{
    bool connected = smb.Connect(
      server: "192.168.1.100",      // Server IP or hostname
        shareName: "SharedDocuments",  // Share name
      username: "john",
        password: "password123",
   domain: "COMPANY"              // Domain name
    );
    
    if (connected)
    {
        Console.WriteLine($"Connected to {smb.ServerName}\\{smb.ShareName}");
    }
    else
    {
        Console.WriteLine($"Error: {smb.LastError}");
        Console.WriteLine($"NT Status: {smb.LastNTStatus}");
    }
}
```

### Connect without Domain (Local Account)
```csharp
bool connected = smb.Connect("192.168.1.100", "Public", "guest", "guest");
```

### Connect Asynchronously
```csharp
bool connected = await smb.ConnectAsync("server.local", "Files", "user", "pass", "DOMAIN");
```

---

## File Operations

### Check if File/Folder Exists
```csharp
// Check file
if (smb.Exists("Documents/report.pdf"))
{
    Console.WriteLine("File exists!");
}

// Check folder
if (smb.Exists("BackupFolder"))
{
    Console.WriteLine("Folder exists!");
}

// Async version
bool exists = await smb.ExistsAsync("Documents/report.pdf");
```

### Read File
```csharp
// Read binary file
byte[] data = smb.ReadFile("backup/archive.zip");
File.WriteAllBytes(@"C:\local\archive.zip", data);

// Read text file
byte[] textData = smb.ReadFile("logs/app.log");
string content = Encoding.UTF8.GetString(textData);
Console.WriteLine(content);

// Async version
byte[] data = await smb.ReadFileAsync("data.bin");
```

### Write File
```csharp
// Write text file
string text = "Hello World!";
byte[] data = Encoding.UTF8.GetBytes(text);
bool success = smb.WriteFile("test.txt", data);

// Upload local file
byte[] fileData = File.ReadAllBytes(@"C:\local\document.pdf");
smb.WriteFile("uploads/document.pdf", fileData);

// Async version
await smb.WriteFileAsync("data.txt", data);
```

### Copy File
```csharp
// Copy within share
smb.CopyFile("Documents/original.docx", "Backup/copy.docx");

// Create backup with timestamp
string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
smb.CopyFile("data.db", $"backups/data_{timestamp}.db");

// Async
await smb.CopyFileAsync("source.txt", "destination.txt");
```

### Delete File
```csharp
if (smb.DeleteFile("temp/old_file.tmp"))
{
    Console.WriteLine("File deleted");
}

// Async
await smb.DeleteFileAsync("obsolete.dat");
```

### Move/Rename File
```csharp
// Move to different folder
smb.MoveFile("Inbox/document.pdf", "Archive/document.pdf");

// Rename in same folder
smb.RenameFile("Documents/old_name.txt", "new_name.txt");

// Async versions
await smb.MoveFileAsync("from.txt", "to.txt");
await smb.RenameFileAsync("Documents/file.txt", "renamed.txt");
```

### Get File Information
```csharp
SmbFileInfo info = smb.GetFileInfo("Documents/report.pdf");
if (info != null)
{
    Console.WriteLine($"Name: {info.Name}");
    Console.WriteLine($"Size: {info.Size:N0} bytes");
    Console.WriteLine($"Created: {info.CreationTime}");
    Console.WriteLine($"Modified: {info.LastWriteTime}");
    Console.WriteLine($"Is Directory: {info.IsDirectory}");
  Console.WriteLine($"Attributes: {info.FileAttributes}");
}

// Get just file size
long size = smb.GetFileSize("large_file.zip");
Console.WriteLine($"Size: {size / 1024 / 1024} MB");

// Check if path is directory
bool isDir = smb.IsDirectory("FolderName");
```

---

## Directory Operations

### List Files in Directory
```csharp
var files = smb.ListFiles("Documents");
foreach (var file in files)
{
    string type = file.IsDirectory ? "DIR " : "FILE";
    Console.WriteLine($"{type} - {file.Name} - {file.Size:N0} bytes");
    Console.WriteLine($"      Modified: {file.LastWriteTime}");
}

// List root directory
var rootFiles = smb.ListFiles(""); // or "/"

// Async
var files = await smb.ListFilesAsync("Documents");
```

### Create Directory
```csharp
// Simple create
if (smb.CreateDirectory("NewFolder"))
{
    Console.WriteLine("Directory created");
}

// Create nested directories (creates parent folders if needed)
smb.CreateDirectoryRecursive("Reports/2024/January");

// Async
await smb.CreateDirectoryAsync("TestFolder");
await smb.CreateDirectoryRecursiveAsync("A/B/C/D");
```

### Delete Directory
```csharp
// Delete empty directory
smb.DeleteDirectory("EmptyFolder");

// Delete directory with all contents (recursive)
if (smb.DeleteDirectoryRecursive("OldProjects"))
{
    Console.WriteLine("Directory and all contents deleted");
}

// Async
await smb.DeleteDirectoryAsync("TempFolder");
await smb.DeleteDirectoryRecursiveAsync("Obsolete");
```

### Move/Rename Directory
```csharp
// Move directory
smb.MoveDirectory("OldLocation", "Archive/OldLocation");

// Async
await smb.MoveDirectoryAsync("Temp", "Backup/Temp");
```

### Copy Directory Recursively
```csharp
// Copy entire directory tree
if (smb.CopyDirectoryRecursive("Projects", "Backup/Projects"))
{
    Console.WriteLine("Directory copied successfully");
}

// Async
await smb.CopyDirectoryRecursiveAsync("Source", "Destination");
```

### Directory Statistics
```csharp
// Count files
int fileCount = smb.GetFileCount("Documents", recursive: false);
int allFiles = smb.GetFileCount("Documents", recursive: true);
Console.WriteLine($"Files: {fileCount}, Total with subfolders: {allFiles}");

// Count directories
int dirCount = smb.GetDirectoryCount("Projects", recursive: true);

// Get total size
long totalSize = smb.GetDirectorySize("Videos", recursive: true);
Console.WriteLine($"Total size: {totalSize / 1024 / 1024} MB");

// Async versions
int count = await smb.GetFileCountAsync("Documents", true);
long size = await smb.GetDirectorySizeAsync("Videos", true);
```

### Search Files
```csharp
// Search for files matching pattern
var logFiles = smb.SearchFiles("Logs", "*.log", recursive: true);
var reports = smb.SearchFiles("Reports", "report_202?_*.pdf", recursive: false);

foreach (var file in logFiles)
{
    Console.WriteLine($"Found: {file.FullPath}");
}

// Async
var results = await smb.SearchFilesAsync("", "*.txt", true);
```

---

## Advanced Listing with Filters

### Basic Filters

#### Files Only
```csharp
var files = smb.ListFiles("Documents", SmbListOptions.FilesOnly(recursive: true));
```

#### Directories Only
```csharp
var folders = smb.ListFiles("Projects", SmbListOptions.DirectoriesOnly());
```

#### By Extension
```csharp
// Single extension
var images = smb.ListFiles("Photos", SmbListOptions.ByExtension(".jpg", ".png", ".gif"));

// Set recursive
var options = SmbListOptions.ByExtension(".pdf");
options.Recursive = true;
var pdfs = smb.ListFiles("", options);
```

#### By Pattern (Wildcards)
```csharp
// All log files
var logs = smb.ListFiles("Logs", SmbListOptions.ByPattern("*.log", recursive: true));

// Specific pattern
var reports = smb.ListFiles("Reports", SmbListOptions.ByPattern("report_2024_*.xlsx"));
```

#### By Size
```csharp
// Files larger than 100MB
var large = smb.ListFiles("Videos", SmbListOptions.LargerThan(100 * 1024 * 1024, recursive: true));

// Files smaller than 1MB
var small = smb.ListFiles("Docs", SmbListOptions.SmallerThan(1024 * 1024));
```

#### By Date
```csharp
// Modified in last 7 days
var recent = smb.ListFiles("Documents", SmbListOptions.ModifiedAfter(DateTime.Now.AddDays(-7)));
```

#### With Sorting
```csharp
// Sort by name ascending
var sorted = smb.ListFiles("Documents", SmbListOptions.SortedBy(SmbSortBy.Name));

// Sort by size descending (largest first)
var largest = smb.ListFiles("Downloads", SmbListOptions.SortedBy(SmbSortBy.Size, descending: true));

// Sort by modified time (newest first)
var newest = smb.ListFiles("Logs", SmbListOptions.SortedBy(SmbSortBy.ModifiedTime, descending: true));
```

### Complex Filters

```csharp
var options = new SmbListOptions
{
    // Recursion settings
 Recursive = true,
    MaxDepth = 5,   // Limit recursion depth
    
    // Type filters
    IncludeFilesOnly = false,
 IncludeDirectoriesOnly = false,
    
    // Name and extension filters
    NamePattern = "report_*.pdf",// Wildcard pattern
    Extensions = new List<string> { ".pdf", ".docx" },
    
    // Size filters (in bytes)
    MinSize = 1024,      // Minimum 1 KB
    MaxSize = 10 * 1024 * 1024,       // Maximum 10 MB
    
    // Date filters
    CreatedAfter = DateTime.Now.AddMonths(-6),
    ModifiedAfterDate = DateTime.Now.AddDays(-30),
    ModifiedBeforeDate = DateTime.Now.AddDays(-1),
    
    // Attribute filters
    RequiredAttributes = SMBLibrary.FileAttributes.Archive,
    ExcludedAttributes = SMBLibrary.FileAttributes.Hidden | SMBLibrary.FileAttributes.System,
    
    // Sorting
    SortBy = SmbSortBy.ModifiedTime,
    SortDescending = true,
    
    // Limit results
    MaxResults = 100     // Return max 100 results
};

var files = smb.ListFiles("Documents", options);
```

### Custom Filter Function

```csharp
var options = new SmbListOptions
{
    Recursive = true,
CustomFilter = file =>
    {
      // Complex custom logic
        if (file.IsDirectory)
            return false; // Skip directories
    
        // Only Word and Excel files
        var extensions = new[] { ".docx", ".xlsx" };
  var ext = Path.GetExtension(file.Name)?.ToLower();
        if (!extensions.Contains(ext))
            return false;
      
      // Modified in last week
        if (file.LastWriteTime < DateTime.Now.AddDays(-7))
            return false;
      
        // Not starting with "temp_"
if (file.Name.StartsWith("temp_", StringComparison.OrdinalIgnoreCase))
return false;
 
        return true;
    }
};

var customFiltered = smb.ListFiles("", options);
```

### Combining Multiple Filters

```csharp
// Find large image files modified recently, sorted by size
var options = SmbListOptions.ByExtension(".jpg", ".png");
options.Recursive = true;
options.MinSize = 5 * 1024 * 1024; // > 5MB
options.ModifiedAfterDate = DateTime.Now.AddMonths(-1);
options.SortBy = SmbSortBy.Size;
options.SortDescending = true;
options.MaxResults = 50;

var largeRecentImages = smb.ListFiles("Photos", options);
```

---

## Error Handling

### Check Last Error
```csharp
var files = smb.ListFiles("NonExistentPath");
if (files.Count == 0 && smb.LastError != null)
{
Console.WriteLine($"Error: {smb.LastError}");
    Console.WriteLine($"NT Status: {smb.LastNTStatus}");
}
```

### Try-Catch Pattern
```csharp
try
{
    byte[] data = smb.ReadFile("Documents/important.pdf");
    File.WriteAllBytes(@"C:\backup\important.pdf", data);
}
catch (IOException ex)
{
  Console.WriteLine($"I/O Error: {ex.Message}");
    Console.WriteLine($"SMB Error: {smb.LastError}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Not connected: {ex.Message}");
}
```

### Using SmbResult for Detailed Errors
```csharp
// For custom operations requiring detailed error info
var result = SmbResult<byte[]>.CreateSuccess(data);
// or
var error = SmbResult<byte[]>.CreateError(
    "Failed to read file",
    NTStatus.STATUS_ACCESS_DENIED,
    exception
);

if (error.Success)
{
    ProcessData(error.Data);
}
else
{
    Console.WriteLine($"Error: {error.ErrorMessage}");
    Console.WriteLine($"Status: {error.NTStatus}");
if (error.Exception != null)
        Console.WriteLine($"Exception: {error.Exception.Message}");
}
```

---

## Best Practices

### 1. Always Use `using` Statement
```csharp
using (var smb = new Smb())
{
    smb.Connect(...);
    // Do work
} // Automatically disconnects
```

### 2. Check Connection Before Operations
```csharp
if (!smb.Connect(...))
{
    Console.WriteLine($"Failed to connect: {smb.LastError}");
    return;
}
```

### 3. Handle Large Files Efficiently
```csharp
// For very large files, consider processing in chunks
// The library handles chunking automatically for read/write
byte[] largeData = smb.ReadFile("huge_file.bin"); // Automatically chunked
```

### 4. Use Async Methods for UI Applications
```csharp
// In WinForms/WPF to avoid blocking UI
var files = await smb.ListFilesAsync("Documents");
progressBar.Value = 100;
```

### 5. Always Check LastError for Failed Operations
```csharp
if (!smb.DeleteFile("file.txt"))
{
    // Check what went wrong
    if (smb.LastNTStatus == NTStatus.STATUS_ACCESS_DENIED)
    {
        Console.WriteLine("Permission denied");
    }
    else
    {
        Console.WriteLine($"Error: {smb.LastError}");
    }
}
```

### 6. Use Filters Instead of Manual Filtering
```csharp
// BAD - Load everything then filter
var allFiles = smb.ListFilesRecursive("Documents");
var pdfs = allFiles.Where(f => f.Name.EndsWith(".pdf")).ToList();

// GOOD - Filter server-side
var pdfs = smb.ListFiles("Documents", SmbListOptions.ByExtension(".pdf"));
pdfs = pdfs.Where(f => /* additional filter */).ToList();
```

### 7. Disconnect Explicitly for Long-Running Apps
```csharp
smb.Connect(...);
// Do work
smb.Disconnect(); // Explicit disconnect
// Later reconnect if needed
smb.Connect(...);
```

### 8. Verify File Exists Before Operations
```csharp
if (smb.Exists("important.dat"))
{
    byte[] data = smb.ReadFile("important.dat");
    // Process data
}
else
{
    Console.WriteLine("File not found");
}
```

---

## Common Scenarios

### Scenario 1: Backup Files to Local Computer
```csharp
using (var smb = new Smb())
{
    if (!smb.Connect("server", "Backups", "user", "pass"))
    return;
        
    var files = smb.ListFiles("Daily", SmbListOptions.FilesOnly(recursive: true));
    foreach (var file in files)
    {
        byte[] data = smb.ReadFile(file.FullPath);
      string localPath = Path.Combine(@"C:\LocalBackup", file.FullPath);
        Directory.CreateDirectory(Path.GetDirectoryName(localPath));
        File.WriteAllBytes(localPath, data);
        Console.WriteLine($"Backed up: {file.FullPath}");
    }
}
```

### Scenario 2: Upload Multiple Files
```csharp
string[] localFiles = Directory.GetFiles(@"C:\ToUpload", "*.*", SearchOption.AllDirectories);
using (var smb = new Smb())
{
    smb.Connect("server", "Uploads", "user", "pass");
    
    foreach (var localFile in localFiles)
    {
        byte[] data = File.ReadAllBytes(localFile);
        string remotePath = Path.GetFileName(localFile);
        if (smb.WriteFile(remotePath, data))
            Console.WriteLine($"Uploaded: {remotePath}");
    }
}
```

### Scenario 3: Clean Old Files
```csharp
using (var smb = new Smb())
{
    smb.Connect("server", "Temp", "user", "pass");
    
    var oldFiles = smb.ListFiles("", SmbListOptions.ModifiedAfter(DateTime.Now.AddDays(-30)));
    oldFiles = oldFiles.Where(f => !f.IsDirectory).ToList();
    
    foreach (var file in oldFiles)
    {
        if (smb.DeleteFile(file.FullPath))
  Console.WriteLine($"Deleted: {file.Name}");
    }
}
```

### Scenario 4: Generate File Report
```csharp
using (var smb = new Smb())
{
    smb.Connect("server", "Documents", "user", "pass");
    
 var files = smb.ListFiles("", SmbListOptions.RecursiveAll());
    
    var report = new StringBuilder();
    report.AppendLine("File Report");
    report.AppendLine($"Total Files: {files.Count(f => !f.IsDirectory)}");
    report.AppendLine($"Total Folders: {files.Count(f => f.IsDirectory)}");
    report.AppendLine($"Total Size: {files.Where(f => !f.IsDirectory).Sum(f => f.Size) / 1024 / 1024} MB");
    
    var largest = files.Where(f => !f.IsDirectory)
        .OrderByDescending(f => f.Size)
          .Take(10);
    report.AppendLine("\nLargest Files:");
    foreach (var file in largest)
    {
        report.AppendLine($"  {file.FullPath} - {file.Size / 1024 / 1024} MB");
    }
    
    Console.WriteLine(report.ToString());
}
```

---

## Enum Reference

### SmbSortBy
- `None` - No sorting
- `Name` - Sort by file/folder name
- `Size` - Sort by file size
- `CreationTime` - Sort by creation date
- `ModifiedTime` - Sort by last modified date
- `Type` - Sort by type (directories first, then files)

---

## Additional Notes

- All paths are relative to the share root
- Use forward slashes `/` or backslashes `\` in paths
- Empty string `""` or `"/"` refers to share root
- File operations automatically handle chunking for large files
- Async methods are thread-safe
- Connection is automatically managed with `using` statement

---

**Version:** 1.0  
**Target Framework:** .NET Framework 4.7.2  
**Required Package:** SMBLibrary (1.5.4.1 or higher)
