namespace SmbClient
{
  partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
  /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
 protected override void Dispose(bool disposing)
        {
    if (disposing && (components != null))
    {
             components.Dispose();
     }
            base.Dispose(disposing);
     }

      #region Windows Form Designer generated code

        /// <summary>
   /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
 /// </summary>
        private void InitializeComponent()
      {
            this.components = new System.ComponentModel.Container();
            this.groupBoxConnection = new System.Windows.Forms.GroupBox();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtDomain = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtShareName = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblDomain = new System.Windows.Forms.Label();
            this.lblPassword = new System.Windows.Forms.Label();
            this.lblUsername = new System.Windows.Forms.Label();
            this.lblShareName = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.groupBoxFilter = new System.Windows.Forms.GroupBox();
            this.btnClearFilter = new System.Windows.Forms.Button();
            this.chkSortDescending = new System.Windows.Forms.CheckBox();
            this.lblSortBy = new System.Windows.Forms.Label();
            this.cmbSortBy = new System.Windows.Forms.ComboBox();
            this.chkDirectoriesOnly = new System.Windows.Forms.CheckBox();
            this.chkFilesOnly = new System.Windows.Forms.CheckBox();
            this.lblModifiedAfter = new System.Windows.Forms.Label();
            this.dateModifiedAfter = new System.Windows.Forms.DateTimePicker();
            this.lblMaxSize = new System.Windows.Forms.Label();
            this.numMaxSize = new System.Windows.Forms.NumericUpDown();
            this.lblMinSize = new System.Windows.Forms.Label();
            this.numMinSize = new System.Windows.Forms.NumericUpDown();
            this.lblNamePattern = new System.Windows.Forms.Label();
            this.txtNamePattern = new System.Windows.Forms.TextBox();
            this.lblExtensions = new System.Windows.Forms.Label();
            this.txtExtensions = new System.Windows.Forms.TextBox();
            this.lblMaxDepth = new System.Windows.Forms.Label();
            this.numMaxDepth = new System.Windows.Forms.NumericUpDown();
            this.chkRecursive = new System.Windows.Forms.CheckBox();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.treeViewFiles = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblFileCount = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.splitContainerMain = new System.Windows.Forms.SplitContainer();
            this.panelSmbTop = new System.Windows.Forms.Panel();
            this.treeViewLocal = new System.Windows.Forms.TreeView();
            this.panelLocalTop = new System.Windows.Forms.Panel();
            this.lblLocalPath = new System.Windows.Forms.Label();
            this.txtLocalPath = new System.Windows.Forms.TextBox();
            this.btnLocalBrowse = new System.Windows.Forms.Button();
            this.groupBoxConnection.SuspendLayout();
            this.groupBoxFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxDepth)).BeginInit();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).BeginInit();
            this.splitContainerMain.Panel1.SuspendLayout();
            this.splitContainerMain.Panel2.SuspendLayout();
            this.splitContainerMain.SuspendLayout();
            this.panelSmbTop.SuspendLayout();
            this.panelLocalTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxConnection
            // 
            this.groupBoxConnection.Controls.Add(this.btnDisconnect);
            this.groupBoxConnection.Controls.Add(this.btnConnect);
            this.groupBoxConnection.Controls.Add(this.txtDomain);
            this.groupBoxConnection.Controls.Add(this.txtPassword);
            this.groupBoxConnection.Controls.Add(this.txtUsername);
            this.groupBoxConnection.Controls.Add(this.txtShareName);
            this.groupBoxConnection.Controls.Add(this.txtServer);
            this.groupBoxConnection.Controls.Add(this.lblDomain);
            this.groupBoxConnection.Controls.Add(this.lblPassword);
            this.groupBoxConnection.Controls.Add(this.lblUsername);
            this.groupBoxConnection.Controls.Add(this.lblShareName);
            this.groupBoxConnection.Controls.Add(this.lblServer);
            this.groupBoxConnection.Location = new System.Drawing.Point(14, 15);
            this.groupBoxConnection.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxConnection.Name = "groupBoxConnection";
            this.groupBoxConnection.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxConnection.Size = new System.Drawing.Size(394, 275);
            this.groupBoxConnection.TabIndex = 0;
            this.groupBoxConnection.TabStop = false;
            this.groupBoxConnection.Text = "SMB Connection";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Enabled = false;
            this.btnDisconnect.Location = new System.Drawing.Point(242, 206);
            this.btnDisconnect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(129, 44);
            this.btnDisconnect.TabIndex = 6;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.UseVisualStyleBackColor = true;
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(101, 206);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(129, 44);
            this.btnConnect.TabIndex = 5;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtDomain
            // 
            this.txtDomain.Location = new System.Drawing.Point(101, 162);
            this.txtDomain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtDomain.Name = "txtDomain";
            this.txtDomain.Size = new System.Drawing.Size(270, 26);
            this.txtDomain.TabIndex = 4;
            this.txtDomain.Text = ".";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(101, 128);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '*';
            this.txtPassword.Size = new System.Drawing.Size(270, 26);
            this.txtPassword.TabIndex = 3;
            this.txtPassword.Text = "1234567890";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(101, 92);
            this.txtUsername.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(270, 26);
            this.txtUsername.TabIndex = 2;
            this.txtUsername.Text = "share";
            // 
            // txtShareName
            // 
            this.txtShareName.Location = new System.Drawing.Point(101, 58);
            this.txtShareName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtShareName.Name = "txtShareName";
            this.txtShareName.Size = new System.Drawing.Size(270, 26);
            this.txtShareName.TabIndex = 1;
            this.txtShareName.Text = "media";
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(101, 22);
            this.txtServer.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(270, 26);
            this.txtServer.TabIndex = 0;
            this.txtServer.Text = "192.168.1.250";
            // 
            // lblDomain
            // 
            this.lblDomain.AutoSize = true;
            this.lblDomain.Location = new System.Drawing.Point(17, 166);
            this.lblDomain.Name = "lblDomain";
            this.lblDomain.Size = new System.Drawing.Size(68, 20);
            this.lblDomain.TabIndex = 4;
            this.lblDomain.Text = "Domain:";
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(17, 131);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(82, 20);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Password:";
            // 
            // lblUsername
            // 
            this.lblUsername.AutoSize = true;
            this.lblUsername.Location = new System.Drawing.Point(17, 96);
            this.lblUsername.Name = "lblUsername";
            this.lblUsername.Size = new System.Drawing.Size(87, 20);
            this.lblUsername.TabIndex = 2;
            this.lblUsername.Text = "Username:";
            // 
            // lblShareName
            // 
            this.lblShareName.AutoSize = true;
            this.lblShareName.Location = new System.Drawing.Point(17, 61);
            this.lblShareName.Name = "lblShareName";
            this.lblShareName.Size = new System.Drawing.Size(56, 20);
            this.lblShareName.TabIndex = 1;
            this.lblShareName.Text = "Share:";
            // 
            // lblServer
            // 
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(17, 26);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(59, 20);
            this.lblServer.TabIndex = 0;
            this.lblServer.Text = "Server:";
            // 
            // groupBoxFilter
            // 
            this.groupBoxFilter.Controls.Add(this.btnClearFilter);
            this.groupBoxFilter.Controls.Add(this.chkSortDescending);
            this.groupBoxFilter.Controls.Add(this.lblSortBy);
            this.groupBoxFilter.Controls.Add(this.cmbSortBy);
            this.groupBoxFilter.Controls.Add(this.chkDirectoriesOnly);
            this.groupBoxFilter.Controls.Add(this.chkFilesOnly);
            this.groupBoxFilter.Controls.Add(this.lblModifiedAfter);
            this.groupBoxFilter.Controls.Add(this.dateModifiedAfter);
            this.groupBoxFilter.Controls.Add(this.lblMaxSize);
            this.groupBoxFilter.Controls.Add(this.numMaxSize);
            this.groupBoxFilter.Controls.Add(this.lblMinSize);
            this.groupBoxFilter.Controls.Add(this.numMinSize);
            this.groupBoxFilter.Controls.Add(this.lblNamePattern);
            this.groupBoxFilter.Controls.Add(this.txtNamePattern);
            this.groupBoxFilter.Controls.Add(this.lblExtensions);
            this.groupBoxFilter.Controls.Add(this.txtExtensions);
            this.groupBoxFilter.Controls.Add(this.lblMaxDepth);
            this.groupBoxFilter.Controls.Add(this.numMaxDepth);
            this.groupBoxFilter.Controls.Add(this.chkRecursive);
            this.groupBoxFilter.Enabled = false;
            this.groupBoxFilter.Location = new System.Drawing.Point(414, 15);
            this.groupBoxFilter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxFilter.Name = "groupBoxFilter";
            this.groupBoxFilter.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBoxFilter.Size = new System.Drawing.Size(472, 400);
            this.groupBoxFilter.TabIndex = 1;
            this.groupBoxFilter.TabStop = false;
            this.groupBoxFilter.Text = "Filter Options";
            // 
            // btnClearFilter
            // 
            this.btnClearFilter.Location = new System.Drawing.Point(112, 344);
            this.btnClearFilter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnClearFilter.Name = "btnClearFilter";
            this.btnClearFilter.Size = new System.Drawing.Size(158, 38);
            this.btnClearFilter.TabIndex = 18;
            this.btnClearFilter.Text = "Clear Filters";
            this.btnClearFilter.UseVisualStyleBackColor = true;
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            // 
            // chkSortDescending
            // 
            this.chkSortDescending.AutoSize = true;
            this.chkSortDescending.Location = new System.Drawing.Point(292, 296);
            this.chkSortDescending.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkSortDescending.Name = "chkSortDescending";
            this.chkSortDescending.Size = new System.Drawing.Size(120, 24);
            this.chkSortDescending.TabIndex = 17;
            this.chkSortDescending.Text = "Descending";
            this.chkSortDescending.UseVisualStyleBackColor = true;
            // 
            // lblSortBy
            // 
            this.lblSortBy.AutoSize = true;
            this.lblSortBy.Location = new System.Drawing.Point(17, 298);
            this.lblSortBy.Name = "lblSortBy";
            this.lblSortBy.Size = new System.Drawing.Size(65, 20);
            this.lblSortBy.TabIndex = 15;
            this.lblSortBy.Text = "Sort By:";
            // 
            // cmbSortBy
            // 
            this.cmbSortBy.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSortBy.FormattingEnabled = true;
            this.cmbSortBy.Items.AddRange(new object[] {
            "None",
            "Name",
            "Size",
            "Creation Time",
            "Modified Time",
            "Type"});
            this.cmbSortBy.Location = new System.Drawing.Point(112, 294);
            this.cmbSortBy.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbSortBy.Name = "cmbSortBy";
            this.cmbSortBy.Size = new System.Drawing.Size(157, 28);
            this.cmbSortBy.TabIndex = 16;
            // 
            // chkDirectoriesOnly
            // 
            this.chkDirectoriesOnly.AutoSize = true;
            this.chkDirectoriesOnly.Location = new System.Drawing.Point(169, 256);
            this.chkDirectoriesOnly.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkDirectoriesOnly.Name = "chkDirectoriesOnly";
            this.chkDirectoriesOnly.Size = new System.Drawing.Size(146, 24);
            this.chkDirectoriesOnly.TabIndex = 14;
            this.chkDirectoriesOnly.Text = "Directories Only";
            this.chkDirectoriesOnly.UseVisualStyleBackColor = true;
            this.chkDirectoriesOnly.CheckedChanged += new System.EventHandler(this.chkDirectoriesOnly_CheckedChanged);
            // 
            // chkFilesOnly
            // 
            this.chkFilesOnly.AutoSize = true;
            this.chkFilesOnly.Location = new System.Drawing.Point(17, 256);
            this.chkFilesOnly.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkFilesOnly.Name = "chkFilesOnly";
            this.chkFilesOnly.Size = new System.Drawing.Size(103, 24);
            this.chkFilesOnly.TabIndex = 13;
            this.chkFilesOnly.Text = "Files Only";
            this.chkFilesOnly.UseVisualStyleBackColor = true;
            this.chkFilesOnly.CheckedChanged += new System.EventHandler(this.chkFilesOnly_CheckedChanged);
            // 
            // lblModifiedAfter
            // 
            this.lblModifiedAfter.AutoSize = true;
            this.lblModifiedAfter.Location = new System.Drawing.Point(17, 221);
            this.lblModifiedAfter.Name = "lblModifiedAfter";
            this.lblModifiedAfter.Size = new System.Drawing.Size(112, 20);
            this.lblModifiedAfter.TabIndex = 11;
            this.lblModifiedAfter.Text = "Modified After:";
            // 
            // dateModifiedAfter
            // 
            this.dateModifiedAfter.CustomFormat = "yyyy-MM-dd";
            this.dateModifiedAfter.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateModifiedAfter.Location = new System.Drawing.Point(146, 219);
            this.dateModifiedAfter.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateModifiedAfter.Name = "dateModifiedAfter";
            this.dateModifiedAfter.Size = new System.Drawing.Size(303, 26);
            this.dateModifiedAfter.TabIndex = 12;
            // 
            // lblMaxSize
            // 
            this.lblMaxSize.AutoSize = true;
            this.lblMaxSize.Location = new System.Drawing.Point(17, 184);
            this.lblMaxSize.Name = "lblMaxSize";
            this.lblMaxSize.Size = new System.Drawing.Size(102, 20);
            this.lblMaxSize.TabIndex = 9;
            this.lblMaxSize.Text = "Max Size (B):";
            // 
            // numMaxSize
            // 
            this.numMaxSize.Location = new System.Drawing.Point(112, 181);
            this.numMaxSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numMaxSize.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numMaxSize.Name = "numMaxSize";
            this.numMaxSize.Size = new System.Drawing.Size(158, 26);
            this.numMaxSize.TabIndex = 10;
            // 
            // lblMinSize
            // 
            this.lblMinSize.AutoSize = true;
            this.lblMinSize.Location = new System.Drawing.Point(17, 146);
            this.lblMinSize.Name = "lblMinSize";
            this.lblMinSize.Size = new System.Drawing.Size(98, 20);
            this.lblMinSize.TabIndex = 7;
            this.lblMinSize.Text = "Min Size (B):";
            // 
            // numMinSize
            // 
            this.numMinSize.Location = new System.Drawing.Point(112, 144);
            this.numMinSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numMinSize.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.numMinSize.Name = "numMinSize";
            this.numMinSize.Size = new System.Drawing.Size(158, 26);
            this.numMinSize.TabIndex = 8;
            // 
            // lblNamePattern
            // 
            this.lblNamePattern.AutoSize = true;
            this.lblNamePattern.Location = new System.Drawing.Point(17, 110);
            this.lblNamePattern.Name = "lblNamePattern";
            this.lblNamePattern.Size = new System.Drawing.Size(65, 20);
            this.lblNamePattern.TabIndex = 5;
            this.lblNamePattern.Text = "Pattern:";
            // 
            // txtNamePattern
            // 
            this.txtNamePattern.Location = new System.Drawing.Point(112, 106);
            this.txtNamePattern.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtNamePattern.Name = "txtNamePattern";
            this.txtNamePattern.Size = new System.Drawing.Size(337, 26);
            this.txtNamePattern.TabIndex = 6;
            // 
            // lblExtensions
            // 
            this.lblExtensions.AutoSize = true;
            this.lblExtensions.Location = new System.Drawing.Point(17, 72);
            this.lblExtensions.Name = "lblExtensions";
            this.lblExtensions.Size = new System.Drawing.Size(91, 20);
            this.lblExtensions.TabIndex = 3;
            this.lblExtensions.Text = "Extensions:";
            // 
            // txtExtensions
            // 
            this.txtExtensions.Location = new System.Drawing.Point(112, 69);
            this.txtExtensions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtExtensions.Name = "txtExtensions";
            this.txtExtensions.Size = new System.Drawing.Size(337, 26);
            this.txtExtensions.TabIndex = 4;
            // 
            // lblMaxDepth
            // 
            this.lblMaxDepth.AutoSize = true;
            this.lblMaxDepth.Location = new System.Drawing.Point(202, 32);
            this.lblMaxDepth.Name = "lblMaxDepth";
            this.lblMaxDepth.Size = new System.Drawing.Size(137, 20);
            this.lblMaxDepth.TabIndex = 1;
            this.lblMaxDepth.Text = "Max Depth (0=all):";
            // 
            // numMaxDepth
            // 
            this.numMaxDepth.Enabled = false;
            this.numMaxDepth.Location = new System.Drawing.Point(338, 30);
            this.numMaxDepth.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numMaxDepth.Name = "numMaxDepth";
            this.numMaxDepth.Size = new System.Drawing.Size(112, 26);
            this.numMaxDepth.TabIndex = 2;
            // 
            // chkRecursive
            // 
            this.chkRecursive.AutoSize = true;
            this.chkRecursive.Location = new System.Drawing.Point(17, 31);
            this.chkRecursive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkRecursive.Name = "chkRecursive";
            this.chkRecursive.Size = new System.Drawing.Size(105, 24);
            this.chkRecursive.TabIndex = 0;
            this.chkRecursive.Text = "Recursive";
            this.chkRecursive.UseVisualStyleBackColor = true;
            this.chkRecursive.CheckedChanged += new System.EventHandler(this.chkRecursive_CheckedChanged);
            // 
            // txtPath
            // 
            this.txtPath.Enabled = false;
            this.txtPath.Location = new System.Drawing.Point(54, 9);
            this.txtPath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(350, 26);
            this.txtPath.TabIndex = 3;
            this.txtPath.Text = "/";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(8, 12);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(46, 20);
            this.lblPath.TabIndex = 2;
            this.lblPath.Text = "Path:";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Enabled = false;
            this.btnBrowse.Location = new System.Drawing.Point(410, 6);
            this.btnBrowse.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(87, 32);
            this.btnBrowse.TabIndex = 4;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // treeViewFiles
            // 
            this.treeViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewFiles.ImageIndex = 0;
            this.treeViewFiles.ImageList = this.imageList;
            this.treeViewFiles.Location = new System.Drawing.Point(0, 45);
            this.treeViewFiles.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.treeViewFiles.Name = "treeViewFiles";
            this.treeViewFiles.SelectedImageIndex = 0;
            this.treeViewFiles.Size = new System.Drawing.Size(600, 435);
            this.treeViewFiles.TabIndex = 5;
            // 
            // imageList
            // 
            this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
            this.imageList.ImageSize = new System.Drawing.Size(16, 16);
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus,
            this.lblFileCount,
            this.progressBar});
            this.statusStrip.Location = new System.Drawing.Point(0, 898);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(1230, 32);
            this.statusStrip.TabIndex = 6;
            this.statusStrip.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(119, 25);
            this.lblStatus.Text = "Disconnected";
            // 
            // lblFileCount
            // 
            this.lblFileCount.Name = "lblFileCount";
            this.lblFileCount.Size = new System.Drawing.Size(1094, 25);
            this.lblFileCount.Spring = true;
            this.lblFileCount.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // progressBar
            // 
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(169, 24);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar.Visible = false;
            // 
            // splitContainerMain
            // 
            this.splitContainerMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainerMain.Location = new System.Drawing.Point(14, 425);
            this.splitContainerMain.Name = "splitContainerMain";
            // 
            // splitContainerMain.Panel1
            // 
            this.splitContainerMain.Panel1.Controls.Add(this.treeViewFiles);
            this.splitContainerMain.Panel1.Controls.Add(this.panelSmbTop);
            // 
            // splitContainerMain.Panel2
            // 
            this.splitContainerMain.Panel2.Controls.Add(this.treeViewLocal);
            this.splitContainerMain.Panel2.Controls.Add(this.panelLocalTop);
            this.splitContainerMain.Size = new System.Drawing.Size(1200, 480);
            this.splitContainerMain.SplitterDistance = 600;
            this.splitContainerMain.TabIndex = 6;
            // 
            // panelSmbTop
            // 
            this.panelSmbTop.Controls.Add(this.lblPath);
            this.panelSmbTop.Controls.Add(this.txtPath);
            this.panelSmbTop.Controls.Add(this.btnBrowse);
            this.panelSmbTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSmbTop.Location = new System.Drawing.Point(0, 0);
            this.panelSmbTop.Name = "panelSmbTop";
            this.panelSmbTop.Size = new System.Drawing.Size(600, 45);
            this.panelSmbTop.TabIndex = 10;
            // 
            // treeViewLocal
            // 
            this.treeViewLocal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewLocal.ImageIndex = 0;
            this.treeViewLocal.ImageList = this.imageList;
            this.treeViewLocal.Location = new System.Drawing.Point(0, 45);
            this.treeViewLocal.Name = "treeViewLocal";
            this.treeViewLocal.SelectedImageIndex = 0;
            this.treeViewLocal.Size = new System.Drawing.Size(596, 435);
            this.treeViewLocal.TabIndex = 6;
            // 
            // panelLocalTop
            // 
            this.panelLocalTop.Controls.Add(this.lblLocalPath);
            this.panelLocalTop.Controls.Add(this.txtLocalPath);
            this.panelLocalTop.Controls.Add(this.btnLocalBrowse);
            this.panelLocalTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLocalTop.Location = new System.Drawing.Point(0, 0);
            this.panelLocalTop.Name = "panelLocalTop";
            this.panelLocalTop.Size = new System.Drawing.Size(596, 45);
            this.panelLocalTop.TabIndex = 11;
            // 
            // lblLocalPath
            // 
            this.lblLocalPath.AutoSize = true;
            this.lblLocalPath.Location = new System.Drawing.Point(8, 12);
            this.lblLocalPath.Name = "lblLocalPath";
            this.lblLocalPath.Size = new System.Drawing.Size(51, 20);
            this.lblLocalPath.TabIndex = 2;
            this.lblLocalPath.Text = "Local:";
            // 
            // txtLocalPath
            // 
            this.txtLocalPath.Location = new System.Drawing.Point(60, 9);
            this.txtLocalPath.Name = "txtLocalPath";
            this.txtLocalPath.Size = new System.Drawing.Size(350, 26);
            this.txtLocalPath.TabIndex = 3;
            // 
            // btnLocalBrowse
            // 
            this.btnLocalBrowse.Location = new System.Drawing.Point(416, 6);
            this.btnLocalBrowse.Name = "btnLocalBrowse";
            this.btnLocalBrowse.Size = new System.Drawing.Size(87, 32);
            this.btnLocalBrowse.TabIndex = 4;
            this.btnLocalBrowse.Text = "Go";
            this.btnLocalBrowse.UseVisualStyleBackColor = true;
            this.btnLocalBrowse.Click += new System.EventHandler(this.btnLocalBrowse_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1230, 930);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.splitContainerMain);
            this.Controls.Add(this.groupBoxFilter);
            this.Controls.Add(this.groupBoxConnection);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "Form1";
            this.Text = "SMB Client - File Browser";
            this.groupBoxConnection.ResumeLayout(false);
            this.groupBoxConnection.PerformLayout();
            this.groupBoxFilter.ResumeLayout(false);
            this.groupBoxFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMinSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxDepth)).EndInit();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.splitContainerMain.Panel1.ResumeLayout(false);
            this.splitContainerMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerMain)).EndInit();
            this.splitContainerMain.ResumeLayout(false);
            this.panelSmbTop.ResumeLayout(false);
            this.panelSmbTop.PerformLayout();
            this.panelLocalTop.ResumeLayout(false);
            this.panelLocalTop.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxConnection;
        private System.Windows.Forms.Button btnDisconnect;
        private System.Windows.Forms.Button btnConnect;
    private System.Windows.Forms.TextBox txtDomain;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtShareName;
        private System.Windows.Forms.TextBox txtServer;
  private System.Windows.Forms.Label lblDomain;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblShareName;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.GroupBox groupBoxFilter;
        private System.Windows.Forms.CheckBox chkSortDescending;
  private System.Windows.Forms.Label lblSortBy;
  private System.Windows.Forms.ComboBox cmbSortBy;
        private System.Windows.Forms.CheckBox chkDirectoriesOnly;
   private System.Windows.Forms.CheckBox chkFilesOnly;
        private System.Windows.Forms.Label lblModifiedAfter;
