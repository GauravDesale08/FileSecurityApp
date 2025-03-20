using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;

namespace SecureFileManager
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }

    public partial class MainForm : Form
    {
        private Form loginForm;
        private Panel contentPanel;
        private Panel navPanel;
        string logFilePath = @"C:\ProgramData\log.txt";

        public MainForm()
        {
            InitializeComponent();
            ShowLoginPage();
        }

        public void LogEvent(string level, string message, string source)
        {
            string timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"{timestamp} | {level} | {message} | {source}";

            try
            {
                using (StreamWriter sw = new StreamWriter(logFilePath, true)) // Append mode
                {
                    sw.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        private void InitializeComponent()
        {
            this.Text = "Secure File Manager";
            this.Size = new Size(1000, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Create content panel that will host different pages
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill
            };

            // Create navigation panel that will serve as left drawer
            navPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 200,
                BackColor = Color.FromArgb(60, 60, 60)
            };

            this.Controls.Add(contentPanel);

            // Handle form loading
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // We've removed the folder tree view initialization
        }

        private void ShowLoginPage()
        {
            // Clear content panel
            contentPanel.Controls.Clear();

            // Create login form
            loginForm = new Form
            {
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None,
                Dock = DockStyle.Fill
            };

            Panel loginPanel = new Panel
            {
                Width = 350,
                Height = 250,
                Location = new Point(
                    (contentPanel.Width - 350) / 2,
                    (contentPanel.Height - 250) / 2
                ),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            Label titleLabel = new Label
            {
                Text = "Login",
                Font = new Font("Arial", 16, FontStyle.Bold),
                Location = new Point(140, 20),
                AutoSize = true
            };

            Label usernameLabel = new Label
            {
                Text = "Username:",
                Location = new Point(60, 70),
                AutoSize = true
            };

            TextBox usernameTextBox = new TextBox
            {
                Location = new Point(150, 70),
                Width = 150
            };

            Label passwordLabel = new Label
            {
                Text = "Password:",
                Location = new Point(60, 110),
                AutoSize = true
            };

            TextBox passwordTextBox = new TextBox
            {
                Location = new Point(150, 110),
                Width = 150,
                PasswordChar = '*'
            };

            Button loginButton = new Button
            {
                Text = "Login",
                Location = new Point(130, 160),
                Width = 100,
                Height = 30
            };

            loginButton.Click += (sender, e) => {
                // Simple authentication for demo
                if (usernameTextBox.Text == "admin" && passwordTextBox.Text == "password")
                {
                    ShowMainPage();
                }
                else
                {
                    MessageBox.Show("Invalid credentials. Please try again.");
                }
            };

            loginPanel.Controls.Add(titleLabel);
            loginPanel.Controls.Add(usernameLabel);
            loginPanel.Controls.Add(usernameTextBox);
            loginPanel.Controls.Add(passwordLabel);
            loginPanel.Controls.Add(passwordTextBox);
            loginPanel.Controls.Add(loginButton);

            loginForm.Controls.Add(loginPanel);
            contentPanel.Controls.Add(loginForm);
            loginForm.Show();

            // Adjust login panel position when form resizes
            contentPanel.Resize += (sender, e) => {
                loginPanel.Location = new Point(
                    (contentPanel.Width - loginPanel.Width) / 2,
                    (contentPanel.Height - loginPanel.Height) / 2
                );
            };
        }

        private void ShowMainPage()
        {
            // Clear the content panel
            contentPanel.Controls.Clear();

            // First add content panel
            contentPanel.BringToFront();

            // Then add navigation panel
            this.Controls.Add(navPanel);

            // Create navigation buttons in the left drawer
            CreateNavigationButtons();

            // By default, show logs page
            ShowLogsPage();
        }

        private void CreateNavigationButtons()
        {
            // Clear existing controls
            navPanel.Controls.Clear();

            // Add app title
            Label appTitle = new Label
            {
                Text = "Secure File Manager",
                ForeColor = Color.White,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Location = new Point(10, 20),
                AutoSize = true
            };
            navPanel.Controls.Add(appTitle);

            // Add navigation buttons
            Button secureFolderButton = CreateNavButton("My Secure Folder", 70); // New button
            Button logsButton = CreateNavButton("Logs", 110);
            Button integrityButton = CreateNavButton("Integrity/Hashing", 150);
            Button encryptionButton = CreateNavButton("Encryption", 190);

            // Event handlers for buttons
            secureFolderButton.Click += (sender, e) => ShowSecureFolderPage(); // New event handler
            logsButton.Click += (sender, e) => ShowLogsPage();
            integrityButton.Click += (sender, e) => ShowIntegrityPage();
            encryptionButton.Click += (sender, e) => ShowEncryptionPage();

            // Add buttons to the navigation panel
            navPanel.Controls.Add(secureFolderButton);
            navPanel.Controls.Add(logsButton);
            navPanel.Controls.Add(integrityButton);
            navPanel.Controls.Add(encryptionButton);

            // Add logout button at bottom
            Button logoutButton = CreateNavButton("Logout", navPanel.Height - 50);
            logoutButton.Click += (sender, e) => {
                navPanel.Visible = false;
                ShowLoginPage();
            };
            navPanel.Controls.Add(logoutButton);

            // Handle resize to keep logout button at bottom
            navPanel.Resize += (sender, e) => {
                logoutButton.Location = new Point(logoutButton.Location.X, navPanel.Height - 50);
            };
        }

        private Button CreateNavButton(string text, int yPos)
        {
            return new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                Location = new Point(0, yPos),
                Size = new Size(200, 40),
                TextAlign = ContentAlignment.MiddleLeft,
                ImageAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
        }

        private Button viewFolderButton;
        private string secureFolderPath = @"C:\ProgramData\SecureFiles"; // Hidden Secure Folder Path
        private ListView secureFolderList;
        private FileSystemWatcher fileWatcher;

        private void ShowSecureFolderPage()
        {
            Panel contentArea = GetContentArea();
            contentArea.BackColor = Color.FromArgb(240, 240, 245);

            Label pageTitle = new Label
            {
                Text = "My Secure Folder",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Initialize ListView
            secureFolderList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 60),
                Size = new Size(contentArea.Width - 40, contentArea.Height - 130)
            };

            // Adding columns
            secureFolderList.Columns.Add("File Name", 200);
            secureFolderList.Columns.Add("Size", 100);
            secureFolderList.Columns.Add("Last Modified", 150);
            secureFolderList.Columns.Add("Creation Time", 150);  // ✅ Added Column
            secureFolderList.Columns.Add("Last Accessed", 150);  // ✅ Added Column

            // Initialize "View Secure Folder" button
            viewFolderButton = new Button
            {
                Text = "View Secure Folder",
                Location = new Point(20, secureFolderList.Bottom + 10),
                Size = new Size(200, 30)
            };

            viewFolderButton.Click += ViewSecureFolder; // Attach event

            contentArea.Controls.Add(pageTitle);
            contentArea.Controls.Add(secureFolderList);
            contentArea.Controls.Add(viewFolderButton);

            // Adjust UI on resize
            contentArea.Resize += (sender, e) =>
            {
                secureFolderList.Size = new Size(contentArea.Width - 40, contentArea.Height - 130);
                viewFolderButton.Location = new Point(20, secureFolderList.Bottom + 10);
            };

            // Load existing files and start monitoring
            LoadSecureFolderFiles();
            StartFolderMonitoring();
        }

        // Load existing files in the secure folder
        private void LoadSecureFolderFiles()
        {
            secureFolderList.Items.Clear(); // Clear previous items

            if (!Directory.Exists(secureFolderPath))
            {
                Directory.CreateDirectory(secureFolderPath); // Ensure folder exists
            }

            DirectoryInfo dirInfo = new DirectoryInfo(secureFolderPath);
            foreach (FileInfo file in dirInfo.GetFiles())
            {
                string[] row = {
            file.Name,
            (file.Length / 1024.0).ToString("F2") + " KB",
            file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
            file.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),  // ✅ New Column Data
            file.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss") // ✅ New Column Data
        };
                secureFolderList.Items.Add(new ListViewItem(row));
            }
        }

        // Monitor secure folder for real-time changes
        private void StartFolderMonitoring()
        {
            fileWatcher = new FileSystemWatcher
            {
                Path = secureFolderPath,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                Filter = "*.*", // Watch all file types
                IncludeSubdirectories = true, // Monitor subfolders as well
                EnableRaisingEvents = true
            };

            // Attach event handlers for file changes
            fileWatcher.Created += OnFolderChanged;
            fileWatcher.Deleted += OnFolderChanged;
            fileWatcher.Changed += OnFolderChanged;
            fileWatcher.Renamed += OnFileRenamed;
        }

        // Handles file created, modified, or deleted events
        private void OnFolderChanged(object sender, FileSystemEventArgs e)
        {
            LogEvent("INFO", $"File {e.ChangeType}: {e.FullPath}", "FileSystem");
            secureFolderList.Invoke((MethodInvoker)(() =>
            {
                LoadSecureFolderFiles(); // Refresh file list
            }));
        }

        // Handles file rename event
        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            LogEvent("INFO", $"File renamed to {e.FullPath}", "FileSystem");
            secureFolderList.Invoke((MethodInvoker)(() =>
            {
                LoadSecureFolderFiles(); // Refresh file list
            }));
        }

        // Open secure folder in File Explorer
        private void ViewSecureFolder(object sender, EventArgs e)
        {
            if (!Directory.Exists(secureFolderPath))
            {
                Directory.CreateDirectory(secureFolderPath);
                ProcessStartInfo psi = new ProcessStartInfo("cmd.exe");
                psi.Arguments = $"/c attrib +s +h \"{secureFolderPath}\"";
                Process.Start(psi);
                System.Diagnostics.Process.Start("explorer.exe", secureFolderPath);
            }
            else
            {
                System.Diagnostics.Process.Start("explorer.exe", secureFolderPath);
            }
        }

        private void ShowLogsPage()
        {
            // Create and show logs page in content panel
            Panel contentArea = GetContentArea();
            contentArea.BackColor = Color.FromArgb(240, 240, 245);

            Label pageTitle = new Label
            {
                Text = "System Logs",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            ListView logsList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 60),
                Size = new Size(contentArea.Width - 40, contentArea.Height - 80)
            };

            logsList.Columns.Add("Timestamp", 150);
            logsList.Columns.Add("Level", 80);
            logsList.Columns.Add("Message", 350);
            logsList.Columns.Add("Source", 120);

            // Load logs from file
            if (!File.Exists(logFilePath)) // Check if log file exists
            {
                try
                {
                    File.WriteAllText(logFilePath, ""); // Create empty log file
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error creating log file: {ex.Message}", "Log Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return; // Exit if file cannot be created
                }
            }

            try
            {
                foreach (string log in File.ReadAllLines(logFilePath))
                {
                    string[] parts = log.Split('|'); // Log format: "timestamp | level | message | source"

                    if (parts.Length == 4) // Ensure correct format
                    {
                        ListViewItem item = new ListViewItem(parts);
                        logsList.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error reading log file: {ex.Message}", "Log Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            contentArea.Controls.Add(pageTitle);
            contentArea.Controls.Add(logsList);

            // Adjust log list size when content area resizes
            contentArea.Resize += (sender, e) => {
                logsList.Size = new Size(contentArea.Width - 40, contentArea.Height - 80);
            };
        }

        private void ShowIntegrityPage()
        {
            // Create and show integrity/hashing page in content panel
            Panel contentArea = GetContentArea();
            contentArea.BackColor = Color.FromArgb(240, 240, 245);

            Label pageTitle = new Label
            {
                Text = "File Integrity & Hashing",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label fileLabel = new Label
            {
                Text = "Select File:",
                Location = new Point(20, 70),
                AutoSize = true
            };

            TextBox fileTextBox = new TextBox
            {
                Location = new Point(120, 70),
                Width = 350,
                ReadOnly = true
            };

            Button browseButton = new Button
            {
                Text = "Browse",
                Location = new Point(480, 69),
                Width = 80
            };

            Label hashTypeLabel = new Label
            {
                Text = "Hash Type:",
                Location = new Point(20, 110),
                AutoSize = true
            };

            ComboBox hashTypeComboBox = new ComboBox
            {
                Location = new Point(120, 110),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            hashTypeComboBox.Items.AddRange(new object[] { "MD5", "SHA-1", "SHA-256", "SHA-512" });
            hashTypeComboBox.SelectedIndex = 2; // Default to SHA-256

            Button calculateButton = new Button
            {
                Text = "Calculate Hash",
                Location = new Point(120, 150),
                Width = 150
            };

            Label resultTitleLabel = new Label
            {
                Text = "Hash Result:",
                Location = new Point(20, 190),
                AutoSize = true
            };

            TextBox resultTextBox = new TextBox
            {
                Location = new Point(120, 190),
                Width = 440,
                ReadOnly = true,
                Multiline = true,
                Height = 60
            };

            GroupBox verifyGroupBox = new GroupBox
            {
                Text = "Verify Hash",
                Location = new Point(20, 270),
                Width = 540,
                Height = 120
            };

            Label compareHashLabel = new Label
            {
                Text = "Expected Hash:",
                Location = new Point(20, 30),
                AutoSize = true
            };

            TextBox compareHashTextBox = new TextBox
            {
                Location = new Point(120, 30),
                Width = 390
            };

            Button verifyButton = new Button
            {
                Text = "Verify",
                Location = new Point(120, 70),
                Width = 150
            };

            verifyGroupBox.Controls.Add(compareHashLabel);
            verifyGroupBox.Controls.Add(compareHashTextBox);
            verifyGroupBox.Controls.Add(verifyButton);

            browseButton.Click += (sender, e) => {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = @"C:\ProgramData\SecureFiles"; // Secure Folder Path
                    openFileDialog.Filter = "All Files|*.*"; // Allow all files (you can modify this)
                    openFileDialog.Title = "Select a file from Secure Folder";

                    // Restrict navigation outside the folder
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedPath = openFileDialog.FileName;

                        // Ensure the selected file is inside the secure folder
                        if (selectedPath.StartsWith(@"C:\ProgramData\SecureFiles"))
                        {
                            fileTextBox.Text = selectedPath; // Show selected file path
                        }
                        else
                        {
                            MessageBox.Show("You can only select files from the Secure Folder!", "Access Denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                    }
                }
            };

            calculateButton.Click += (sender, e) => {
                if (string.IsNullOrEmpty(fileTextBox.Text) || !File.Exists(fileTextBox.Text))
                {
                    MessageBox.Show("Please select a valid file.");
                    return;
                }

                try
                {
                    string hashString = CalculateFileHash(fileTextBox.Text, hashTypeComboBox.SelectedItem.ToString());
                    resultTextBox.Text = hashString;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error calculating hash: " + ex.Message);
                }
            };

            verifyButton.Click += (sender, e) => {
                if (string.IsNullOrEmpty(resultTextBox.Text) || string.IsNullOrEmpty(compareHashTextBox.Text))
                {
                    MessageBox.Show("Please calculate a hash and enter an expected hash value.");
                    return;
                }

                if (resultTextBox.Text.Equals(compareHashTextBox.Text, StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Hash verification successful! Files match.", "Verification", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Hash verification failed! Files do not match.", "Verification", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            contentArea.Controls.Add(pageTitle);
            contentArea.Controls.Add(fileLabel);
            contentArea.Controls.Add(fileTextBox);
            contentArea.Controls.Add(browseButton);
            contentArea.Controls.Add(hashTypeLabel);
            contentArea.Controls.Add(hashTypeComboBox);
            contentArea.Controls.Add(calculateButton);
            contentArea.Controls.Add(resultTitleLabel);
            contentArea.Controls.Add(resultTextBox);
            contentArea.Controls.Add(verifyGroupBox);
        }

        private string CalculateFileHash(string filePath, string hashType)
        {
            using (FileStream stream = File.OpenRead(filePath))
            {
                HashAlgorithm hashAlgorithm = null;

                switch (hashType)
                {
                    case "MD5":
                        hashAlgorithm = MD5.Create();
                        break;
                    case "SHA-1":
                        hashAlgorithm = SHA1.Create();
                        break;
                    case "SHA-256":
                        hashAlgorithm = SHA256.Create();
                        break;
                    case "SHA-512":
                        hashAlgorithm = SHA512.Create();
                        break;
                    default:
                        hashAlgorithm = SHA256.Create();
                        break;
                }

                byte[] hashBytes = hashAlgorithm.ComputeHash(stream);
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private void ShowEncryptionPage()
        {
            // Create and show encryption page in content panel
            Panel contentArea = GetContentArea();
            contentArea.BackColor = Color.FromArgb(240, 240, 245);

            Label pageTitle = new Label
            {
                Text = "File Encryption",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            TabControl tabControl = new TabControl
            {
                Location = new Point(20, 60),
                Size = new Size(contentArea.Width - 40, contentArea.Height - 80)
            };

            // Encrypt Tab
            TabPage encryptTab = new TabPage("Encrypt");

            Label fileToEncryptLabel = new Label
            {
                Text = "Select File:",
                Location = new Point(20, 30),
                AutoSize = true
            };

            TextBox fileToEncryptTextBox = new TextBox
            {
                Location = new Point(130, 30),
                Width = 300,
                ReadOnly = true
            };

            Button browsEncryptFileButton = new Button
            {
                Text = "Browse",
                Location = new Point(440, 29),
                Width = 80
            };

            Label outputEncryptLabel = new Label
            {
                Text = "Output File:",
                Location = new Point(20, 70),
                AutoSize = true
            };

            TextBox outputEncryptTextBox = new TextBox
            {
                Location = new Point(130, 70),
                Width = 300,
                ReadOnly = true
            };

            Button browseOutputEncryptButton = new Button
            {
                Text = "Browse",
                Location = new Point(440, 69),
                Width = 80
            };

            Label passwordEncryptLabel = new Label
            {
                Text = "Password:",
                Location = new Point(20, 110),
                AutoSize = true
            };

            TextBox passwordEncryptTextBox = new TextBox
            {
                Location = new Point(130, 110),
                Width = 250,
                PasswordChar = '*'
            };

            CheckBox showPassEncryptCheckBox = new CheckBox
            {
                Text = "Show",
                Location = new Point(390, 110),
                AutoSize = true
            };

            Label confirmEncryptLabel = new Label
            {
                Text = "Confirm:",
                Location = new Point(20, 150),
                AutoSize = true
            };

            TextBox confirmEncryptTextBox = new TextBox
            {
                Location = new Point(130, 150),
                Width = 250,
                PasswordChar = '*'
            };

            Label algorithmEncryptLabel = new Label
            {
                Text = "Algorithm:",
                Location = new Point(20, 190),
                AutoSize = true
            };

            ComboBox algorithmEncryptCombo = new ComboBox
            {
                Location = new Point(130, 190),
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            algorithmEncryptCombo.Items.AddRange(new object[] { "AES-256", "Triple DES" });
            algorithmEncryptCombo.SelectedIndex = 0;

            Button encryptButton = new Button
            {
                Text = "Encrypt File",
                Location = new Point(130, 230),
                Width = 150,
                Height = 35
            };

            Label encryptStatusLabel = new Label
            {
                Text = "",
                Location = new Point(130, 280),
                AutoSize = true
            };

            // Decrypt Tab
            TabPage decryptTab = new TabPage("Decrypt");

            Label fileToDecryptLabel = new Label
            {
                Text = "Select File:",
                Location = new Point(20, 30),
                AutoSize = true
            };

            TextBox fileToDecryptTextBox = new TextBox
            {
                Location = new Point(130, 30),
                Width = 300,
                ReadOnly = true
            };

            Button browseDecryptFileButton = new Button
            {
                Text = "Browse",
                Location = new Point(440, 29),
                Width = 80
            };

            Label outputDecryptLabel = new Label
            {
                Text = "Output File:",
                Location = new Point(20, 70),
                AutoSize = true
            };

            TextBox outputDecryptTextBox = new TextBox
            {
                Location = new Point(130, 70),
                Width = 300,
                ReadOnly = true
            };

            Button browseOutputDecryptButton = new Button
            {
                Text = "Browse",
                Location = new Point(440, 69),
                Width = 80
            };

            Label passwordDecryptLabel = new Label
            {
                Text = "Password:",
                Location = new Point(20, 110),
                AutoSize = true
            };

            TextBox passwordDecryptTextBox = new TextBox
            {
                Location = new Point(130, 110),
                Width = 250,
                PasswordChar = '*'
            };

            CheckBox showPassDecryptCheckBox = new CheckBox
            {
                Text = "Show",
                Location = new Point(390, 110),
                AutoSize = true
            };

            Button decryptButton = new Button
            {
                Text = "Decrypt File",
                Location = new Point(130, 150),
                Width = 150,
                Height = 35
            };

            Label decryptStatusLabel = new Label
            {
                Text = "",
                Location = new Point(130, 200),
                AutoSize = true
            };

            // Add controls to encrypt tab
            encryptTab.Controls.Add(fileToEncryptLabel);
            encryptTab.Controls.Add(fileToEncryptTextBox);
            encryptTab.Controls.Add(browsEncryptFileButton);
            encryptTab.Controls.Add(outputEncryptLabel);
            encryptTab.Controls.Add(outputEncryptTextBox);
            encryptTab.Controls.Add(browseOutputEncryptButton);
            encryptTab.Controls.Add(passwordEncryptLabel);
            encryptTab.Controls.Add(passwordEncryptTextBox);
            encryptTab.Controls.Add(showPassEncryptCheckBox);
            encryptTab.Controls.Add(confirmEncryptLabel);
            encryptTab.Controls.Add(confirmEncryptTextBox);
            encryptTab.Controls.Add(algorithmEncryptLabel);
            encryptTab.Controls.Add(algorithmEncryptCombo);
            encryptTab.Controls.Add(encryptButton);
            encryptTab.Controls.Add(encryptStatusLabel);

            // Add controls to decrypt tab
            decryptTab.Controls.Add(fileToDecryptLabel);
            decryptTab.Controls.Add(fileToDecryptTextBox);
            decryptTab.Controls.Add(browseDecryptFileButton);
            decryptTab.Controls.Add(outputDecryptLabel);
            decryptTab.Controls.Add(outputDecryptTextBox);
            decryptTab.Controls.Add(browseOutputDecryptButton);
            decryptTab.Controls.Add(passwordDecryptLabel);
            decryptTab.Controls.Add(passwordDecryptTextBox);
            decryptTab.Controls.Add(showPassDecryptCheckBox);
            decryptTab.Controls.Add(decryptButton);
            decryptTab.Controls.Add(decryptStatusLabel);

            // Add tabs to tab control
            tabControl.TabPages.Add(encryptTab);
            tabControl.TabPages.Add(decryptTab);

            // Wire up events
            browsEncryptFileButton.Click += (sender, e) => {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileToEncryptTextBox.Text = openFileDialog.FileName;
                        outputEncryptTextBox.Text = openFileDialog.FileName + ".encrypted";
                    }
                }
            };

            browseOutputEncryptButton.Click += (sender, e) => {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Encrypted Files|*.encrypted|All Files|*.*";
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        outputEncryptTextBox.Text = saveFileDialog.FileName;
                    }
                }
            };

            browseDecryptFileButton.Click += (sender, e) => {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Encrypted Files|*.encrypted|All Files|*.*";
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileToDecryptTextBox.Text = openFileDialog.FileName;
                        string decryptedName = openFileDialog.FileName.EndsWith(".encrypted") ?
                            openFileDialog.FileName.Substring(0, openFileDialog.FileName.Length - 10) :
                            openFileDialog.FileName + ".decrypted";
                        outputDecryptTextBox.Text = decryptedName;
                    }
                }
            };

            browseOutputDecryptButton.Click += (sender, e) => {
                using (SaveFileDialog saveFileDialog = new SaveFileDialog())
                {
                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        outputDecryptTextBox.Text = saveFileDialog.FileName;
                    }
                }
            };

            showPassEncryptCheckBox.CheckedChanged += (sender, e) => {
                passwordEncryptTextBox.PasswordChar = showPassEncryptCheckBox.Checked ? '\0' : '*';
                confirmEncryptTextBox.PasswordChar = showPassEncryptCheckBox.Checked ? '\0' : '*';
            };

            showPassDecryptCheckBox.CheckedChanged += (sender, e) => {
                passwordDecryptTextBox.PasswordChar = showPassDecryptCheckBox.Checked ? '\0' : '*';
            };

            encryptButton.Click += (sender, e) => {
                if (string.IsNullOrEmpty(fileToEncryptTextBox.Text) ||
                    string.IsNullOrEmpty(outputEncryptTextBox.Text) ||
                    string.IsNullOrEmpty(passwordEncryptTextBox.Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }
                if (passwordEncryptTextBox.Text != confirmEncryptTextBox.Text)
                {
                    MessageBox.Show("Passwords do not match.");
                    return;
                }
                try
                {
                    string algorithm = algorithmEncryptCombo.SelectedItem.ToString();
                    if (algorithm == "AES-256")
                    {
                        EncryptFileAES(fileToEncryptTextBox.Text, outputEncryptTextBox.Text, passwordEncryptTextBox.Text);
                    }
                    else if (algorithm == "Triple DES")
                    {
                        EncryptFileTripleDES(fileToEncryptTextBox.Text, outputEncryptTextBox.Text, passwordEncryptTextBox.Text);
                    }
                    encryptStatusLabel.Text = "Encryption successful! Original file deleted.";
                    encryptStatusLabel.ForeColor = Color.Green;
                    MessageBox.Show("File encrypted successfully! Original file deleted.");
                }
                catch (Exception ex)
                {
                    encryptStatusLabel.Text = "Encryption failed: " + ex.Message;
                    encryptStatusLabel.ForeColor = Color.Red;
                }
            };

            decryptButton.Click += (sender, e) => {
                if (string.IsNullOrEmpty(fileToDecryptTextBox.Text) ||
                    string.IsNullOrEmpty(outputDecryptTextBox.Text) ||
                    string.IsNullOrEmpty(passwordDecryptTextBox.Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }
                try
                {
                    string algorithm = algorithmEncryptCombo.SelectedItem.ToString();
                    if (algorithm == "AES-256")
                    {
                        DecryptFileAES(fileToDecryptTextBox.Text, outputDecryptTextBox.Text, passwordDecryptTextBox.Text);
                    }
                    else if (algorithm == "Triple DES")
                    {
                        DecryptFileTripleDES(fileToDecryptTextBox.Text, outputDecryptTextBox.Text, passwordDecryptTextBox.Text);
                    }
                    decryptStatusLabel.Text = "Decryption successful! Encrypted file deleted.";
                    decryptStatusLabel.ForeColor = Color.Green;
                    MessageBox.Show("File decrypted successfully! Encrypted file deleted.");
                }
                catch (Exception ex)
                {
                    decryptStatusLabel.Text = "Decryption failed: " + ex.Message;
                    decryptStatusLabel.ForeColor = Color.Red;
                }
            };

            contentArea.Controls.Add(pageTitle);
            contentArea.Controls.Add(tabControl);

            // Adjust tab control size when content area resizes
            contentArea.Resize += (sender, e) => {
                tabControl.Size = new Size(contentArea.Width - 40, contentArea.Height - 80);
            };
        }

        private Panel GetContentArea()
        {
            // Clear current content area
            foreach (Control control in contentPanel.Controls)
            {
                if (control != loginForm)
                {
                    control.Dispose();
                }
            }

            // Create a new panel for content
            Panel contentArea = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(navPanel.Width, 0, 0, 0)  // This ensures content doesn't show behind nav panel
            };

            contentPanel.Controls.Add(contentArea);
            return contentArea;
        }

        // Encrypt a file using AES-256
        private void EncryptFileAES(string inputFile, string outputFile, string password)
        {
            try
            {
                // Generate a key and IV from the password
                byte[] key = GenerateKeyAES(password);
                byte[] iv = GenerateIV();

                // Create AES encryption algorithm
                using (Aes aes = Aes.Create())
                {
                    aes.Key = key;
                    aes.IV = iv;

                    // Create output file and write the IV (needed for decryption)
                    using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                    {
                        outputStream.Write(iv, 0, iv.Length); // Write IV to the beginning of the file

                        // Encrypt the file
                        using (CryptoStream cryptoStream = new CryptoStream(outputStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                        {
                            inputStream.CopyTo(cryptoStream);
                        }
                    }
                }

                LogEvent("INFO", $"File encrypted using AES: {inputFile}", "Encryption");
            }
            catch (Exception ex)
            {
                LogEvent("ERROR", $"AES Encryption failed: {ex.Message}", "Encryption");
                throw;
            }
        }

        // Decrypt a file using AES-256
        private void DecryptFileAES(string inputFile, string outputFile, string password)
        {
            try
            {
                // Generate a key from the password
                byte[] key = GenerateKeyAES(password);

                // Read the IV from the beginning of the file
                byte[] iv = new byte[16]; // AES IV is always 16 bytes
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                {
                    inputStream.Read(iv, 0, iv.Length); // Read IV from the file

                    // Create AES decryption algorithm
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;

                        // Decrypt the file
                        using (CryptoStream cryptoStream = new CryptoStream(inputStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                        {
                            cryptoStream.CopyTo(outputStream);
                        }
                    }
                }

                LogEvent("INFO", $"File decrypted using AES: {inputFile}", "Decryption");
            }
            catch (Exception ex)
            {
                LogEvent("ERROR", $"AES Decryption failed: {ex.Message}", "Decryption");
                throw;
            }
        }

        // Generate a 256-bit key from a password for AES
        private byte[] GenerateKeyAES(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        // Encrypt a file using Triple DES
        private void EncryptFileTripleDES(string inputFile, string outputFile, string password)
        {
            try
            {
                // Generate a key and IV from the password
                byte[] key = GenerateKeyTripleDES(password);
                byte[] iv = GenerateIV();

                // Create Triple DES encryption algorithm
                using (TripleDES tripleDes = TripleDES.Create())
                {
                    tripleDes.Key = key;
                    tripleDes.IV = iv;

                    // Create output file and write the IV (needed for decryption)
                    using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                    {
                        outputStream.Write(iv, 0, iv.Length); // Write IV to the beginning of the file

                        // Encrypt the file
                        using (CryptoStream cryptoStream = new CryptoStream(outputStream, tripleDes.CreateEncryptor(), CryptoStreamMode.Write))
                        using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                        {
                            inputStream.CopyTo(cryptoStream);
                        }
                    }
                }

                LogEvent("INFO", $"File encrypted using Triple DES: {inputFile}", "Encryption");
            }
            catch (Exception ex)
            {
                LogEvent("ERROR", $"Triple DES Encryption failed: {ex.Message}", "Encryption");
                throw;
            }
        }

        // Decrypt a file using Triple DES
        private void DecryptFileTripleDES(string inputFile, string outputFile, string password)
        {
            try
            {
                // Generate a key from the password
                byte[] key = GenerateKeyTripleDES(password);

                // Read the IV from the beginning of the file
                byte[] iv = new byte[8]; // Triple DES IV is 8 bytes
                using (FileStream inputStream = new FileStream(inputFile, FileMode.Open))
                {
                    inputStream.Read(iv, 0, iv.Length); // Read IV from the file

                    // Create Triple DES decryption algorithm
                    using (TripleDES tripleDes = TripleDES.Create())
                    {
                        tripleDes.Key = key;
                        tripleDes.IV = iv;

                        // Decrypt the file
                        using (CryptoStream cryptoStream = new CryptoStream(inputStream, tripleDes.CreateDecryptor(), CryptoStreamMode.Read))
                        using (FileStream outputStream = new FileStream(outputFile, FileMode.Create))
                        {
                            cryptoStream.CopyTo(outputStream);
                        }
                    }
                }

                LogEvent("INFO", $"File decrypted using Triple DES: {inputFile}", "Decryption");
            }
            catch (Exception ex)
            {
                LogEvent("ERROR", $"Triple DES Decryption failed: {ex.Message}", "Decryption");
                throw;
            }
        }

        // Generate a 192-bit key from a password for Triple DES
        private byte[] GenerateKeyTripleDES(string password)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(password));
                byte[] key = new byte[24]; // Triple DES requires a 192-bit key (24 bytes)
                Array.Copy(hash, 0, key, 0, 16); // Copy first 16 bytes
                Array.Copy(hash, 0, key, 16, 8); // Repeat first 8 bytes to make 24 bytes
                return key;
            }
        }

        // Generate a random IV (Initialization Vector)
        private byte[] GenerateIV()
        {
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] iv = new byte[16]; // AES uses 16 bytes, Triple DES uses 8 bytes
                rng.GetBytes(iv);
                return iv;
            }
        }
    }
}