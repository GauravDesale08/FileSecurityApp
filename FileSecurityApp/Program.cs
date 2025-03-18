using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SecureFileManager  // Make sure this matches your project namespace
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
}




namespace SecureFileManager
{
    public partial class MainForm : Form
    {
        private Form loginForm;
        private Panel contentPanel;
        private Panel navPanel;

        public MainForm()
        {
            InitializeComponent();
            ShowLoginPage();
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

        private void ShowSecureFolderPage()
        {
            // Create and show the secure folder page in the content panel
            Panel contentArea = GetContentArea();
            contentArea.BackColor = Color.FromArgb(240, 240, 245);

            Label pageTitle = new Label
            {
                Text = "My Secure Folder",
                Font = new Font("Arial", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Add a ListView to display files in the secure folder
            ListView secureFolderList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                Location = new Point(20, 60),
                Size = new Size(contentArea.Width - 40, contentArea.Height - 80)
            };

            secureFolderList.Columns.Add("File Name", 300);
            secureFolderList.Columns.Add("Size", 100);
            secureFolderList.Columns.Add("Last Modified", 150);

            // Add sample files (replace with actual file loading logic)
            string[] sampleFiles = {
        "Document1.txt|1.2 MB|2025-03-16 08:30:22",
        "Image1.jpg|3.5 MB|2025-03-15 12:45:10",
        "Report.pdf|2.0 MB|2025-03-14 17:20:35"
    };

            foreach (string file in sampleFiles)
            {
                string[] parts = file.Split('|');
                ListViewItem item = new ListViewItem(parts);
                secureFolderList.Items.Add(item);
            }

            // Add controls to the content area
            contentArea.Controls.Add(pageTitle);
            contentArea.Controls.Add(secureFolderList);

            // Adjust list view size when content area resizes
            contentArea.Resize += (sender, e) => {
                secureFolderList.Size = new Size(contentArea.Width - 40, contentArea.Height - 80);
            };
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

            // Add sample log data
            string[] sampleLogs = {
                "2025-03-16 08:30:22|INFO|Application started|System",
                "2025-03-16 08:31:05|INFO|User authentication successful|Authentication",
                "2025-03-16 08:35:18|WARNING|Low disk space detected|Storage",
                "2025-03-16 08:42:33|ERROR|Database connection failed|Database",
                "2025-03-16 09:15:07|INFO|New file uploaded|FileSystem",
                "2025-03-16 09:22:41|INFO|File encryption complete|Security",
                "2025-03-16 09:30:55|WARNING|Network connectivity issues|Network"
            };

            foreach (string log in sampleLogs)
            {
                string[] parts = log.Split('|');
                ListViewItem item = new ListViewItem(parts);
                logsList.Items.Add(item);
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
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        fileTextBox.Text = openFileDialog.FileName;
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

            ProgressBar encryptProgressBar = new ProgressBar
            {
                Location = new Point(130, 230),
                Width = 300,
                Height = 20,
                Visible = false
            };

            Button encryptButton = new Button
            {
                Text = "Encrypt File",
                Location = new Point(130, 260),
                Width = 150,
                Height = 35
            };

            Label encryptStatusLabel = new Label
            {
                Text = "",
                Location = new Point(130, 310),
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

            ProgressBar decryptProgressBar = new ProgressBar
            {
                Location = new Point(130, 150),
                Width = 300,
                Height = 20,
                Visible = false
            };

            Button decryptButton = new Button
            {
                Text = "Decrypt File",
                Location = new Point(130, 180),
                Width = 150,
                Height = 35
            };

            Label decryptStatusLabel = new Label
            {
                Text = "",
                Location = new Point(130, 230),
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
            encryptTab.Controls.Add(encryptProgressBar);
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
            decryptTab.Controls.Add(decryptProgressBar);
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

            // Modify the encrypt button click handler
            encryptButton.Click += async (sender, e) => {
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

                if (!File.Exists(fileToEncryptTextBox.Text))
                {
                    MessageBox.Show("Source file does not exist.");
                    return;
                }

                // Store values from UI before starting the task
                string sourceFile = fileToEncryptTextBox.Text;
                string destFile = outputEncryptTextBox.Text;
                string password = passwordEncryptTextBox.Text;
                string algorithm = algorithmEncryptCombo.SelectedItem.ToString();

                try
                {
                    encryptButton.Enabled = false;
                    encryptStatusLabel.Text = "Encrypting...";
                    encryptStatusLabel.ForeColor = Color.Blue;
                    encryptProgressBar.Visible = true;
                    encryptProgressBar.Value = 0;

                    // Synchronized context for UI updates
                    var uiContext = SynchronizationContext.Current;

                    // Run encryption on background thread
                    await Task.Run(() => {
                        try
                        {
                            EncryptFile(sourceFile, destFile, password, algorithm,
                                new Progress<int>(percent => {
                                    uiContext.Post(_ => {
                                        encryptProgressBar.Value = percent;
                                    }, null);
                                }));
                        }
                        catch (Exception ex)
                        {
                            uiContext.Post(_ => {
                                throw ex; // Re-throw on UI thread
                            }, null);
                        }
                    });

                    encryptStatusLabel.Text = "Encryption successful!";
                    encryptStatusLabel.ForeColor = Color.Green;
                    MessageBox.Show("File encrypted successfully!");
                }
                catch (Exception ex)
                {
                    encryptStatusLabel.Text = "Encryption failed: " + ex.Message;
                    encryptStatusLabel.ForeColor = Color.Red;
                    MessageBox.Show("Encryption failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    encryptButton.Enabled = true;
                }
            };

            // Modify the decrypt button click handler similarly
            decryptButton.Click += async (sender, e) => {
                if (string.IsNullOrEmpty(fileToDecryptTextBox.Text) ||
                    string.IsNullOrEmpty(outputDecryptTextBox.Text) ||
                    string.IsNullOrEmpty(passwordDecryptTextBox.Text))
                {
                    MessageBox.Show("Please fill in all fields.");
                    return;
                }

                if (!File.Exists(fileToDecryptTextBox.Text))
                {
                    MessageBox.Show("Source file does not exist.");
                    return;
                }

                // Store values from UI before starting the task
                string sourceFile = fileToDecryptTextBox.Text;
                string destFile = outputDecryptTextBox.Text;
                string password = passwordDecryptTextBox.Text;

                try
                {
                    decryptButton.Enabled = false;
                    decryptStatusLabel.Text = "Decrypting...";
                    decryptStatusLabel.ForeColor = Color.Blue;
                    decryptProgressBar.Visible = true;
                    decryptProgressBar.Value = 0;

                    // Synchronized context for UI updates
                    var uiContext = SynchronizationContext.Current;

                    // Run decryption on background thread
                    await Task.Run(() => {
                        try
                        {
                            DecryptFile(sourceFile, destFile, password,
                                new Progress<int>(percent => {
                                    uiContext.Post(_ => {
                                        decryptProgressBar.Value = percent;
                                    }, null);
                                }));
                        }
                        catch (Exception ex)
                        {
                            uiContext.Post(_ => {
                                throw ex; // Re-throw on UI thread
                            }, null);
                        }
                    });

                    decryptStatusLabel.Text = "Decryption successful!";
                    decryptStatusLabel.ForeColor = Color.Green;
                    MessageBox.Show("File decrypted successfully!");
                }
                catch (Exception ex)
                {
                    decryptStatusLabel.Text = "Decryption failed: " + ex.Message;
                    decryptStatusLabel.ForeColor = Color.Red;
                    MessageBox.Show("Decryption failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    decryptButton.Enabled = true;
                }
            };

            contentArea.Controls.Add(pageTitle);
            contentArea.Controls.Add(tabControl);

            // Adjust tab control size when content area resizes
            contentArea.Resize += (sender, e) => {
                tabControl.Size = new Size(contentArea.Width - 40, contentArea.Height - 80);
            };
        }

        private void EncryptFile(string sourceFile, string destinationFile, string password, string algorithm, IProgress<int> progress)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
            {
                // Derive a key and IV from the password
                byte[] salt = new byte[16];
                new RNGCryptoServiceProvider().GetBytes(salt);

                // Write the salt to the output file
                destinationStream.Write(salt, 0, salt.Length);

                // Create key and IV from password
                Rfc2898DeriveBytes keyDerivation = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] key = keyDerivation.GetBytes(algorithm == "AES-256" ? 32 : 24); // AES-256 uses 32 bytes, Triple DES uses 24 bytes
                byte[] iv = keyDerivation.GetBytes(algorithm == "AES-256" ? 16 : 8); // AES uses 16 bytes IV, Triple DES uses 8 bytes IV

                // Create the cryptographic transformation
                ICryptoTransform encryptor = null;
                if (algorithm == "AES-256")
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;
                        encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                    }
                }
                else // Triple DES
                {
                    using (TripleDES tripledes = TripleDES.Create())
                    {
                        tripledes.Key = key;
                        tripledes.IV = iv;
                        encryptor = tripledes.CreateEncryptor(tripledes.Key, tripledes.IV);
                    }
                }

                // Store the encryption algorithm for decryption later
                byte[] algorithmBytes = Encoding.UTF8.GetBytes(algorithm);
                destinationStream.WriteByte((byte)algorithmBytes.Length);
                destinationStream.Write(algorithmBytes, 0, algorithmBytes.Length);

                // Create crypto stream for encryption
                using (CryptoStream cryptoStream = new CryptoStream(destinationStream, encryptor, CryptoStreamMode.Write))
                {
                    // Create a buffer and process the file in chunks for progress reporting
                    byte[] buffer = new byte[4096];
                    long totalBytes = sourceStream.Length;
                    long bytesProcessed = 0;
                    int bytesRead;

                    while ((bytesRead = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        cryptoStream.Write(buffer, 0, bytesRead);
                        bytesProcessed += bytesRead;

                        // Report progress
                        int percentComplete = (int)((bytesProcessed * 100) / totalBytes);
                        progress.Report(percentComplete);
                    }
                }
            }
        }

        private void DecryptFile(string sourceFile, string destinationFile, string password, IProgress<int> progress)
        {
            using (FileStream sourceStream = new FileStream(sourceFile, FileMode.Open, FileAccess.Read))
            using (FileStream destinationStream = new FileStream(destinationFile, FileMode.Create, FileAccess.Write))
            {
                // Read the salt
                byte[] salt = new byte[16];
                sourceStream.Read(salt, 0, salt.Length);

                // Read the algorithm
                int algorithmLength = sourceStream.ReadByte();
                byte[] algorithmBytes = new byte[algorithmLength];
                sourceStream.Read(algorithmBytes, 0, algorithmLength);
                string algorithm = Encoding.UTF8.GetString(algorithmBytes);

                // Create key and IV from password
                Rfc2898DeriveBytes keyDerivation = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] key = keyDerivation.GetBytes(algorithm == "AES-256" ? 32 : 24);
                byte[] iv = keyDerivation.GetBytes(algorithm == "AES-256" ? 16 : 8);

                // Create the cryptographic transformation
                ICryptoTransform decryptor = null;
                if (algorithm == "AES-256")
                {
                    using (Aes aes = Aes.Create())
                    {
                        aes.Key = key;
                        aes.IV = iv;
                        decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    }
                }
                else // Triple DES
                {
                    using (TripleDES tripledes = TripleDES.Create())
                    {
                        tripledes.Key = key;
                        tripledes.IV = iv;
                        decryptor = tripledes.CreateDecryptor(tripledes.Key, tripledes.IV);
                    }
                }

                // Create crypto stream for decryption
                using (CryptoStream cryptoStream = new CryptoStream(sourceStream, decryptor, CryptoStreamMode.Read))
                {
                    // Create a buffer and process the file in chunks for progress reporting
                    byte[] buffer = new byte[4096];
                    long totalBytes = sourceStream.Length - 16 - algorithmLength - 1; // Subtract salt, algorithm length, and algorithm
                    long bytesProcessed = 0;
                    int bytesRead;

                    while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        destinationStream.Write(buffer, 0, bytesRead);
                        bytesProcessed += bytesRead;

                        // Report progress
                        int percentComplete = (int)((bytesProcessed * 100) / totalBytes);
                        progress.Report(percentComplete > 100 ? 100 : percentComplete);
                    }
                }
            }
        }

        private Panel GetContentArea()
        {
            // Clear existing controls in content panel
            contentPanel.Controls.Clear();

            // Create new content area
            Panel contentArea = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true
            };

            // Add content area to content panel
            contentPanel.Controls.Add(contentArea);

            return contentArea;
        }
    }
}