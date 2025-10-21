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

        private void InitializeForm()
        {
            // Set default values
            txtServer.Text = "192.168.1.100";
            txtShareName.Text = "SharedDocs";
            txtUsername.Text = "";
            txtPassword.Text = "";
            txtDomain.Text = "";
            txtPath.Text = "";

            // Initialize combo box
            cmbSortBy.SelectedIndex = 0; // None

            // Setup TreeView icons
            SetupTreeViewIcons();

            // Disable modified after filter by default
            dateModifiedAfter.Checked = false;
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
                    MessageBox.Show("Successfully connected to SMB server!", "Connection Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Enable browse and filter controls
                    groupBoxFilter.Enabled = true;
                    txtPath.Enabled = true;
                    btnBrowse.Enabled = true;
                    btnDisconnect.Enabled = true;
                    btnConnect.Enabled = false;

                    // Load root directory
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
    }
}
