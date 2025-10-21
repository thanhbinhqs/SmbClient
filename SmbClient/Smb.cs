using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SMBLibrary;
using SMBLibrary.Client;

namespace SmbClient
{
    public class Smb : IDisposable
    {
        private SMB2Client _client;
        private ISMBFileStore _fileStore;
        private bool _isConnected;

        public string ServerName { get; private set; }
        public string ShareName { get; private set; }

        /// <summary>
        /// Gets the last error message from the most recent operation.
        /// </summary>
        public string LastError { get; private set; }

        /// <summary>
        /// Gets the last NT Status code from the most recent SMB operation.
        /// </summary>
        public NTStatus LastNTStatus { get; private set; }

        /// <summary>
        /// Connects to an SMB server with domain authentication.
        /// </summary>
        /// <param name="server">Server IP address or hostname (e.g., "192.168.1.100" or "fileserver.local")</param>
        /// <param name="shareName">Name of the shared folder (e.g., "Documents", "SharedFiles")</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <param name="domain">Domain name (use empty string or null for local accounts)</param>
        /// <returns>True if connection successful, false otherwise. Check LastError for details.</returns>
        /// <example>
        /// <code>
        /// var smb = new Smb();
        /// if (smb.Connect("192.168.1.100", "SharedDocs", "admin", "password123", "DOMAIN"))
        /// {
        ///     Console.WriteLine($"Connected to {smb.ServerName}");
        /// }
        /// else
        /// {
        ///     Console.WriteLine($"Error: {smb.LastError}");
        /// }
        /// </code>
        /// </example>
        public bool Connect(string server, string shareName, string username, string password, string domain)
        {
            try
            {
                LastError = null;
                LastNTStatus = NTStatus.STATUS_SUCCESS;

                _client = new SMB2Client();
                bool isConnected = _client.Connect(server, SMBTransportType.DirectTCPTransport);

                if (!isConnected)
                {
                    LastError = $"Failed to connect to server '{server}'";
                    return false;
                }

                // Use domain if available, otherwise use empty string
                NTStatus status = _client.Login(domain ?? string.Empty, username, password);
                LastNTStatus = status;

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    LastError = $"Login failed with status: {status}. Check username, password, and domain.";
                    _client.Disconnect();
                    return false;
                }

                _fileStore = _client.TreeConnect(shareName, out status);
                LastNTStatus = status;

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    LastError = $"Failed to connect to share '{shareName}' with status: {status}";
                    _client.Logoff();
                    _client.Disconnect();
                    return false;
                }

                ServerName = server;
                ShareName = shareName;
                _isConnected = true;
                return true;
            }
            catch (Exception ex)
            {
                LastError = $"Connection error: {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Connects to an SMB server without domain (local authentication).
        /// </summary>
        /// <param name="server">Server IP address or hostname</param>
        /// <param name="shareName">Name of the shared folder</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <returns>True if connection successful, false otherwise</returns>
        /// <example>
        /// <code>
        /// var smb = new Smb();
        /// bool connected = smb.Connect("192.168.1.100", "Public", "guest", "guest123");
        /// </code>
        /// </example>
        public bool Connect(string server, string shareName, string username, string password)
        {
            return Connect(server, shareName, username, password, string.Empty);
        }

        // Connect to SMB server asynchronously (with domain)
        public async Task<bool> ConnectAsync(string server, string shareName, string username, string password, string domain)
        {
            return await Task.Run(() => Connect(server, shareName, username, password, domain));
        }

        /// <summary>
        /// Connects to an SMB server asynchronously without domain.
        /// </summary>
        /// <param name="server">Server IP address or hostname</param>
        /// <param name="shareName">Name of the shared folder</param>
        /// <param name="username">Username for authentication</param>
        /// <param name="password">Password for authentication</param>
        /// <returns>Task returning true if successful</returns>
        /// <example>
        /// <code>
        /// var smb = new Smb();
        /// bool connected = await smb.ConnectAsync("server.local", "Files", "user", "pass", "DOMAIN");
        /// </code>
        /// </example>
        public async Task<bool> ConnectAsync(string server, string shareName, string username, string password)
        {
            return await Task.Run(() => Connect(server, shareName, username, password));
        }

        // Check if file/folder exists (synchronous)
        public bool Exists(string path)
        {
            if (!_isConnected)
            {
                LastError = "Not connected to SMB server";
                throw new InvalidOperationException(LastError);
            }

            try
            {
                LastError = null;
                LastNTStatus = NTStatus.STATUS_SUCCESS;

                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
           AccessMask.GENERIC_READ, SMBLibrary.FileAttributes.Normal,
            ShareAccess.Read | ShareAccess.Write,
                      CreateDisposition.FILE_OPEN, CreateOptions.FILE_NON_DIRECTORY_FILE, null);

                if (status == NTStatus.STATUS_SUCCESS)
                {
                    _fileStore.CloseFile(handle);
                    return true;
                }

                // Try checking as directory
                status = _fileStore.CreateFile(out handle, out fileStatus, path,
                      AccessMask.GENERIC_READ, SMBLibrary.FileAttributes.Directory,
             ShareAccess.Read | ShareAccess.Write,
                    CreateDisposition.FILE_OPEN, CreateOptions.FILE_DIRECTORY_FILE, null);

                LastNTStatus = status;

                if (status == NTStatus.STATUS_SUCCESS)
                {
                    _fileStore.CloseFile(handle);
                    return true;
                }

                if (status != NTStatus.STATUS_OBJECT_NAME_NOT_FOUND && status != NTStatus.STATUS_OBJECT_PATH_NOT_FOUND)
                {
                    LastError = $"Error checking path '{path}': {status}";
                }

                return false;
            }
            catch (Exception ex)
            {
                LastError = $"Exception checking existence of '{path}': {ex.Message}";
                return false;
            }
        }