private System.Windows.Forms.DateTimePicker dateModifiedAfter;
    private System.Windows.Forms.Label lblMaxSize;
        private System.Windows.Forms.NumericUpDown numMaxSize;
        private System.Windows.Forms.Label lblMinSize;
 private System.Windows.Forms.NumericUpDown numMinSize;
        private System.Windows.Forms.Label lblNamePattern;
        private System.Windows.Forms.TextBox txtNamePattern;
        private System.Windows.Forms.Label lblExtensions;
 private System.Windows.Forms.TextBox txtExtensions;
        private System.Windows.Forms.Label lblMaxDepth;
        private System.Windows.Forms.NumericUpDown numMaxDepth;
        private System.Windows.Forms.CheckBox chkRecursive;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TreeView treeViewFiles;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStripStatusLabel lblFileCount;
        private System.Windows.Forms.ToolStripProgressBar progressBar;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.SplitContainer splitContainerMain;
        private System.Windows.Forms.Panel panelLocalTop;
        private System.Windows.Forms.TextBox txtLocalPath;
        private System.Windows.Forms.Button btnLocalBrowse;
        private System.Windows.Forms.Label lblLocalPath;
        private System.Windows.Forms.TreeView treeViewLocal;
        private System.Windows.Forms.Panel panelSmbTop;
    }
}

