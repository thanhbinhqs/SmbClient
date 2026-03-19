using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmbClient
{
    public partial class Form1 : Form
    {
        private Smb _smbClient;
        private bool _isConnected = false;

        public Form1()
        {
            InitializeComponent();
            InitializeForm();
        }

        private ContextMenuStrip _ctxMenu;
        private ContextMenuStrip _ctxLocalMenu;

        private void InitializeForm()
        {
            // Set default values
            //txtServer.Text = "192.168.1.100";
            //txtShareName.Text = "SharedDocs";
            //txtUsername.Text = "";
            //txtPassword.Text = "";
            //txtDomain.Text = "";
            //txtPath.Text = "";
            txtLocalPath.Text = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);

            // Initialize combo box
            cmbSortBy.SelectedIndex = 0; // None

            // Setup TreeView icons
            SetupTreeViewIcons();
            SetupContextMenu();
            
            treeViewLocal.BeforeExpand += TreeViewLocal_BeforeExpand;
            treeViewLocal.AfterSelect += TreeViewLocal_AfterSelect;
            treeViewFiles.BeforeExpand += TreeViewFiles_BeforeExpand;
            treeViewFiles.AfterSelect += TreeViewFiles_AfterSelect;

            // Load local files initially
            LoadLocalFiles(txtLocalPath.Text);

            // Disable modified after filter by default
            dateModifiedAfter.Checked = false;
        }

        private void SetupContextMenu()
        {
            _ctxMenu = new ContextMenuStrip();
            
            var itemDownload = new ToolStripMenuItem("Download to Local selected path");
            itemDownload.Click += ItemDownload_Click;
            
            var itemUpload = new ToolStripMenuItem("Upload from PC via Dialog");
            itemUpload.Click += ItemUpload_Click;
            
            var itemDelete = new ToolStripMenuItem("Delete");
            itemDelete.Click += ItemDelete_Click;
            
            _ctxMenu.Items.Add(itemDownload);
            _ctxMenu.Items.Add(itemUpload);
            _ctxMenu.Items.Add(new ToolStripSeparator());
            _ctxMenu.Items.Add(itemDelete);
            
            treeViewFiles.ContextMenuStrip = _ctxMenu;
            treeViewFiles.NodeMouseClick += TreeViewFiles_NodeMouseClick;

            // Setup Local Context Menu
            _ctxLocalMenu = new ContextMenuStrip();

            var itemUploadLocal = new ToolStripMenuItem("Upload to SMB selected path");
            itemUploadLocal.Click += ItemUploadLocal_Click;

            var itemDeleteLocal = new ToolStripMenuItem("Delete Local File");
            itemDeleteLocal.Click += ItemDeleteLocal_Click;

            _ctxLocalMenu.Items.Add(itemUploadLocal);
            _ctxLocalMenu.Items.Add(new ToolStripSeparator());
            _ctxLocalMenu.Items.Add(itemDeleteLocal);

            treeViewLocal.ContextMenuStrip = _ctxLocalMenu;
            treeViewLocal.NodeMouseClick += TreeViewLocal_NodeMouseClick;
        }

        private void TreeViewFiles_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewFiles.SelectedNode = e.Node;
            }
        }

        private void TreeViewLocal_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeViewLocal.SelectedNode = e.Node;
            }
        }

        private void SetupTreeViewIcons()
        {
            try
            {
                // Create simple icons for folder and file
                imageList.Images.Add("folder", SystemIcons.Shield);
                imageList.Images.Add("file", SystemIcons.Application);
            }
            catch
            {
                // If icons fail to load, continue without them
            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(txtServer.Text))
            {
                MessageBox.Show("Please enter server address.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtServer.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtShareName.Text))
            {
                MessageBox.Show("Please enter share name.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtShareName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Please enter username.", "Validation Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            // Disable controls during connection
            SetControlsEnabled(false);
            lblStatus.Text = "Connecting...";
            progressBar.Visible = true;

            try
            {
                _smbClient = new Smb();

                bool connected = await _smbClient.ConnectAsync(
                txtServer.Text.Trim(),
                txtShareName.Text.Trim(),
                txtUsername.Text.Trim(),
                txtPassword.Text,
                txtDomain.Text.Trim()
                );

                if (connected)
                {
                    _isConnected = true;
                    lblStatus.Text = $"Connected to {txtServer.Text}\\{txtShareName.Text}";

                    // Enable browse and filter controls
                    groupBoxFilter.Enabled = true;
                    txtPath.Enabled = true;
                    btnBrowse.Enabled = true;
                    btnDisconnect.Enabled = true;
                    btnConnect.Enabled = false;

                    // Load root directory
                    //txtPath.Text = "";
                    await LoadFilesAsync("");
                }
                else
                {
                    MessageBox.Show($"Failed to connect to SMB server.\n\nError: {_smbClient.LastError}",
                    "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    lblStatus.Text = "Connection failed";
                    SetControlsEnabled(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error connecting to server:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Connection error";
                SetControlsEnabled(true);
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            DisconnectFromServer();
        }

        private void DisconnectFromServer()
        {
            if (_smbClient != null)
            {
                _smbClient.Dispose();
                _smbClient = null;
            }

            _isConnected = false;
            lblStatus.Text = "Disconnected";
            treeViewFiles.Nodes.Clear();
            lblFileCount.Text = "";

            // Reset controls
            groupBoxFilter.Enabled = false;
            txtPath.Enabled = false;
            btnBrowse.Enabled = false;
            btnDisconnect.Enabled = false;
            btnConnect.Enabled = true;
            SetControlsEnabled(true);
        }

        private void SetControlsEnabled(bool enabled)
        {
            txtServer.Enabled = enabled;
            txtShareName.Enabled = enabled;
            txtUsername.Enabled = enabled;
            txtPassword.Enabled = enabled;
            txtDomain.Enabled = enabled;
        }

        private async void btnBrowse_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                MessageBox.Show("Please connect to SMB server first.", "Not Connected",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            await LoadFilesAsync(txtPath.Text.Trim());
        }

        private void btnLocalBrowse_Click(object sender, EventArgs e)
        {
            LoadLocalFiles(txtLocalPath.Text.Trim());
        }

        private void LoadLocalFiles(string path)
        {
            treeViewLocal.Nodes.Clear();
            if (string.IsNullOrEmpty(path))
            {
                // List drives
                foreach (var drive in System.IO.DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        var node = new TreeNode(drive.Name);
                        node.Tag = new LocalFileInfo { FullPath = drive.Name, IsDirectory = true, Name = drive.Name };
                        node.ImageKey = "folder";
                        node.SelectedImageKey = "folder";
                        node.Nodes.Add("..."); // Dummy node for expansion
                        treeViewLocal.Nodes.Add(node);
                    }
                }
                return;
            }

            try
            {
                var dirInfo = new System.IO.DirectoryInfo(path);
                if (!dirInfo.Exists)
                {
                    MessageBox.Show("Local directory does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var dir in dirInfo.GetDirectories())
                {
                    var node = new TreeNode(dir.Name);
                    node.Tag = new LocalFileInfo { FullPath = dir.FullName, IsDirectory = true, Name = dir.Name };
                    node.ImageKey = "folder";
                    node.SelectedImageKey = "folder";
                    node.Nodes.Add("..."); // Lazy loading
                    treeViewLocal.Nodes.Add(node);
                }
                foreach (var file in dirInfo.GetFiles())
                {
                    var node = new TreeNode(file.Name + $" ({FormatFileSize(file.Length)})");
                    node.Tag = new LocalFileInfo { FullPath = file.FullName, IsDirectory = false, Name = file.Name, Size = file.Length };
                    node.ImageKey = "file";
                    node.SelectedImageKey = "file";
                    treeViewLocal.Nodes.Add(node);
                }
                txtLocalPath.Text = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading local files: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void TreeViewFiles_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "...")
            {
                e.Node.Nodes.Clear();
                var fileInfo = e.Node.Tag as SmbFileInfo;
                if (fileInfo != null && fileInfo.IsDirectory)
                {
                    try
                    {
                        var options = BuildFilterOptions();
                        options.Recursive = false; // Override for lazy expansion
                        
                        string currentPath = fileInfo.FullPath ?? fileInfo.Name;
                        progressBar.Visible = true;
                        lblStatus.Text = $"Loading {currentPath}...";

                        List<SmbFileInfo> subFiles = await _smbClient.ListFilesAsync(currentPath, options);

                        foreach (var subFile in subFiles)
                        {
                            // Ensure FullPath is properly formatted so further sub-expansions know their absolute path
                            subFile.FullPath = string.IsNullOrEmpty(currentPath) ? subFile.Name : currentPath + "/" + subFile.Name;
                            var subNode = CreateTreeNode(subFile);
                            e.Node.Nodes.Add(subNode);
                        }
                        
                        lblStatus.Text = "Ready";
                        progressBar.Visible = false;
                    }
                    catch (Exception ex)
                    {
                        progressBar.Visible = false;
                        lblStatus.Text = "Error expanding directory";
                        MessageBox.Show($"Error expanding directory:\n{ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        
        private void TreeViewFiles_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var info = e.Node.Tag as SmbFileInfo;
            if (info != null)
            {
                txtPath.Text = info.FullPath ?? info.Name;
            }
            else
            {
                txtPath.Text = "";
            }
        }

        private void TreeViewLocal_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Text == "...")
            {
                e.Node.Nodes.Clear();
                var info = e.Node.Tag as LocalFileInfo;
                if (info != null && info.IsDirectory)
                {
                    try
                    {
                        var dirInfo = new System.IO.DirectoryInfo(info.FullPath);
                        foreach (var dir in dirInfo.GetDirectories())
                        {
                            var node = new TreeNode(dir.Name);
                            node.Tag = new LocalFileInfo { FullPath = dir.FullName, IsDirectory = true, Name = dir.Name };
                            node.ImageKey = "folder";
                            node.SelectedImageKey = "folder";
                            node.Nodes.Add("...");
                            e.Node.Nodes.Add(node);
                        }
                        foreach (var file in dirInfo.GetFiles())
                        {
                            var node = new TreeNode(file.Name + $" ({FormatFileSize(file.Length)})");
                            node.Tag = new LocalFileInfo { FullPath = file.FullName, IsDirectory = false, Name = file.Name, Size = file.Length };
                            node.ImageKey = "file";
                            node.SelectedImageKey = "file";
                            e.Node.Nodes.Add(node);
                        }
                    }
                    catch { } // Ignore access denied exceptions
                }
            }
        }

        private void TreeViewLocal_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var info = e.Node.Tag as LocalFileInfo;
            if (info != null)
            {
                txtLocalPath.Text = info.FullPath;
            }
        }

        private async Task LoadFilesAsync(string path)
        {
            if (_smbClient == null || !_isConnected)
                return;

            progressBar.Visible = true;
            lblStatus.Text = "Loading files...";
            treeViewFiles.Nodes.Clear();

            try
            {
                // Build filter options
                var options = BuildFilterOptions();

                // Load files with filter
                List<SmbFileInfo> files = await _smbClient.ListFilesAsync(path, options);

                // Display in TreeView
                DisplayFilesInTreeView(files, path);

                lblFileCount.Text = $"Total: {files.Count} items ({files.Count(f => !f.IsDirectory)} files, {files.Count(f => f.IsDirectory)} folders)";
                lblStatus.Text = "Ready";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading files:\n{ex.Message}", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Error loading files";
                lblFileCount.Text = "";
            }
            finally
            {
                progressBar.Visible = false;
            }
        }

        private SmbListOptions BuildFilterOptions()
        {
            var options = new SmbListOptions
            {
                Recursive = chkRecursive.Checked,
                MaxDepth = chkRecursive.Checked ? (int)numMaxDepth.Value : 0,
                IncludeFilesOnly = chkFilesOnly.Checked,
                IncludeDirectoriesOnly = chkDirectoriesOnly.Checked,
                MinSize = (long)numMinSize.Value,
                MaxSize = (long)numMaxSize.Value > 0 ? (long)numMaxSize.Value : 0
            };

            // Name pattern filter
            if (!string.IsNullOrWhiteSpace(txtNamePattern.Text))
            {
                options.NamePattern = txtNamePattern.Text.Trim();
            }

            // Extensions filter
            if (!string.IsNullOrWhiteSpace(txtExtensions.Text))
            {
                var extensions = txtExtensions.Text.Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                options.Extensions = extensions.Select(ext => ext.Trim().ToLower().StartsWith(".") ? ext.Trim().ToLower() : "." + ext.Trim().ToLower()).ToList();
            }

            // Modified after filter
            if (dateModifiedAfter.Checked)
            {
                options.ModifiedAfterDate = dateModifiedAfter.Value.Date;
            }

            // Sort options
            switch (cmbSortBy.SelectedIndex)
            {
                case 0: options.SortBy = SmbSortBy.None; break;
                case 1: options.SortBy = SmbSortBy.Name; break;
                case 2: options.SortBy = SmbSortBy.Size; break;
                case 3: options.SortBy = SmbSortBy.CreationTime; break;
                case 4: options.SortBy = SmbSortBy.ModifiedTime; break;
                case 5: options.SortBy = SmbSortBy.Type; break;
            }
            options.SortDescending = chkSortDescending.Checked;

            return options;
        }

        private void DisplayFilesInTreeView(List<SmbFileInfo> files, string basePath)
        {
            treeViewFiles.BeginUpdate();
            treeViewFiles.Nodes.Clear();

            if (chkRecursive.Checked)
            {
                // Build hierarchical tree for recursive listing
                BuildHierarchicalTree(files, basePath);
            }
            else
            {
                // Flat list for non-recursive
                foreach (var file in files)
                {
                    var node = CreateTreeNode(file);
                    treeViewFiles.Nodes.Add(node);
                }
            }

            treeViewFiles.EndUpdate();
        }

        private void BuildHierarchicalTree(List<SmbFileInfo> files, string basePath)
        {
            // Group files by their directory path
            var grouped = files.GroupBy(f => GetDirectoryPath(f.FullPath ?? f.Name))
                .OrderBy(g => g.Key);

            // Create root nodes first
            var rootFiles = files.Where(f => !HasParentInPath(f.FullPath ?? f.Name)).ToList();

            foreach (var file in rootFiles)
            {
                var node = CreateTreeNode(file);
                treeViewFiles.Nodes.Add(node);

                if (file.IsDirectory)
                {
                    // Add children recursively
                    AddChildNodes(node, files, file.FullPath ?? file.Name);
                }
            }
        }

        private void AddChildNodes(TreeNode parentNode, List<SmbFileInfo> allFiles, string parentPath)
        {
            var children = allFiles.Where(f =>
            {
                var fullPath = f.FullPath ?? f.Name;
                if (fullPath == parentPath) return false;

                var dir = GetDirectoryPath(fullPath);
                return dir == parentPath;
            }).ToList();

            foreach (var child in children)
            {
                var childNode = CreateTreeNode(child);
                parentNode.Nodes.Add(childNode);

                if (child.IsDirectory)
                {
                    AddChildNodes(childNode, allFiles, child.FullPath ?? child.Name);
                }
            }
        }

        private string GetDirectoryPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return "";

            var lastSlash = fullPath.LastIndexOf('/');
            if (lastSlash < 0)
                return "";

            return fullPath.Substring(0, lastSlash);
        }

        private bool HasParentInPath(string fullPath)
        {
            if (string.IsNullOrEmpty(fullPath))
                return false;

            return fullPath.Contains('/');
        }

        private TreeNode CreateTreeNode(SmbFileInfo file)
        {
            var node = new TreeNode();

            // Format display text
            string displayText = file.Name;
            if (!file.IsDirectory)
            {
                displayText += $" ({FormatFileSize(file.Size)})";
            }
            displayText += $" - {file.LastWriteTime:yyyy-MM-dd HH:mm:ss}";

            node.Text = displayText;
            node.Tag = file;

            // Set icon
            if (file.IsDirectory)
            {
                node.ImageKey = "folder";
                node.SelectedImageKey = "folder";
                
                // Add dummy node for lazy expansion if not running recursively
                if (!chkRecursive.Checked)
                {
                    node.Nodes.Add("...");
                }
            }
            else
            {
                node.ImageKey = "file";
                node.SelectedImageKey = "file";
            }

            return node;
        }

        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        private void chkRecursive_CheckedChanged(object sender, EventArgs e)
        {
            numMaxDepth.Enabled = chkRecursive.Checked;
        }

        private void chkFilesOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkFilesOnly.Checked)
            {
                chkDirectoriesOnly.Checked = false;
            }
        }

        private void chkDirectoriesOnly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkDirectoriesOnly.Checked)
            {
                chkFilesOnly.Checked = false;
            }
        }

        private async void btnClearFilter_Click(object sender, EventArgs e)
        {
            // Clear all filter controls
            chkRecursive.Checked = false;
            numMaxDepth.Value = 0;
            txtExtensions.Clear();
            txtNamePattern.Clear();
            numMinSize.Value = 0;
            numMaxSize.Value = 0;
            dateModifiedAfter.Checked = false;
            chkFilesOnly.Checked = false;
            chkDirectoriesOnly.Checked = false;
            cmbSortBy.SelectedIndex = 0;
            chkSortDescending.Checked = false;

            // Reload files if connected
            if (_isConnected)
            {
                await LoadFilesAsync(txtPath.Text.Trim());
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            DisconnectFromServer();
            base.OnFormClosing(e);
        }

        private async void ItemDownload_Click(object sender, EventArgs e)
        {
            var smbNode = treeViewFiles.SelectedNode;
            if (smbNode == null) return;

            var fileInfo = smbNode.Tag as SmbFileInfo;
            if (fileInfo == null || fileInfo.IsDirectory)
            {
                MessageBox.Show("Please select an SMB file to download, not a directory.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string localDir = txtLocalPath.Text.Trim();
            var localNode = treeViewLocal.SelectedNode;
            if (localNode != null)
            {
                var localTag = localNode.Tag as LocalFileInfo;
                if (localTag != null)
                {
                    localDir = localTag.IsDirectory ? localTag.FullPath : System.IO.Path.GetDirectoryName(localTag.FullPath);
                }
            }

            if (string.IsNullOrEmpty(localDir) || !System.IO.Directory.Exists(localDir))
            {
                MessageBox.Show("Please select a valid local destination folder in the right pane.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string localFilePath = System.IO.Path.Combine(localDir, fileInfo.Name);

            if (System.IO.File.Exists(localFilePath) && MessageBox.Show($"File '{fileInfo.Name}' already exists locally. Overwrite?", "Overwrite", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            {
                return;
            }

            await DownloadFileWithProgressAsync(fileInfo.FullPath ?? fileInfo.Name, localFilePath, fileInfo.Size);
            
            // Refresh local pane
            LoadLocalFiles(localDir);
        }

        private async void ItemUploadLocal_Click(object sender, EventArgs e)
        {
            if (!_isConnected)
            {
                MessageBox.Show("Please connect to SMB server first.", "Not Connected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var localNode = treeViewLocal.SelectedNode;
            if (localNode == null) return;

            var localTag = localNode.Tag as LocalFileInfo;
            if (localTag == null || localTag.IsDirectory)
            {
                MessageBox.Show("Please select a local file to upload, not a directory.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string remoteDir = txtPath.Text.Trim();
            var smbNode = treeViewFiles.SelectedNode;
            if (smbNode != null)
            {
                var smbTag = smbNode.Tag as SmbFileInfo;
                if (smbTag != null)
                {
                    remoteDir = smbTag.IsDirectory ? (smbTag.FullPath ?? smbTag.Name) : GetDirectoryPath(smbTag.FullPath ?? smbTag.Name);
                }
            }

            string remoteFilePath = string.IsNullOrEmpty(remoteDir) ? localTag.Name : $"{remoteDir}/{localTag.Name}";
            
            await UploadFileWithProgressAsync(localTag.FullPath, remoteFilePath, localTag.Size);
        }

        private void ItemDeleteLocal_Click(object sender, EventArgs e)
        {
            var node = treeViewLocal.SelectedNode;
            if (node == null) return;

            var localTag = node.Tag as LocalFileInfo;
            if (localTag == null) return;

            if (MessageBox.Show($"Are you sure you want to delete local item '{localTag.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    if (localTag.IsDirectory)
                    {
                        System.IO.Directory.Delete(localTag.FullPath, true);
                    }
                    else
                    {
                        System.IO.File.Delete(localTag.FullPath);
                    }
                    LoadLocalFiles(txtLocalPath.Text.Trim());
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting local file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void ItemUpload_Click(object sender, EventArgs e)
        {
            string remoteDir = "";
            
            var node = treeViewFiles.SelectedNode;
            if (node != null)
            {
                var fileInfo = node.Tag as SmbFileInfo;
                if (fileInfo != null)
                {
                    remoteDir = fileInfo.IsDirectory ? (fileInfo.FullPath ?? fileInfo.Name) : GetDirectoryPath(fileInfo.FullPath ?? fileInfo.Name);
                }
            }

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    var localFile = new System.IO.FileInfo(ofd.FileName);
                    string remotePath = string.IsNullOrEmpty(remoteDir) ? localFile.Name : $"{remoteDir}/{localFile.Name}";
                    
                    await UploadFileWithProgressAsync(ofd.FileName, remotePath, localFile.Length);
                }
            }
        }

        private async void ItemDelete_Click(object sender, EventArgs e)
        {
            var node = treeViewFiles.SelectedNode;
            if (node == null) return;

            var fileInfo = node.Tag as SmbFileInfo;
            if (fileInfo == null) return;

            if (MessageBox.Show($"Are you sure you want to delete '{fileInfo.Name}'?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string path = fileInfo.FullPath ?? fileInfo.Name;
                bool success = false;
                
                try
                {
                    lblStatus.Text = $"Deleting {fileInfo.Name}...";
                    if (fileInfo.IsDirectory)
                    {
                        success = await _smbClient.DeleteDirectoryRecursiveAsync(path);
                    }
                    else
                    {
                        success = await Task.Run(() => _smbClient.DeleteFile(path));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error deleting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    lblStatus.Text = success ? "Deleted successfully" : "Delete failed";
                    if (success)
                    {
                        await LoadFilesAsync(txtPath.Text.Trim());
                    }
                }
            }
        }

        private async Task DownloadFileWithProgressAsync(string remotePath, string localPath, long totalBytes)
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = true;
            lblStatus.Text = $"Downloading {System.IO.Path.GetFileName(remotePath)}...";

            try
            {
                var progress = new Progress<long>(bytes =>
                {
                    if (totalBytes > 0)
                    {
                        int percent = (int)(bytes * 100 / totalBytes);
                        progressBar.Value = Math.Min(percent, 100);
                        lblStatus.Text = $"Downloading: {FormatFileSize(bytes)} / {FormatFileSize(totalBytes)} ({percent}%)";
                    }
                    else
                    {
                        lblStatus.Text = $"Downloading: {FormatFileSize(bytes)}";
                    }
                });

                await _smbClient.DownloadFileAsync(remotePath, localPath, progress);
                MessageBox.Show("Download completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Download completed";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Download failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Download error";
            }
            finally
            {
                progressBar.Visible = false;
                progressBar.Style = ProgressBarStyle.Marquee;
            }
        }

        private async Task UploadFileWithProgressAsync(string localPath, string remotePath, long totalBytes)
        {
            progressBar.Style = ProgressBarStyle.Continuous;
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;
            progressBar.Visible = true;
            lblStatus.Text = $"Uploading {System.IO.Path.GetFileName(localPath)}...";

            try
            {
                var progress = new Progress<long>(bytes =>
                {
                    if (totalBytes > 0)
                    {
                        int percent = (int)(bytes * 100 / totalBytes);
                        progressBar.Value = Math.Min(percent, 100);
                        lblStatus.Text = $"Uploading: {FormatFileSize(bytes)} / {FormatFileSize(totalBytes)} ({percent}%)";
                    }
                    else
                    {
                        lblStatus.Text = $"Uploading: {FormatFileSize(bytes)}";
                    }
                });

                await _smbClient.UploadFileAsync(localPath, remotePath, progress);
                MessageBox.Show("Upload completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                lblStatus.Text = "Upload completed";
                
                // Refresh the current folder view
                await LoadFilesAsync(txtPath.Text.Trim());
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Upload failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                lblStatus.Text = "Upload error";
            }
            finally
            {
                progressBar.Visible = false;
                progressBar.Style = ProgressBarStyle.Marquee;
            }
        }
    }

    public class LocalFileInfo
    {
        public string Name { get; set; }
        public string FullPath { get; set; }
        public bool IsDirectory { get; set; }
        public long Size { get; set; }
    }
}