        /// <summary>
        /// Checks if a file or folder exists on the SMB server.
        /// </summary>
        /// <param name="path">Path to the file or folder (relative to share root, e.g., "Documents/report.pdf" or "Folder")</param>
        /// <returns>True if exists, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected to server</exception>
        /// <example>
        /// <code>
        /// if (smb.Exists("Documents/report.pdf"))
        /// {
        ///     Console.WriteLine("File exists!");
        /// }
        /// 
        /// if (smb.Exists("BackupFolder"))
        /// {
        ///     Console.WriteLine("Folder exists!");
        /// }
        /// </code>
        /// </example>
        public async Task<bool> ExistsAsync(string path)
        {
            return await Task.Run(() => Exists(path));
        }

        /// <summary>
        /// Lists all files and folders in the specified directory.
        /// </summary>
        /// <param name="path">Directory path (relative to share root, use "" or "/" for root)</param>
        /// <returns>List of file/folder information. Returns empty list on error (check LastError)</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected to server</exception>
        /// <example>
        /// <code>
        /// var files = smb.ListFiles("Documents");
        /// foreach (var file in files)
        /// {
        ///     Console.WriteLine($"{file.Name} - {(file.IsDirectory ? "DIR" : "FILE")} - {file.Size} bytes");
        ///     Console.WriteLine($"  Modified: {file.LastWriteTime}");
        /// }
        /// </code>
        /// </example>
        public List<SmbFileInfo> ListFiles(string path)
        {
            if (!_isConnected)
            {
                LastError = "Not connected to SMB server";
                throw new InvalidOperationException(LastError);
            }

            var result = new List<SmbFileInfo>();

            try
            {
                LastError = null;
                LastNTStatus = NTStatus.STATUS_SUCCESS;

                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
            AccessMask.GENERIC_READ, SMBLibrary.FileAttributes.Directory,
         ShareAccess.Read | ShareAccess.Write,
         CreateDisposition.FILE_OPEN, CreateOptions.FILE_DIRECTORY_FILE, null);

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    LastError = $"Failed to open directory '{path}': {status}";
                    LastNTStatus = status;
                    return result;
                }

                List<QueryDirectoryFileInformation> fileList;
                status = _fileStore.QueryDirectory(out fileList, handle, "*", FileInformationClass.FileDirectoryInformation);
                LastNTStatus = status;

                _fileStore.CloseFile(handle);

                if (status == NTStatus.STATUS_SUCCESS)
                {
                    foreach (var fileInfo in fileList)
                    {
                        var dirInfo = (FileDirectoryInformation)fileInfo;
                        if (dirInfo.FileName != "." && dirInfo.FileName != "..")
                        {
                            result.Add(new SmbFileInfo
                            {
                                Name = dirInfo.FileName,
                                IsDirectory = (dirInfo.FileAttributes & SMBLibrary.FileAttributes.Directory) != 0,
                                Size = dirInfo.EndOfFile,
                                CreationTime = dirInfo.CreationTime,
                                LastWriteTime = dirInfo.LastWriteTime,
                                LastAccessTime = dirInfo.LastAccessTime
                            });
                        }
                    }
                }
                else
                {
                    LastError = $"Failed to query directory '{path}': {status}";
                }

                return result;
            }
            catch (Exception ex)
            {
                LastError = $"Exception listing files in '{path}': {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Lists files asynchronously.
        /// </summary>
        /// <param name="path">Directory path (relative to share root, use "" or "/" for root)</param>
        /// <returns>Task returning a list of file/folder information. Returns empty list on error (check LastError)</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected to server</exception>
        /// <example>
        /// <code>
        /// var files = await smb.ListFilesAsync("Documents");
        /// foreach (var file in files)
        /// {
        ///     Console.WriteLine($"{file.Name} - {(file.IsDirectory ? "DIR" : "FILE")} - {file.Size} bytes");
        ///     Console.WriteLine($"  Modified: {file.LastWriteTime}");
        /// }
        /// </code>
        /// </example>
        public async Task<List<SmbFileInfo>> ListFilesAsync(string path)
        {
            return await Task.Run(() => ListFiles(path));
        }

        /// <summary>
        /// Lists files and folders with advanced filtering, sorting, and recursive options.
        /// </summary>
        /// <param name="path">Directory path to search</param>
        /// <param name="options">Filtering and sorting options (use SmbListOptions factory methods for convenience)</param>
        /// <returns>Filtered and sorted list of files/folders</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected to server</exception>
        /// <example>
        /// <code>
        /// // List all .txt files recursively
        /// var txtFiles = smb.ListFiles("Documents", SmbListOptions.ByExtension(".txt").Recursive = true);
        /// 
        /// // List files larger than 10MB, sorted by size
        /// var largeFiles = smb.ListFiles("Downloads", SmbListOptions.LargerThan(10 * 1024 * 1024));
        /// 
        /// // Complex filter: recent PDF files only
        /// var options = new SmbListOptions
        /// {
        ///     NamePattern = "*.pdf",
        ///     ModifiedAfterDate = DateTime.Now.AddDays(-7),
        ///     MinSize = 1024,
        ///     SortBy = SmbSortBy.ModifiedTime,
        ///     SortDescending = true,
        ///     MaxResults = 50
        /// };
        /// var recentPdfs = smb.ListFiles("Reports", options);
        /// 
        /// // Custom filter function
        /// var customOptions = new SmbListOptions
        /// {
        ///     Recursive = true,
        ///     CustomFilter = file => file.Name.Contains("backup") && file.Size > 1024 * 1024
        /// };
        /// var backups = smb.ListFiles("", customOptions);
        /// </code>
        /// </example>
        public List<SmbFileInfo> ListFiles(string path, SmbListOptions options)
        {
            if (!_isConnected)
            {
                LastError = "Not connected to SMB server";
                throw new InvalidOperationException(LastError);
            }

            var result = new List<SmbFileInfo>();

            try
            {
                LastError = null;
                LastNTStatus = NTStatus.STATUS_SUCCESS;

                if (options.Recursive)
                {
                    ListFilesRecursiveWithFilter(path, "", result, options);
                }
                else
                {
                    var files = ListFiles(path);
                    foreach (var file in files)
                    {
                        if (ShouldIncludeFile(file, options))
                        {
                            result.Add(file);
                        }
                    }
                }

                // Apply sorting
                result = ApplySorting(result, options);

                // Apply limit
                if (options.MaxResults > 0 && result.Count > options.MaxResults)
                {
                    result = result.Take(options.MaxResults).ToList();
                }

                return result;
            }
            catch (Exception ex)
            {
                LastError = $"Exception listing files with options in '{path}': {ex.Message}";
                return result;
            }
        }

        // List files with advanced options asynchronously
        public async Task<List<SmbFileInfo>> ListFilesAsync(string path, SmbListOptions options)
        {
            return await Task.Run(() => ListFiles(path, options));
        }

        private void ListFilesRecursiveWithFilter(string basePath, string relativePath, List<SmbFileInfo> result, SmbListOptions options)
        {
            string currentPath = string.IsNullOrEmpty(relativePath)
                ? basePath
                : basePath + "/" + relativePath;

            // Check depth limit
            int currentDepth = string.IsNullOrEmpty(relativePath) ? 0 : relativePath.Split('/').Length;
            if (options.MaxDepth > 0 && currentDepth >= options.MaxDepth)
            {
                return;
            }

            var files = ListFiles(currentPath);

            foreach (var file in files)
            {
                var fileInfo = new SmbFileInfo
                {
                    Name = file.Name,
                    FullPath = string.IsNullOrEmpty(relativePath)
                        ? file.Name
                        : relativePath + "/" + file.Name,
                    IsDirectory = file.IsDirectory,
                    Size = file.Size,
                    CreationTime = file.CreationTime,
                    LastWriteTime = file.LastWriteTime,
                    LastAccessTime = file.LastAccessTime
                };

                // Apply filters
                if (ShouldIncludeFile(fileInfo, options))
                {
                    result.Add(fileInfo);
                }

                // Recurse into directories if needed
                if (file.IsDirectory && options.Recursive)
                {
                    ListFilesRecursiveWithFilter(basePath, fileInfo.FullPath, result, options);
                }
            }
        }

        private bool ShouldIncludeFile(SmbFileInfo file, SmbListOptions options)
        {
            // File type filter
            if (options.IncludeFilesOnly && file.IsDirectory)
                return false;
            if (options.IncludeDirectoriesOnly && !file.IsDirectory)
                return false;

            // Name pattern filter
            if (!string.IsNullOrEmpty(options.NamePattern))
            {
                string pattern = "^" + System.Text.RegularExpressions.Regex.Escape(options.NamePattern)
                    .Replace("\\*", ".*")
                    .Replace("\\?", ".") + "$";
                var regex = new System.Text.RegularExpressions.Regex(pattern,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (!regex.IsMatch(file.Name))
                    return false;
            }

            // Extension filter
            if (options.Extensions != null && options.Extensions.Count > 0 && !file.IsDirectory)
            {
                string ext = Path.GetExtension(file.Name)?.ToLower();
                if (string.IsNullOrEmpty(ext) || !options.Extensions.Contains(ext))
                    return false;
            }

            // Size filters
            if (options.MinSize > 0 && file.Size < options.MinSize)
                return false;
            if (options.MaxSize > 0 && file.Size > options.MaxSize)
                return false;

            // Date filters
            if (options.CreatedAfter.HasValue && file.CreationTime < options.CreatedAfter.Value)
                return false;
            if (options.CreatedBefore.HasValue && file.CreationTime > options.CreatedBefore.Value)
                return false;
            if (options.ModifiedAfterDate.HasValue && file.LastWriteTime < options.ModifiedAfterDate.Value)
                return false;
            if (options.ModifiedBeforeDate.HasValue && file.LastWriteTime > options.ModifiedBeforeDate.Value)
                return false;

            // Attributes filter
            if (options.RequiredAttributes.HasValue)
            {
                if ((file.FileAttributes & options.RequiredAttributes.Value) == 0)
                    return false;
            }
            if (options.ExcludedAttributes.HasValue)
            {
                if ((file.FileAttributes & options.ExcludedAttributes.Value) != 0)
                    return false;
            }

            // Custom filter
            if (options.CustomFilter != null && !options.CustomFilter(file))
                return false;

            return true;
        }

        private List<SmbFileInfo> ApplySorting(List<SmbFileInfo> files, SmbListOptions options)
        {
            if (options.SortBy == SmbSortBy.None)
                return files;

            IOrderedEnumerable<SmbFileInfo> sorted = null;

            switch (options.SortBy)
            {
                case SmbSortBy.Name:
                    sorted = options.SortDescending
                        ? files.OrderByDescending(f => f.Name)
                        : files.OrderBy(f => f.Name);
                    break;
                case SmbSortBy.Size:
                    sorted = options.SortDescending
                        ? files.OrderByDescending(f => f.Size)
                        : files.OrderBy(f => f.Size);
                    break;
                case SmbSortBy.CreationTime:
                    sorted = options.SortDescending
                        ? files.OrderByDescending(f => f.CreationTime)
                        : files.OrderBy(f => f.CreationTime);
                    break;
                case SmbSortBy.ModifiedTime:
                    sorted = options.SortDescending
                        ? files.OrderByDescending(f => f.LastWriteTime)
                        : files.OrderBy(f => f.LastWriteTime);
                    break;
                case SmbSortBy.Type:
                    sorted = options.SortDescending
                        ? files.OrderByDescending(f => f.IsDirectory).ThenBy(f => f.Name)
                        : files.OrderBy(f => f.IsDirectory).ThenBy(f => f.Name);
                    break;
            }

            return sorted?.ToList() ?? files;
        }

        /// <summary>
        /// Reads the entire contents of a file into a byte array.
        /// </summary>
        /// <param name="path">Path to the file (e.g., "Documents/data.bin")</param>
        /// <returns>Byte array containing file contents</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected</exception>
        /// <exception cref="IOException">Thrown when file cannot be read</exception>
        /// <example>
        /// <code>
        /// // Read binary file
      /// byte[] data = smb.ReadFile("backup/archive.zip");
        /// File.WriteAllBytes(@"C:\local\archive.zip", data);
        /// 
        /// // Read text file
        /// byte[] textData = smb.ReadFile("logs/app.log");
     /// string content = Encoding.UTF8.GetString(textData);
  /// Console.WriteLine(content);
 /// </code>
  /// </example>
        public byte[] ReadFile(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            object handle;
            FileStatus fileStatus;
            NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
     AccessMask.GENERIC_READ | AccessMask.SYNCHRONIZE, SMBLibrary.FileAttributes.Normal,
                ShareAccess.Read, CreateDisposition.FILE_OPEN, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);

            if (status != NTStatus.STATUS_SUCCESS)
                throw new IOException($"Failed to open file: {status}");

            using (var stream = new MemoryStream())
            {
                long bytesRead = 0;
                while (true)
                {
                    byte[] data;
                    status = _fileStore.ReadFile(out data, handle, bytesRead, (int)_client.MaxReadSize);

                    if (status != NTStatus.STATUS_SUCCESS && status != NTStatus.STATUS_END_OF_FILE)
                    {
                        _fileStore.CloseFile(handle);
                        throw new IOException($"Failed to read file: {status}");
                    }

                    if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                        break;

                    bytesRead += data.Length;
                    stream.Write(data, 0, data.Length);
                }

                _fileStore.CloseFile(handle);
                return stream.ToArray();
            }
        }

        /// <summary>
        /// Reads file asynchronously.
        /// </summary>
        /// <param name="path">Path to the file (e.g., "Documents/data.bin")</param>
        /// <returns>Task<byte[]> that represents the asynchronous read operation. The task result contains the byte array with file contents.</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected</exception>
        /// <exception cref="IOException">Thrown when file cannot be read</exception>
        /// <example>
        /// <code>
        /// // Read binary file
      /// byte[] data = await smb.ReadFileAsync("backup/archive.zip");
        /// File.WriteAllBytes(@"C:\local\archive.zip", data);
        /// 
        /// // Read text file
        /// byte[] textData = await smb.ReadFileAsync("logs/app.log");
     /// string content = Encoding.UTF8.GetString(textData);
  /// Console.WriteLine(content);
 /// </code>
  /// </example>
        public async Task<byte[]> ReadFileAsync(string path)
        {
            return await Task.Run(() => ReadFile(path));
        }

        /// <summary>
        /// Writes data to a file, creating or overwriting the file.
        /// </summary>
        /// <param name="path">Path to the file (e.g., "Documents/report.pdf")</param>
        /// <param name="data">Byte array containing the data to write</param>
        /// <returns>True if write successful, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected</exception>
        /// <example>
        /// <code>
        /// // Write binary data
        /// byte[] data = File.ReadAllBytes(@"C:\local\archive.zip");
        /// bool success = smb.WriteFile("backup/archive.zip", data);
        /// 
        /// // Write text data
        /// string text = "Hello, SMB!";
        /// byte[] textData = Encoding.UTF8.GetBytes(text);
        /// bool textSuccess = smb.WriteFile("greetings.txt", textData);
        /// </code>
        /// </example>
        public bool WriteFile(string path, byte[] data)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
                      AccessMask.GENERIC_WRITE | AccessMask.SYNCHRONIZE, SMBLibrary.FileAttributes.Normal,
                       ShareAccess.None, CreateDisposition.FILE_OVERWRITE_IF, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);

                if (status != NTStatus.STATUS_SUCCESS)
                    return false;

                int writeSize = (int)_client.MaxWriteSize;
                int bytesWritten = 0;

                while (bytesWritten < data.Length)
                {
                    int chunkSize = Math.Min(writeSize, data.Length - bytesWritten);
                    byte[] chunk = new byte[chunkSize];
                    Array.Copy(data, bytesWritten, chunk, 0, chunkSize);

                    int numberOfBytesWritten;
                    status = _fileStore.WriteFile(out numberOfBytesWritten, handle, bytesWritten, chunk);

                    if (status != NTStatus.STATUS_SUCCESS)
                    {
                        _fileStore.CloseFile(handle);
                        return false;
                    }

                    bytesWritten += numberOfBytesWritten;
                }

                _fileStore.CloseFile(handle);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes file asynchronously.
        /// </summary>
        /// <param name="path">Path to the file (e.g., "Documents/report.pdf")</param>
        /// <param name="data">Byte array containing the data to write</param>
        /// <returns>Task<bool> that represents the asynchronous write operation. The task result is true if write is successful.</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected</exception>
        /// <example>
        /// <code>
        /// // Write binary data
        /// byte[] data = File.ReadAllBytes(@"C:\local\archive.zip");
        /// bool success = await smb.WriteFileAsync("backup/archive.zip", data);
        /// 
        /// // Write text data
        /// string text = "Hello, async SMB!";
        /// byte[] textData = Encoding.UTF8.GetBytes(text);
        /// bool textSuccess = await smb.WriteFileAsync("greetings.txt", textData);
        /// </code>
        /// </example>
        public async Task<bool> WriteFileAsync(string path, byte[] data)
        {
            return await Task.Run(() => WriteFile(path, data));
        }

        /// <summary>
        /// Copies a file from source to destination path.
        /// </summary>
        /// <param name="sourcePath">Source file path</param>
        /// <param name="destinationPath">Destination file path (will be overwritten if exists)</param>
        /// <returns>True if successful</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected</exception>
        /// <example>
        /// <code>
        /// // Copy file within same share
        /// smb.CopyFile("Documents/original.docx", "Backup/copy.docx");
   /// 
        /// // Create backup with timestamp
        /// string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
 /// smb.CopyFile("data.db", $"backups/data_{timestamp}.db");
      /// </code>
  /// </example>
        public bool CopyFile(string sourcePath, string destinationPath)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                byte[] data = ReadFile(sourcePath);
                return WriteFile(destinationPath, data);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes a file from the server.
        /// </summary>
        /// <param name="path">Path to the file to delete (e.g., "Documents/report.pdf")</param>
        /// <returns>True if delete successful, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected</exception>
        /// <example>
        /// <code>
        /// // Delete a file
        /// bool success = smb.DeleteFile("Documents/old_report.pdf");
        /// </code>
        /// </example>
        public bool DeleteFile(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
                          AccessMask.GENERIC_WRITE | AccessMask.DELETE | AccessMask.SYNCHRONIZE, SMBLibrary.FileAttributes.Normal,
                       ShareAccess.None, CreateDisposition.FILE_OPEN, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);

                if (status != NTStatus.STATUS_SUCCESS)
                    return false;

                FileDispositionInformation fileDispositionInformation = new FileDispositionInformation
                {
                    DeletePending = true
                };

                status = _fileStore.SetFileInformation(handle, fileDispositionInformation);
                _fileStore.CloseFile(handle);

                return status == NTStatus.STATUS_SUCCESS;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a directory on the SMB server.
        /// </summary>
        /// <param name="path">Directory path to create (e.g., "Documents/NewFolder")</param>
        /// <returns>True if directory created successfully, false otherwise</returns>
        /// <exception cref="InvalidOperationException">Thrown when not connected</exception>
        /// <example>
        /// <code>
        /// // Create a new directory
        /// bool success = smb.CreateDirectory("Projects/2023");
        /// </code>
        /// </example>
        public bool CreateDirectory(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
   AccessMask.GENERIC_WRITE, SMBLibrary.FileAttributes.Directory,
             ShareAccess.None, CreateDisposition.FILE_CREATE, CreateOptions.FILE_DIRECTORY_FILE, null);

                if (status == NTStatus.STATUS_SUCCESS)
                {
                    _fileStore.CloseFile(handle);
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        // Create directory asynchronously
        public async Task<bool> CreateDirectoryAsync(string path)
        {
            return await Task.Run(() => CreateDirectory(path));
        }

        // Delete directory (synchronous)
        public bool DeleteDirectory(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
               AccessMask.GENERIC_WRITE | AccessMask.DELETE, SMBLibrary.FileAttributes.Directory,
              ShareAccess.None, CreateDisposition.FILE_OPEN, CreateOptions.FILE_DIRECTORY_FILE, null);

                if (status != NTStatus.STATUS_SUCCESS)
                    return false;

                FileDispositionInformation fileDispositionInformation = new FileDispositionInformation
                {
                    DeletePending = true
                };

                status = _fileStore.SetFileInformation(handle, fileDispositionInformation);
                _fileStore.CloseFile(handle);

                return status == NTStatus.STATUS_SUCCESS;
            }
            catch
            {
                return false;
            }
        }

        // Delete directory asynchronously
        public async Task<bool> DeleteDirectoryAsync(string path)
        {
            return await Task.Run(() => DeleteDirectory(path));
        }

        // Move/rename file (synchronous)
        public bool MoveFile(string sourcePath, string destinationPath)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, sourcePath,
          AccessMask.GENERIC_WRITE | AccessMask.DELETE | AccessMask.SYNCHRONIZE,
              SMBLibrary.FileAttributes.Normal,
         ShareAccess.None,
                    CreateDisposition.FILE_OPEN,
           CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT,
           null);

                if (status != NTStatus.STATUS_SUCCESS)
                    return false;

                FileRenameInformationType2 renameInfo = new FileRenameInformationType2
                {
                    ReplaceIfExists = false,
                    FileName = destinationPath
                };

                status = _fileStore.SetFileInformation(handle, renameInfo);
                _fileStore.CloseFile(handle);

                return status == NTStatus.STATUS_SUCCESS;
            }
            catch
            {
                return false;
            }
        }

        // Move/rename file asynchronously
        public async Task<bool> MoveFileAsync(string sourcePath, string destinationPath)
        {
            return await Task.Run(() => MoveFile(sourcePath, destinationPath));
        }

        // Rename file (wrapper for MoveFile)
        public bool RenameFile(string path, string newName)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                string directory = Path.GetDirectoryName(path)?.Replace("\\", "/") ?? string.Empty;
                string newPath = string.IsNullOrEmpty(directory)
         ? newName
                    : directory + "/" + newName;

                return MoveFile(path, newPath);
            }
            catch
            {
                return false;
            }
        }

        // Rename file asynchronously
        public async Task<bool> RenameFileAsync(string path, string newName)
        {
            return await Task.Run(() => RenameFile(path, newName));
        }

        // Move directory
        public bool MoveDirectory(string sourcePath, string destinationPath)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, sourcePath,
                AccessMask.GENERIC_WRITE | AccessMask.DELETE,
                          SMBLibrary.FileAttributes.Directory,
                         ShareAccess.None,
                  CreateDisposition.FILE_OPEN,
                        CreateOptions.FILE_DIRECTORY_FILE,
                                 null);

                if (status != NTStatus.STATUS_SUCCESS)
                    return false;

                FileRenameInformationType2 renameInfo = new FileRenameInformationType2
                {
                    ReplaceIfExists = false,
                    FileName = destinationPath
                };

                status = _fileStore.SetFileInformation(handle, renameInfo);
                _fileStore.CloseFile(handle);

                return status == NTStatus.STATUS_SUCCESS;
            }
            catch
            {
                return false;
            }
        }

        // Move directory asynchronously
        public async Task<bool> MoveDirectoryAsync(string sourcePath, string destinationPath)
        {
            return await Task.Run(() => MoveDirectory(sourcePath, destinationPath));
        }

        // Get detailed file information
        public SmbFileInfo GetFileInfo(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                object handle;
                FileStatus fileStatus;
                NTStatus status = _fileStore.CreateFile(out handle, out fileStatus, path,
             AccessMask.GENERIC_READ,
                        SMBLibrary.FileAttributes.Normal,
               ShareAccess.Read | ShareAccess.Write,
                      CreateDisposition.FILE_OPEN,
           CreateOptions.FILE_NON_DIRECTORY_FILE,
                  null);

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    // Try as directory
                    status = _fileStore.CreateFile(out handle, out fileStatus, path,
                   AccessMask.GENERIC_READ,
               SMBLibrary.FileAttributes.Directory,
                ShareAccess.Read | ShareAccess.Write,
                 CreateDisposition.FILE_OPEN,
            CreateOptions.FILE_DIRECTORY_FILE,
                     null);

                    if (status != NTStatus.STATUS_SUCCESS)
                        return null;
                }

                FileInformation fileInfo;
                status = _fileStore.GetFileInformation(out fileInfo, handle, FileInformationClass.FileBasicInformation);

                if (status != NTStatus.STATUS_SUCCESS)
                {
                    _fileStore.CloseFile(handle);
                    return null;
                }

                var basicInfo = (FileBasicInformation)fileInfo;

                // Get size information
                status = _fileStore.GetFileInformation(out fileInfo, handle, FileInformationClass.FileStandardInformation);
                var standardInfo = status == NTStatus.STATUS_SUCCESS ? (FileStandardInformation)fileInfo : null;

                _fileStore.CloseFile(handle);

                return new SmbFileInfo
                {
                    Name = Path.GetFileName(path),
                    IsDirectory = (basicInfo.FileAttributes & SMBLibrary.FileAttributes.Directory) != 0,
                    Size = standardInfo?.EndOfFile ?? 0,
                    CreationTime = basicInfo.CreationTime.Time ?? DateTime.MinValue,
                    LastWriteTime = basicInfo.LastWriteTime.Time ?? DateTime.MinValue,
                    LastAccessTime = basicInfo.LastAccessTime.Time ?? DateTime.MinValue,
                    ChangeTime = basicInfo.ChangeTime.Time ?? DateTime.MinValue,
                    FileAttributes = basicInfo.FileAttributes
                };
            }
            catch
            {
                return null;
            }
        }

        // Get file information asynchronously
        public async Task<SmbFileInfo> GetFileInfoAsync(string path)
        {
            return await Task.Run(() => GetFileInfo(path));
        }

        // Get file size
        public long GetFileSize(string path)
        {
            var info = GetFileInfo(path);
            return info?.Size ?? -1;
        }

        // Get file size asynchronously
        public async Task<long> GetFileSizeAsync(string path)
        {
            return await Task.Run(() => GetFileSize(path));
        }

        // Check if path is directory
        public bool IsDirectory(string path)
        {
            var info = GetFileInfo(path);
            return info?.IsDirectory ?? false;
        }

        // Check if path is directory asynchronously
        public async Task<bool> IsDirectoryAsync(string path)
        {
            return await Task.Run(() => IsDirectory(path));
        }

        // List files recursively (including subfolders)
        public List<SmbFileInfo> ListFilesRecursive(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            var result = new List<SmbFileInfo>();
            ListFilesRecursiveInternal(path, "", result);
            return result;
        }

        private void ListFilesRecursiveInternal(string basePath, string relativePath, List<SmbFileInfo> result)
        {
            string currentPath = string.IsNullOrEmpty(relativePath)
              ? basePath
         : basePath + "/" + relativePath;

            var files = ListFiles(currentPath);

            foreach (var file in files)
            {
                var fileInfo = new SmbFileInfo
                {
                    Name = file.Name,
                    FullPath = string.IsNullOrEmpty(relativePath)
    ? file.Name
: relativePath + "/" + file.Name,
                    IsDirectory = file.IsDirectory,
                    Size = file.Size,
                    CreationTime = file.CreationTime,
                    LastWriteTime = file.LastWriteTime,
                    LastAccessTime = file.LastAccessTime
                };

                result.Add(fileInfo);

                if (file.IsDirectory)
                {
                    ListFilesRecursiveInternal(basePath, fileInfo.FullPath, result);
                }
            }
        }

        // List files recursively asynchronously
        public async Task<List<SmbFileInfo>> ListFilesRecursiveAsync(string path)
        {
            return await Task.Run(() => ListFilesRecursive(path));
        }

        // Delete directory recursively (including all files and subfolders)
        public bool DeleteDirectoryRecursive(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                var files = ListFiles(path);

                foreach (var file in files)
                {
                    string fullPath = path + "/" + file.Name;

                    if (file.IsDirectory)
                    {
                        if (!DeleteDirectoryRecursive(fullPath))
                            return false;
                    }
                    else
                    {
                        if (!DeleteFile(fullPath))
                            return false;
                    }
                }

                return DeleteDirectory(path);
            }
            catch
            {
                return false;
            }
        }

        // Delete directory recursively asynchronously
        public async Task<bool> DeleteDirectoryRecursiveAsync(string path)
        {
            return await Task.Run(() => DeleteDirectoryRecursive(path));
        }

        // Copy directory recursively
        public bool CopyDirectoryRecursive(string sourcePath, string destinationPath)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                // Create destination directory
                if (!Exists(destinationPath))
                {
                    if (!CreateDirectory(destinationPath))
                        return false;
                }

                var files = ListFiles(sourcePath);

                foreach (var file in files)
                {
                    string sourceFullPath = sourcePath + "/" + file.Name;
                    string destFullPath = destinationPath + "/" + file.Name;

                    if (file.IsDirectory)
                    {
                        if (!CopyDirectoryRecursive(sourceFullPath, destFullPath))
                            return false;
                    }
                    else
                    {
                        if (!CopyFile(sourceFullPath, destFullPath))
                            return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Copy directory recursively asynchronously
        public async Task<bool> CopyDirectoryRecursiveAsync(string sourcePath, string destinationPath)
        {
            return await Task.Run(() => CopyDirectoryRecursive(sourcePath, destinationPath));
        }

        // Create directory recursively (create parent folders if not exists)
        public bool CreateDirectoryRecursive(string path)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                if (Exists(path))
                    return true;

                string[] parts = path.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string currentPath = "";

                foreach (var part in parts)
                {
                    currentPath = string.IsNullOrEmpty(currentPath) ? part : currentPath + "/" + part;

                    if (!Exists(currentPath))
                    {
                        if (!CreateDirectory(currentPath))
                            return false;
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        // Create directory recursively asynchronously
        public async Task<bool> CreateDirectoryRecursiveAsync(string path)
        {
            return await Task.Run(() => CreateDirectoryRecursive(path));
        }

        // Count files in directory
        public int GetFileCount(string path, bool recursive = false)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                if (recursive)
                {
                    var files = ListFilesRecursive(path);
                    return files.Count(f => !f.IsDirectory);
                }
                else
                {
                    var files = ListFiles(path);
                    return files.Count(f => !f.IsDirectory);
                }
            }
            catch
            {
                return -1;
            }
        }

        // Count files asynchronously
        public async Task<int> GetFileCountAsync(string path, bool recursive = false)
        {
            return await Task.Run(() => GetFileCount(path, recursive));
        }

        // Count directories in directory
        public int GetDirectoryCount(string path, bool recursive = false)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                if (recursive)
                {
                    var files = ListFilesRecursive(path);
                    return files.Count(f => f.IsDirectory);
                }
                else
                {
                    var files = ListFiles(path);
                    return files.Count(f => f.IsDirectory);
                }
            }
            catch
            {
                return -1;
            }
        }

        // Count directories asynchronously
        public async Task<int> GetDirectoryCountAsync(string path, bool recursive = false)
        {
            return await Task.Run(() => GetDirectoryCount(path, recursive));
        }

        // Calculate total directory size
        public long GetDirectorySize(string path, bool recursive = true)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                long totalSize = 0;

                if (recursive)
                {
                    var files = ListFilesRecursive(path);
                    totalSize = files.Where(f => !f.IsDirectory).Sum(f => f.Size);
                }
                else
                {
                    var files = ListFiles(path);
                    totalSize = files.Where(f => !f.IsDirectory).Sum(f => f.Size);
                }

                return totalSize;
            }
            catch
            {
                return -1;
            }
        }

        // Calculate total directory size asynchronously
        public async Task<long> GetDirectorySizeAsync(string path, bool recursive = true)
        {
            return await Task.Run(() => GetDirectorySize(path, recursive));
        }

        // Search files by pattern
        public List<SmbFileInfo> SearchFiles(string path, string searchPattern, bool recursive = false)
        {
            if (!_isConnected)
                throw new InvalidOperationException("Not connected to SMB server");

            try
            {
                var allFiles = recursive ? ListFilesRecursive(path) : ListFiles(path);

                // Convert wildcard pattern to regex pattern
                string regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(searchPattern)
                 .Replace("\\*", ".*")
                     .Replace("\\?", ".") + "$";

                var regex = new System.Text.RegularExpressions.Regex(regexPattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase);

                return allFiles.Where(f => regex.IsMatch(f.Name)).ToList();
            }
            catch
            {
                return new List<SmbFileInfo>();
            }
        }

        // Search files asynchronously
        public async Task<List<SmbFileInfo>> SearchFilesAsync(string path, string searchPattern, bool recursive = false)
        {
            return await Task.Run(() => SearchFiles(path, searchPattern, recursive));
        }

        // Disconnect from server
        public void Disconnect()
        {
            if (_fileStore != null)
            {
                _fileStore.Disconnect();
                _fileStore = null;
            }

            if (_client != null)
            {
                _client.Logoff();
                _client.Disconnect();
                _client = null;
            }

            _isConnected = false;
        }

        public void Dispose()
        {
            Disconnect();
        }
    }

    // Class containing file/folder information
    public class SmbFileInfo
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime LastWriteTime { get; set; }
        public DateTime LastAccessTime { get; set; }
        public DateTime ChangeTime { get; set; }
        public SMBLibrary.FileAttributes FileAttributes { get; set; }
    }

    // Options for listing files with advanced filtering
    public class SmbListOptions
    {
        // Recursion settings
        public bool Recursive { get; set; }
        public int MaxDepth { get; set; } // 0 = unlimited

        // Type filters
        public bool IncludeFilesOnly { get; set; }
        public bool IncludeDirectoriesOnly { get; set; }

        // Name filters
        public string NamePattern { get; set; } // Supports wildcards: *, ?
        public List<string> Extensions { get; set; } // e.g., .txt, .pdf

        // Size filters (in bytes)
        public long MinSize { get; set; }
        public long MaxSize { get; set; }

        // Date filters
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public DateTime? ModifiedAfterDate { get; set; }
        public DateTime? ModifiedBeforeDate { get; set; }

        // Attribute filters
        public SMBLibrary.FileAttributes? RequiredAttributes { get; set; }
        public SMBLibrary.FileAttributes? ExcludedAttributes { get; set; }

        // Sorting
        public SmbSortBy SortBy { get; set; }
        public bool SortDescending { get; set; }

        // Limit results
        public int MaxResults { get; set; } // 0 = unlimited

        // Custom filter function
        public Func<SmbFileInfo, bool> CustomFilter { get; set; }

        public SmbListOptions()
        {
            Recursive = false;
            MaxDepth = 0;
            IncludeFilesOnly = false;
            IncludeDirectoriesOnly = false;
            MinSize = 0;
            MaxSize = 0;
            SortBy = SmbSortBy.None;
            SortDescending = false;
            MaxResults = 0;
            Extensions = new List<string>();
        }

        // Quick factory methods for common scenarios
        public static SmbListOptions Default()
        {
            return new SmbListOptions();
        }

        public static SmbListOptions RecursiveAll(int maxDepth = 0)
        {
            return new SmbListOptions { Recursive = true, MaxDepth = maxDepth };
        }

        public static SmbListOptions FilesOnly(bool recursive = false)
        {
            return new SmbListOptions { IncludeFilesOnly = true, Recursive = recursive };
        }

        public static SmbListOptions DirectoriesOnly(bool recursive = false)
        {
            return new SmbListOptions { IncludeDirectoriesOnly = true, Recursive = recursive };
        }

        public static SmbListOptions ByExtension(params string[] extensions)
        {
            var options = new SmbListOptions();
            foreach (var ext in extensions)
            {
                string normalized = ext.StartsWith(".") ? ext.ToLower() : "." + ext.ToLower();
                options.Extensions.Add(normalized);
            }
            return options;
        }

        public static SmbListOptions ByPattern(string pattern, bool recursive = false)
        {
            return new SmbListOptions { NamePattern = pattern, Recursive = recursive };
        }

        public static SmbListOptions LargerThan(long minSizeBytes, bool recursive = false)
        {
            return new SmbListOptions { MinSize = minSizeBytes, Recursive = recursive };
        }

        public static SmbListOptions SmallerThan(long maxSizeBytes, bool recursive = false)
        {
            return new SmbListOptions { MaxSize = maxSizeBytes, Recursive = recursive };
        }

        public static SmbListOptions ModifiedAfter(DateTime date, bool recursive = false)
        {
            return new SmbListOptions { ModifiedAfterDate = date, Recursive = recursive };
        }

        public static SmbListOptions SortedBy(SmbSortBy sortBy, bool descending = false)
        {
            return new SmbListOptions { SortBy = sortBy, SortDescending = descending };
        }
    }

    // Sorting options for file listing
    public enum SmbSortBy
    {
        None,
        Name,
        Size,
        CreationTime,
        ModifiedTime,
        Type // Directories first, then files
    }

    // Result class for operations that need detailed error information
    public class SmbResult<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public string ErrorMessage { get; set; }
        public NTStatus? NTStatus { get; set; }
        public Exception Exception { get; set; }

        public static SmbResult<T> CreateSuccess(T data)
        {
            return new SmbResult<T>
            {
                Success = true,
                Data = data
            };
        }

        public static SmbResult<T> CreateError(string errorMessage, NTStatus? ntStatus = null, Exception ex = null)
        {
            return new SmbResult<T>
            {
                Success = false,
                ErrorMessage = errorMessage,
                NTStatus = ntStatus,
                Exception = ex
            };
        }
    }
}
