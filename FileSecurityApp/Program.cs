using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

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

        // UI Constants
        private readonly Color primaryColor = Color.FromArgb(52, 73, 94);    // Dark blue-gray
        private readonly Color accentColor = Color.FromArgb(41, 128, 185);   // Medium blue
        private readonly Color lightColor = Color.FromArgb(236, 240, 241);   // Light gray
        private readonly Color darkTextColor = Color.FromArgb(44, 62, 80);   // Very dark blue-gray
        private readonly Color successColor = Color.FromArgb(46, 204, 113);  // Green
        private readonly Color errorColor = Color.FromArgb(231, 76, 60);     // Red
        private readonly Color warningColor = Color.FromArgb(241, 196, 15);  // Yellow
        private readonly Font titleFont = new Font("Segoe UI", 18, FontStyle.Regular);
        private readonly Font subtitleFont = new Font("Segoe UI", 14, FontStyle.Regular);
        private readonly Font regularFont = new Font("Segoe UI", 10, FontStyle.Regular);
        private readonly Font smallFont = new Font("Segoe UI", 9, FontStyle.Regular);
        private readonly Font buttonFont = new Font("Segoe UI", 10, FontStyle.Regular);
        private readonly Font navButtonFont = new Font("Segoe UI", 11, FontStyle.Regular);
        private readonly int standardPadding = 20;
        private readonly int standardButtonHeight = 40;

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
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = regularFont;
            this.Icon = SystemIcons.Shield; // Use a security-related icon

            // Create content panel that will host different pages
            contentPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = lightColor
            };

            // Create navigation panel that will serve as left drawer
            navPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = primaryColor
            };

            this.Controls.Add(contentPanel);

            // Handle form loading
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // No additional initialization needed
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
                Dock = DockStyle.Fill,
                BackColor = lightColor
            };

            // Create logo/app name
            PictureBox logoBox = new PictureBox
            {
                Image = SystemIcons.Shield.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(64, 64),
                Location = new Point(
                    (contentPanel.Width - 64) / 2,
                    (contentPanel.Height - 400) / 2
                )
            };

            Label appNameLabel = new Label
            {
                Text = "SECURE FILE MANAGER",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = darkTextColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Size = new Size(400, 40),
                Location = new Point(
                    (contentPanel.Width - 400) / 2,
                    logoBox.Location.Y + logoBox.Height + 10
                )
            };

            // Login panel with drop shadow effect
            Panel loginPanel = new Panel
            {
                Width = 400,
                Height = 300,
                Location = new Point(
                    (contentPanel.Width - 400) / 2,
                    appNameLabel.Location.Y + appNameLabel.Height + 20
                ),
                BackColor = Color.White
            };

            // Add drop shadow effect to login panel
            loginPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, loginPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 3, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };

            Label titleLabel = new Label
            {
                Text = "Login to continue",
                Font = subtitleFont,
                ForeColor = darkTextColor,
                Location = new Point(25, 20),
                Size = new Size(350, 30)
            };

            Label usernameLabel = new Label
            {
                Text = "Username",
                Font = regularFont,
                ForeColor = darkTextColor,
                Location = new Point(25, 70),
                AutoSize = true
            };

            TextBox usernameTextBox = new TextBox
            {
                Location = new Point(25, 95),
                Width = 350,
                Height = 30,
                Font = regularFont,
                BorderStyle = BorderStyle.FixedSingle
            };

            Label passwordLabel = new Label
            {
                Text = "Password",
                Font = regularFont,
                ForeColor = darkTextColor,
                Location = new Point(25, 135),
                AutoSize = true
            };

            TextBox passwordTextBox = new TextBox
            {
                Location = new Point(25, 160),
                Width = 350,
                Height = 30,
                Font = regularFont,
                PasswordChar = '•',
                BorderStyle = BorderStyle.FixedSingle
            };

            Button loginButton = new Button
            {
                Text = "LOGIN",
                Font = buttonFont,
                Location = new Point(25, 210),
                Width = 350,
                Height = standardButtonHeight,
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Cursor = Cursors.Hand
            };

            // Remove focus rectangle
            loginButton.FlatAppearance.BorderSize = 0;

            // Add hover effect
            loginButton.MouseEnter += (s, e) => {
                loginButton.BackColor = ControlPaint.Light(accentColor);
            };

            loginButton.MouseLeave += (s, e) => {
                loginButton.BackColor = accentColor;
            };

            loginButton.Click += (sender, e) => {
                // Simple authentication for demo
                if (usernameTextBox.Text == "admin" && passwordTextBox.Text == "password")
                {
                    LogEvent("INFO", $"User '{usernameTextBox.Text}' logged in successfully", "Auth");
                    ShowMainPage();
                }
                else
                {
                    LogEvent("WARNING", $"Failed login attempt with username '{usernameTextBox.Text}'", "Auth");

                    // Create a custom error message panel
                    Panel errorPanel = new Panel
                    {
                        BackColor = Color.FromArgb(255, 245, 245),
                        BorderStyle = BorderStyle.None,
                        Size = new Size(350, 40),
                        Location = new Point(25, 260)
                    };

                    Label errorMessage = new Label
                    {
                        Text = "⚠️ Invalid username or password",
                        ForeColor = errorColor,
                        Font = smallFont,
                        AutoSize = true,
                        Location = new Point(10, 12)
                    };

                    errorPanel.Controls.Add(errorMessage);
                    loginPanel.Controls.Add(errorPanel);
                }
            };

            // Add controls to the login panel
            loginPanel.Controls.Add(titleLabel);
            loginPanel.Controls.Add(usernameLabel);
            loginPanel.Controls.Add(usernameTextBox);
            loginPanel.Controls.Add(passwordLabel);
            loginPanel.Controls.Add(passwordTextBox);
            loginPanel.Controls.Add(loginButton);

            // Add all elements to the form
            loginForm.Controls.Add(logoBox);
            loginForm.Controls.Add(appNameLabel);
            loginForm.Controls.Add(loginPanel);

            contentPanel.Controls.Add(loginForm);
            loginForm.Show();

            // Adjust positions when form resizes
            contentPanel.Resize += (sender, e) => {
                logoBox.Location = new Point(
                    (contentPanel.Width - logoBox.Width) / 2,
                    (contentPanel.Height - 400) / 2
                );

                appNameLabel.Location = new Point(
                    (contentPanel.Width - appNameLabel.Width) / 2,
                    logoBox.Location.Y + logoBox.Height + 10
                );

                loginPanel.Location = new Point(
                    (contentPanel.Width - loginPanel.Width) / 2,
                    appNameLabel.Location.Y + appNameLabel.Height + 20
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

            // By default, show the secure folder page
            ShowSecureFolderPage();
        }

        private void CreateNavigationButtons()
        {
            // Clear existing controls
            navPanel.Controls.Clear();

            // Add app title and logo
            PictureBox logoBox = new PictureBox
            {
                Image = SystemIcons.Shield.ToBitmap(),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Size = new Size(32, 32),
                Location = new Point(20, 20)
            };

            Label appTitle = new Label
            {
                Text = "Secure File Manager",
                ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 14),
                Location = new Point(60, 20),
                Size = new Size(180, 32)
            };

            navPanel.Controls.Add(logoBox);
            navPanel.Controls.Add(appTitle);

            // Add user info section
            Panel userPanel = new Panel
            {
                BackColor = Color.FromArgb(44, 62, 80), // Slightly darker than primary
                Location = new Point(0, 70),
                Size = new Size(navPanel.Width, 80)
            };

            Label userIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 24),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };

            Label userNameLabel = new Label
            {
                Text = "Administrator",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(60, 20),
                AutoSize = true
            };

            Label userRoleLabel = new Label
            {
                Text = "System Admin",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.LightGray,
                Location = new Point(60, 45),
                AutoSize = true
            };

            userPanel.Controls.Add(userIcon);
            userPanel.Controls.Add(userNameLabel);
            userPanel.Controls.Add(userRoleLabel);
            navPanel.Controls.Add(userPanel);

            // Add navigation buttons with icons
            int buttonY = 170; // Start position after user panel

            Button secureFolderButton = CreateNavButton("🗂️  My Secure Folder", buttonY);
            Button logsButton = CreateNavButton("📋  System Logs", buttonY + 50);
            Button integrityButton = CreateNavButton("🔒  Integrity Checking", buttonY + 100);
            Button encryptionButton = CreateNavButton("🔐  File Encryption", buttonY + 150);

            // Event handlers for buttons
            secureFolderButton.Click += (sender, e) => {
                HighlightNavButton(secureFolderButton);
                ShowSecureFolderPage();
            };

            logsButton.Click += (sender, e) => {
                HighlightNavButton(logsButton);
                ShowLogsPage();
            };

            integrityButton.Click += (sender, e) => {
                HighlightNavButton(integrityButton);
                ShowIntegrityPage();
            };

            encryptionButton.Click += (sender, e) => {
                HighlightNavButton(encryptionButton);
                ShowEncryptionPage();
            };

            // Add buttons to the navigation panel
            navPanel.Controls.Add(secureFolderButton);
            navPanel.Controls.Add(logsButton);
            navPanel.Controls.Add(integrityButton);
            navPanel.Controls.Add(encryptionButton);

            // Add logout button at bottom
            Button logoutButton = new Button
            {
                Text = "🚪  Logout",
                Font = navButtonFont,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = Color.FromArgb(44, 62, 80), // Darker
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Size = new Size(navPanel.Width, standardButtonHeight),
                Location = new Point(0, navPanel.Height - standardButtonHeight),
                Cursor = Cursors.Hand
            };

            logoutButton.Click += (sender, e) => {
                LogEvent("INFO", "User logged out", "Auth");
                navPanel.Visible = false;
                ShowLoginPage();
            };

            navPanel.Controls.Add(logoutButton);

            // Handle resize to keep logout button at bottom
            navPanel.Resize += (sender, e) => {
                logoutButton.Location = new Point(0, navPanel.Height - standardButtonHeight);
            };

            // Highlight the first button by default
            HighlightNavButton(secureFolderButton);
        }

        private Button CreateNavButton(string text, int yPos)
        {
            Button button = new Button
            {
                Text = text,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0 },
                BackColor = primaryColor,
                ForeColor = Color.White,
                Location = new Point(0, yPos),
                Size = new Size(navPanel.Width, standardButtonHeight),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Font = navButtonFont,
                Cursor = Cursors.Hand
            };

            // Add hover effect
            button.MouseEnter += (s, e) => {
                if (button.BackColor != accentColor) // Don't change if it's the active button
                    button.BackColor = Color.FromArgb(64, 85, 106); // Slightly lighter
            };

            button.MouseLeave += (s, e) => {
                if (button.BackColor != accentColor) // Don't change if it's the active button
                    button.BackColor = primaryColor;
            };

            return button;
        }

        private void HighlightNavButton(Button activeButton)
        {
            // Reset all buttons
            foreach (Control control in navPanel.Controls)
            {
                if (control is Button button && button.Location.Y >= 170 && button.Location.Y < navPanel.Height - standardButtonHeight)
                {
                    button.BackColor = primaryColor;
                    button.FlatAppearance.BorderSize = 0;
                }
            }

            // Highlight the active button
            activeButton.BackColor = accentColor;
            activeButton.FlatAppearance.BorderSize = 0;
        }

        private Button viewFolderButton;
        private string secureFolderPath = @"C:\ProgramData\SecureFiles"; // Hidden Secure Folder Path
        private ListView secureFolderList;
        private FileSystemWatcher fileWatcher;

        private void ShowSecureFolderPage()
        {
            Panel contentArea = GetContentArea();

            // Create a page header
            Panel headerPanel = CreatePageHeader("My Secure Folder", "Manage your sensitive files in a secured location");
            contentArea.Controls.Add(headerPanel);

            // Add stats cards
            int cardWidth = (contentArea.Width - (4 * standardPadding)) / 3;

            Panel filesCard = CreateStatsCard("Total Files", "0", "🗂️", cardWidth);
            filesCard.Location = new Point(standardPadding, headerPanel.Bottom + standardPadding);

            Panel sizeCard = CreateStatsCard("Total Size", "0 KB", "💾", cardWidth);
            sizeCard.Location = new Point(filesCard.Right + standardPadding, headerPanel.Bottom + standardPadding);

            Panel lastModifiedCard = CreateStatsCard("Last Modified", "Never", "🕒", cardWidth);
            lastModifiedCard.Location = new Point(sizeCard.Right + standardPadding, headerPanel.Bottom + standardPadding);

            contentArea.Controls.Add(filesCard);
            contentArea.Controls.Add(sizeCard);
            contentArea.Controls.Add(lastModifiedCard);

            // Create action button panel
            Panel actionPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = 50,
                Location = new Point(standardPadding, filesCard.Bottom + standardPadding),
                BackColor = Color.White
            };

            // Add drop shadow
            actionPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, actionPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            // Add button to view folder
            viewFolderButton = new Button
            {
                Text = "Open Secure Folder",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Size = new Size(180, 34),
                Location = new Point(10, 8),
                Cursor = Cursors.Hand
            };

            viewFolderButton.FlatAppearance.BorderSize = 0;
            viewFolderButton.Click += ViewSecureFolder;
            actionPanel.Controls.Add(viewFolderButton);

            // Add a refresh button
            Button refreshButton = new Button
            {
                Text = "↻ Refresh",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(236, 240, 241),
                ForeColor = darkTextColor,
                Size = new Size(120, 34),
                Location = new Point(viewFolderButton.Right + 10, 8),
                Cursor = Cursors.Hand
            };

            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += (s, e) => LoadSecureFolderFiles();
            actionPanel.Controls.Add(refreshButton);

            contentArea.Controls.Add(actionPanel);

            // Create file list with a panel for styling
            Panel listPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = contentArea.Height - actionPanel.Bottom - (2 * standardPadding),
                Location = new Point(standardPadding, actionPanel.Bottom + standardPadding),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Add drop shadow
            listPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, listPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            // Initialize ListView
            secureFolderList = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                Location = new Point(1, 1),
                Size = new Size(listPanel.Width - 2, listPanel.Height - 2),
                BorderStyle = BorderStyle.None,
                BackColor = Color.White,
                Font = regularFont
            };

            // Adding columns
            secureFolderList.Columns.Add("File Name", 300);
            secureFolderList.Columns.Add("Size", 100);
            secureFolderList.Columns.Add("Last Modified", 150);
            secureFolderList.Columns.Add("Creation Time", 150);
            secureFolderList.Columns.Add("Last Accessed", 150);

            // Add right-click context menu
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add("Open", null, (s, e) => {
                if (secureFolderList.SelectedItems.Count > 0)
                {
                    string filePath = Path.Combine(secureFolderPath, secureFolderList.SelectedItems[0].Text);
                    Process.Start(filePath);
                }
            });

            contextMenu.Items.Add("Calculate Hash", null, (s, e) => {
                if (secureFolderList.SelectedItems.Count > 0)
                {
                    // Show integrity page
                    foreach (Control control in navPanel.Controls)
                    {
                        if (control is Button button && button.Text.Contains("Integrity"))
                        {
                            button.PerformClick();
                            break;
                        }
                    }
                }
            });

            contextMenu.Items.Add("Encrypt", null, (s, e) => {
                if (secureFolderList.SelectedItems.Count > 0)
                {
                    // Show encryption page
                    foreach (Control control in navPanel.Controls)
                    {
                        if (control is Button button && button.Text.Contains("Encryption"))
                        {
                            button.PerformClick();
                            break;
                        }
                    }
                }
            });

            contextMenu.Items.Add(new ToolStripSeparator());

            contextMenu.Items.Add("Delete", null, (s, e) => {
                if (secureFolderList.SelectedItems.Count > 0)
                {
                    string fileName = secureFolderList.SelectedItems[0].Text;
                    if (MessageBox.Show($"Are you sure you want to delete '{fileName}'?",
                        "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        try
                        {
                            File.Delete(Path.Combine(secureFolderPath, fileName));
                            LoadSecureFolderFiles(); // Refresh
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error deleting file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            });

            secureFolderList.ContextMenuStrip = contextMenu;
            listPanel.Controls.Add(secureFolderList);
            contentArea.Controls.Add(listPanel);

            // Adjust UI on resize
            contentArea.Resize += (sender, e) => {
                // Resize header
                headerPanel.Width = contentArea.Width;

                // Resize cards
                cardWidth = (contentArea.Width - (4 * standardPadding)) / 3;
                filesCard.Width = cardWidth;
                sizeCard.Width = cardWidth;
                lastModifiedCard.Width = cardWidth;
                sizeCard.Location = new Point(filesCard.Right + standardPadding, headerPanel.Bottom + standardPadding);
                lastModifiedCard.Location = new Point(sizeCard.Right + standardPadding, headerPanel.Bottom + standardPadding);

                // Resize action panel
                actionPanel.Width = contentArea.Width - (2 * standardPadding);

                // Resize list panel
                listPanel.Width = contentArea.Width - (2 * standardPadding);
                listPanel.Height = contentArea.Height - actionPanel.Bottom - (2 * standardPadding);
                secureFolderList.Size = new Size(listPanel.Width - 2, listPanel.Height - 2);
            };

            // Load existing files and start monitoring
            LoadSecureFolderFiles();
            StartFolderMonitoring();
        }

        private Panel CreatePageHeader(string title, string subtitle)
        {
            Panel headerPanel = new Panel
            {
                Width = contentPanel.Width,
                Height = 80,
                BackColor = Color.White,
                Dock = DockStyle.Top
            };

            // Add shadow effect
            headerPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, headerPanel.ClientRectangle,
                    Color.Transparent, 0, ButtonBorderStyle.None,
                    Color.Transparent, 0, ButtonBorderStyle.None,
                    Color.Transparent, 0, ButtonBorderStyle.None,
                    Color.LightGray, 1, ButtonBorderStyle.Solid);
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = titleFont,
                ForeColor = darkTextColor,
                Location = new Point(standardPadding, 15),
                AutoSize = true
            };

            Label subtitleLabel = new Label
            {
                Text = subtitle,
                Font = smallFont,
                ForeColor = Color.Gray,
                Location = new Point(standardPadding, titleLabel.Bottom + 5),
                AutoSize = true
            };

            headerPanel.Controls.Add(titleLabel);
            headerPanel.Controls.Add(subtitleLabel);

            return headerPanel;
        }

        private Panel CreateStatsCard(string title, string value, string icon, int width)
        {
            Panel card = new Panel
            {
                Width = width,
                Height = 100,
                BackColor = Color.White
            };

            // Add shadow effect
            card.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, card.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            Label iconLabel = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 24),
                ForeColor = accentColor,
                Location = new Point(20, 20),
                AutoSize = true
            };

            Label titleLabel = new Label
            {
                Text = title,
                Font = smallFont,
                ForeColor = Color.Gray,
                Location = new Point(80, 25),
                AutoSize = true
            };

            Label valueLabel = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 24),
                ForeColor = darkTextColor,
                Location = new Point(80, 45),
                AutoSize = true,
                Tag = "value" // Tag for updating later
            };

            card.Controls.Add(iconLabel);
            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);

            return card;
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
            FileInfo[] files = dirInfo.GetFiles();

            // Update stats cards
            long totalSize = 0;
            DateTime lastModified = DateTime.MinValue;

            foreach (FileInfo file in files)
            {
                string[] row = {
                    file.Name,
                    (file.Length / 1024.0).ToString("F2") + " KB",
                    file.LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    file.CreationTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    file.LastAccessTime.ToString("yyyy-MM-dd HH:mm:ss")
                };

                ListViewItem item = new ListViewItem(row);

                // Add file type icon
                if (file.Extension.ToLower() == ".txt" || file.Extension.ToLower() == ".log")
                    item.ImageIndex = 0; // Text document icon
                else if (file.Extension.ToLower() == ".encrypted")
                    item.ImageIndex = 1; // Encrypted file icon
                else
                    item.ImageIndex = 2; // Generic file icon

                secureFolderList.Items.Add(item);

                // Track stats
                totalSize += file.Length;
                if (file.LastWriteTime > lastModified)
                    lastModified = file.LastWriteTime;
            }

            // Update stats cards if they exist
            UpdateStatsCards(files.Length, totalSize, lastModified);
        }

        private void UpdateStatsCards(int fileCount, long totalSizeBytes, DateTime lastModified)
        {
            // Find and update the statistics cards
            foreach (Control control in contentPanel.Controls)
            {
                if (control is Panel panel)
                {
                    foreach (Control panelControl in panel.Controls)
                    {
                        if (panelControl is Label label && label.Tag != null && label.Tag.ToString() == "value")
                        {
                            // Update based on the panel's title
                            Panel parentCard = (Panel)label.Parent;
                            string cardTitle = "";

                            foreach (Control titleControl in parentCard.Controls)
                            {
                                if (titleControl is Label titleLabel && titleLabel != label && !titleLabel.Text.Contains("📋") &&
                                    !titleLabel.Text.Contains("💾") && !titleLabel.Text.Contains("🕒"))
                                {
                                    cardTitle = titleLabel.Text;
                                    break;
                                }
                            }

                            if (cardTitle == "Total Files")
                            {
                                label.Text = fileCount.ToString();
                            }
                            else if (cardTitle == "Total Size")
                            {
                                string sizeText;
                                if (totalSizeBytes < 1024)
                                    sizeText = totalSizeBytes + " B";
                                else if (totalSizeBytes < 1024 * 1024)
                                    sizeText = (totalSizeBytes / 1024.0).ToString("F2") + " KB";
                                else
                                    sizeText = (totalSizeBytes / (1024.0 * 1024.0)).ToString("F2") + " MB";

                                label.Text = sizeText;
                            }
                            else if (cardTitle == "Last Modified")
                            {
                                if (lastModified == DateTime.MinValue)
                                    label.Text = "Never";
                                else
                                    label.Text = lastModified.ToString("yyyy-MM-dd");
                            }
                        }
                    }
                }
            }
        }

        private void StartFolderMonitoring()
        {
            // Dispose of any existing watcher
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
            }

            // Create new file system watcher
            fileWatcher = new FileSystemWatcher
            {
                Path = secureFolderPath,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size,
                Filter = "*.*",
                EnableRaisingEvents = true
            };

            // Add event handlers
            fileWatcher.Created += (sender, e) => {
                Invoke(new Action(() => {
                    LoadSecureFolderFiles();
                    LogEvent("INFO", $"File created: {Path.GetFileName(e.FullPath)}", "SecureFolder");
                }));
            };

            fileWatcher.Deleted += (sender, e) => {
                Invoke(new Action(() => {
                    LoadSecureFolderFiles();
                    LogEvent("INFO", $"File deleted: {Path.GetFileName(e.FullPath)}", "SecureFolder");
                }));
            };

            fileWatcher.Changed += (sender, e) => {
                Invoke(new Action(() => {
                    LoadSecureFolderFiles();
                    LogEvent("INFO", $"File changed: {Path.GetFileName(e.FullPath)}", "SecureFolder");
                }));
            };

            fileWatcher.Renamed += (sender, e) => {
                Invoke(new Action(() => {
                    LoadSecureFolderFiles();
                    LogEvent("INFO", $"File renamed from {Path.GetFileName(e.OldFullPath)} to {Path.GetFileName(e.FullPath)}", "SecureFolder");
                }));
            };
        }

        private void ViewSecureFolder(object sender, EventArgs e)
        {
            try
            {
                // Ensure the directory exists
                if (!Directory.Exists(secureFolderPath))
                {
                    Directory.CreateDirectory(secureFolderPath);
                }

                // Open the folder in explorer
                Process.Start("explorer.exe", secureFolderPath);
                LogEvent("INFO", "Secure folder opened by user", "SecureFolder");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening folder: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogEvent("ERROR", $"Failed to open secure folder: {ex.Message}", "SecureFolder");
            }
        }

        private void ShowLogsPage()
        {
            Panel contentArea = GetContentArea();

            // Create a page header
            Panel headerPanel = CreatePageHeader("System Logs", "View and analyze system activity logs");
            contentArea.Controls.Add(headerPanel);

            // Create a panel for the log viewer
            Panel logPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = contentArea.Height - headerPanel.Bottom - (2 * standardPadding),
                Location = new Point(standardPadding, headerPanel.Bottom + standardPadding),
                BackColor = Color.White,
                BorderStyle = BorderStyle.None
            };

            // Add drop shadow
            logPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, logPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            // Create log viewer
            RichTextBox logViewer = new RichTextBox
            {
                Location = new Point(1, 1),
                Size = new Size(logPanel.Width - 2, logPanel.Height - 2),
                BackColor = Color.White,
                Font = new Font("Consolas", 10),
                ReadOnly = true,
                BorderStyle = BorderStyle.None
            };

            // Load log contents
            try
            {
                if (File.Exists(logFilePath))
                {
                    string[] logLines = File.ReadAllLines(logFilePath);
                    foreach (string line in logLines)
                    {
                        // Add colorization based on log level
                        if (line.Contains(" | ERROR | "))
                        {
                            logViewer.SelectionColor = errorColor;
                        }
                        else if (line.Contains(" | WARNING | "))
                        {
                            logViewer.SelectionColor = warningColor;
                        }
                        else if (line.Contains(" | INFO | "))
                        {
                            logViewer.SelectionColor = accentColor;
                        }
                        else
                        {
                            logViewer.SelectionColor = darkTextColor;
                        }

                        logViewer.AppendText(line + "\n");
                    }
                }
                else
                {
                    logViewer.Text = "No log file found. Log events will be recorded here.";
                }
            }
            catch (Exception ex)
            {
                logViewer.Text = $"Error loading log file: {ex.Message}";
            }

            logPanel.Controls.Add(logViewer);
            contentArea.Controls.Add(logPanel);

            // Add refresh button
            Button refreshButton = new Button
            {
                Text = "↻ Refresh Logs",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Size = new Size(150, standardButtonHeight),
                Location = new Point(contentArea.Width - 170, headerPanel.Bottom - 50),
                Cursor = Cursors.Hand
            };

            refreshButton.FlatAppearance.BorderSize = 0;
            refreshButton.Click += (s, e) => {
                try
                {
                    logViewer.Clear();
                    if (File.Exists(logFilePath))
                    {
                        string[] logLines = File.ReadAllLines(logFilePath);
                        foreach (string line in logLines)
                        {
                            // Add colorization based on log level
                            if (line.Contains(" | ERROR | "))
                            {
                                logViewer.SelectionColor = errorColor;
                            }
                            else if (line.Contains(" | WARNING | "))
                            {
                                logViewer.SelectionColor = warningColor;
                            }
                            else if (line.Contains(" | INFO | "))
                            {
                                logViewer.SelectionColor = accentColor;
                            }
                            else
                            {
                                logViewer.SelectionColor = darkTextColor;
                            }

                            logViewer.AppendText(line + "\n");
                        }
                    }
                    else
                    {
                        logViewer.Text = "No log file found. Log events will be recorded here.";
                    }
                }
                catch (Exception ex)
                {
                    logViewer.Text = $"Error loading log file: {ex.Message}";
                }
            };

            headerPanel.Controls.Add(refreshButton);

            // Adjust UI on resize
            contentArea.Resize += (sender, e) => {
                // Resize header
                headerPanel.Width = contentArea.Width;
                refreshButton.Location = new Point(contentArea.Width - 170, headerPanel.Bottom - 50);

                // Resize log panel
                logPanel.Width = contentArea.Width - (2 * standardPadding);
                logPanel.Height = contentArea.Height - headerPanel.Bottom - (2 * standardPadding);
                logViewer.Size = new Size(logPanel.Width - 2, logPanel.Height - 2);
            };
        }

        private void ShowIntegrityPage()
        {
            Panel contentArea = GetContentArea();

            // Create a page header
            Panel headerPanel = CreatePageHeader("File Integrity Checking", "Verify file integrity using cryptographic hash functions");
            contentArea.Controls.Add(headerPanel);

            // Create file selection panel
            Panel fileSelectionPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = 60,
                Location = new Point(standardPadding, headerPanel.Bottom + standardPadding),
                BackColor = Color.White
            };

            // Add drop shadow
            fileSelectionPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, fileSelectionPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            Label fileLabel = new Label
            {
                Text = "Select File:",
                Font = regularFont,
                Location = new Point(15, 20),
                AutoSize = true
            };

            TextBox filePathTextBox = new TextBox
            {
                Location = new Point(100, 17),
                Width = fileSelectionPanel.Width - 400,
                Font = regularFont,
                ReadOnly = true
            };

            Button browseButton = new Button
            {
                Text = "Browse...",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Size = new Size(100, 30),
                Location = new Point(filePathTextBox.Right + 10, 15),
                Cursor = Cursors.Hand
            };

            browseButton.FlatAppearance.BorderSize = 0;
            browseButton.Click += (s, e) => {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = secureFolderPath,
                    Title = "Select a file to check integrity"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePathTextBox.Text = openFileDialog.FileName;
                }
            };

            fileSelectionPanel.Controls.Add(fileLabel);
            fileSelectionPanel.Controls.Add(filePathTextBox);
            fileSelectionPanel.Controls.Add(browseButton);
            contentArea.Controls.Add(fileSelectionPanel);

            // Create hash options panel
            Panel hashOptionsPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = 120,
                Location = new Point(standardPadding, fileSelectionPanel.Bottom + standardPadding),
                BackColor = Color.White
            };

            // Add drop shadow
            hashOptionsPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, hashOptionsPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            Label hashAlgorithmLabel = new Label
            {
                Text = "Hash Algorithm:",
                Font = regularFont,
                Location = new Point(15, 20),
                AutoSize = true
            };

            ComboBox hashAlgorithmComboBox = new ComboBox
            {
                Location = new Point(150, 17),
                Width = 200,
                Font = regularFont,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            hashAlgorithmComboBox.Items.AddRange(new string[] { "MD5", "SHA1", "SHA256", "SHA384", "SHA512" });
            hashAlgorithmComboBox.SelectedIndex = 2; // Default to SHA256

            Button calculateButton = new Button
            {
                Text = "Calculate Hash",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Size = new Size(150, 36),
                Location = new Point(15, 60),
                Cursor = Cursors.Hand
            };

            calculateButton.FlatAppearance.BorderSize = 0;

            Label verifyLabel = new Label
            {
                Text = "Verify Hash:",
                Font = regularFont,
                Location = new Point(400, 20),
                AutoSize = true
            };

            TextBox verifyHashTextBox = new TextBox
            {
                Location = new Point(500, 17),
                Width = hashOptionsPanel.Width - 550,
                Font = regularFont
            };

            Button verifyButton = new Button
            {
                Text = "Verify",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219), // Different blue
                ForeColor = Color.White,
                Size = new Size(100, 36),
                Location = new Point(500, 60),
                Cursor = Cursors.Hand
            };

            verifyButton.FlatAppearance.BorderSize = 0;

            hashOptionsPanel.Controls.Add(hashAlgorithmLabel);
            hashOptionsPanel.Controls.Add(hashAlgorithmComboBox);
            hashOptionsPanel.Controls.Add(calculateButton);
            hashOptionsPanel.Controls.Add(verifyLabel);
            hashOptionsPanel.Controls.Add(verifyHashTextBox);
            hashOptionsPanel.Controls.Add(verifyButton);
            contentArea.Controls.Add(hashOptionsPanel);

            // Create result panel
            Panel resultPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = contentArea.Height - hashOptionsPanel.Bottom - (2 * standardPadding),
                Location = new Point(standardPadding, hashOptionsPanel.Bottom + standardPadding),
                BackColor = Color.White
            };

            // Add drop shadow
            resultPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, resultPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            Label resultTitleLabel = new Label
            {
                Text = "Hash Result:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };

            TextBox hashResultTextBox = new TextBox
            {
                Location = new Point(15, 50),
                Width = resultPanel.Width - 30,
                Height = 50,
                Font = new Font("Consolas", 10),
                ReadOnly = true,
                Multiline = true
            };

            Label statusLabel = new Label
            {
                Text = "Status: Ready",
                Font = regularFont,
                Location = new Point(15, hashResultTextBox.Bottom + 15),
                AutoSize = true,
                ForeColor = darkTextColor
            };

            resultPanel.Controls.Add(resultTitleLabel);
            resultPanel.Controls.Add(hashResultTextBox);
            resultPanel.Controls.Add(statusLabel);
            contentArea.Controls.Add(resultPanel);

            // Calculate hash button logic
            calculateButton.Click += (s, e) => {
                if (string.IsNullOrEmpty(filePathTextBox.Text) || !File.Exists(filePathTextBox.Text))
                {
                    MessageBox.Show("Please select a valid file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    statusLabel.Text = "Status: Calculating hash...";
                    statusLabel.ForeColor = warningColor;
                    Application.DoEvents();

                    string hashAlgorithm = hashAlgorithmComboBox.SelectedItem.ToString();
                    string hashValue = CalculateFileHash(filePathTextBox.Text, hashAlgorithm);

                    hashResultTextBox.Text = hashValue;
                    statusLabel.Text = "Status: Hash calculation complete";
                    statusLabel.ForeColor = successColor;

                    LogEvent("INFO", $"Calculated {hashAlgorithm} hash for file: {Path.GetFileName(filePathTextBox.Text)}", "Integrity");
                }
                catch (Exception ex)
                {
                    hashResultTextBox.Text = "";
                    statusLabel.Text = $"Status: Error - {ex.Message}";
                    statusLabel.ForeColor = errorColor;

                    LogEvent("ERROR", $"Failed to calculate hash: {ex.Message}", "Integrity");
                }
            };

            // Verify hash button logic
            verifyButton.Click += (s, e) => {
                if (string.IsNullOrEmpty(filePathTextBox.Text) || !File.Exists(filePathTextBox.Text))
                {
                    MessageBox.Show("Please select a valid file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(verifyHashTextBox.Text))
                {
                    MessageBox.Show("Please enter a hash value to verify.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                try
                {
                    statusLabel.Text = "Status: Verifying hash...";
                    statusLabel.ForeColor = warningColor;
                    Application.DoEvents();

                    string hashAlgorithm = hashAlgorithmComboBox.SelectedItem.ToString();
                    string actualHash = CalculateFileHash(filePathTextBox.Text, hashAlgorithm);
                    string expectedHash = verifyHashTextBox.Text.Trim();

                    hashResultTextBox.Text = actualHash;

                    if (string.Equals(actualHash, expectedHash, StringComparison.OrdinalIgnoreCase))
                    {
                        statusLabel.Text = "Status: Verification PASSED - Hashes match!";
                        statusLabel.ForeColor = successColor;

                        LogEvent("INFO", $"Hash verification PASSED for file: {Path.GetFileName(filePathTextBox.Text)}", "Integrity");
                    }
                    else
                    {
                        statusLabel.Text = "Status: Verification FAILED - Hashes do not match!";
                        statusLabel.ForeColor = errorColor;

                        LogEvent("WARNING", $"Hash verification FAILED for file: {Path.GetFileName(filePathTextBox.Text)}", "Integrity");
                    }
                }
                catch (Exception ex)
                {
                    hashResultTextBox.Text = "";
                    statusLabel.Text = $"Status: Error - {ex.Message}";
                    statusLabel.ForeColor = errorColor;

                    LogEvent("ERROR", $"Failed to verify hash: {ex.Message}", "Integrity");
                }
            };

            // Adjust UI on resize
            contentArea.Resize += (sender, e) => {
                // Resize header
                headerPanel.Width = contentArea.Width;

                // Resize panels
                fileSelectionPanel.Width = contentArea.Width - (2 * standardPadding);
                filePathTextBox.Width = fileSelectionPanel.Width - 400;
                browseButton.Location = new Point(filePathTextBox.Right + 10, 15);

                hashOptionsPanel.Width = contentArea.Width - (2 * standardPadding);
                verifyHashTextBox.Width = hashOptionsPanel.Width - 550;

                resultPanel.Width = contentArea.Width - (2 * standardPadding);
                resultPanel.Height = contentArea.Height - hashOptionsPanel.Bottom - (2 * standardPadding);
                hashResultTextBox.Width = resultPanel.Width - 30;
            };
        }

        private string CalculateFileHash(string filePath, string algorithm)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                HashAlgorithm hashAlgorithm;

                switch (algorithm)
                {
                    case "MD5":
                        hashAlgorithm = MD5.Create();
                        break;
                    case "SHA1":
                        hashAlgorithm = SHA1.Create();
                        break;
                    case "SHA256":
                        hashAlgorithm = SHA256.Create();
                        break;
                    case "SHA384":
                        hashAlgorithm = SHA384.Create();
                        break;
                    case "SHA512":
                        hashAlgorithm = SHA512.Create();
                        break;
                    default:
                        throw new ArgumentException("Unsupported hash algorithm");
                }

                byte[] hashBytes = hashAlgorithm.ComputeHash(fileStream);

                // Convert to hex string
                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("x2"));
                }

                return hashBuilder.ToString();
            }
        }

        private void ShowEncryptionPage()
        {
            Panel contentArea = GetContentArea();

            // Create a page header
            Panel headerPanel = CreatePageHeader("File Encryption", "Encrypt and decrypt files with AES-256 encryption");
            contentArea.Controls.Add(headerPanel);

            // Create file selection panel
            Panel fileSelectionPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = 60,
                Location = new Point(standardPadding, headerPanel.Bottom + standardPadding),
                BackColor = Color.White
            };

            // Add drop shadow
            fileSelectionPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, fileSelectionPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            Label fileLabel = new Label
            {
                Text = "Select File:",
                Font = regularFont,
                Location = new Point(15, 20),
                AutoSize = true
            };

            TextBox filePathTextBox = new TextBox
            {
                Location = new Point(100, 17),
                Width = fileSelectionPanel.Width - 400,
                Font = regularFont,
                ReadOnly = true
            };

            Button browseButton = new Button
            {
                Text = "Browse...",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Size = new Size(100, 30),
                Location = new Point(filePathTextBox.Right + 10, 15),
                Cursor = Cursors.Hand
            };

            browseButton.FlatAppearance.BorderSize = 0;
            browseButton.Click += (s, e) => {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    InitialDirectory = secureFolderPath,
                    Title = "Select a file to encrypt or decrypt"
                };

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePathTextBox.Text = openFileDialog.FileName;
                }
            };

            fileSelectionPanel.Controls.Add(fileLabel);
            fileSelectionPanel.Controls.Add(filePathTextBox);
            fileSelectionPanel.Controls.Add(browseButton);
            contentArea.Controls.Add(fileSelectionPanel);

            // Create encryption options panel
            Panel encryptionOptionsPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = 150,
                Location = new Point(standardPadding, fileSelectionPanel.Bottom + standardPadding),
                BackColor = Color.White
            };

            // Add drop shadow
            encryptionOptionsPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, encryptionOptionsPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            Label passwordLabel = new Label
            {
                Text = "Password:",
                Font = regularFont,
                Location = new Point(15, 20),
                AutoSize = true
            };

            TextBox passwordTextBox = new TextBox
            {
                Location = new Point(150, 17),
                Width = 300,
                Font = regularFont,
                PasswordChar = '•'
            };

            Label confirmPasswordLabel = new Label
            {
                Text = "Confirm Password:",
                Font = regularFont,
                Location = new Point(15, 60),
                AutoSize = true
            };

            TextBox confirmPasswordTextBox = new TextBox
            {
                Location = new Point(150, 57),
                Width = 300,
                Font = regularFont,
                PasswordChar = '•'
            };

            CheckBox showPasswordCheckBox = new CheckBox
            {
                Text = "Show password",
                Font = smallFont,
                Location = new Point(460, 17),
                AutoSize = true
            };

            showPasswordCheckBox.CheckedChanged += (s, e) => {
                passwordTextBox.PasswordChar = showPasswordCheckBox.Checked ? '\0' : '•';
                confirmPasswordTextBox.PasswordChar = showPasswordCheckBox.Checked ? '\0' : '•';
            };

            Button encryptButton = new Button
            {
                Text = "Encrypt File",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = accentColor,
                ForeColor = Color.White,
                Size = new Size(150, 36),
                Location = new Point(15, 100),
                Cursor = Cursors.Hand
            };

            encryptButton.FlatAppearance.BorderSize = 0;

            Button decryptButton = new Button
            {
                Text = "Decrypt File",
                Font = buttonFont,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(52, 152, 219), // Different blue
                ForeColor = Color.White,
                Size = new Size(150, 36),
                Location = new Point(180, 100),
                Cursor = Cursors.Hand
            };

            decryptButton.FlatAppearance.BorderSize = 0;

            encryptionOptionsPanel.Controls.Add(passwordLabel);
            encryptionOptionsPanel.Controls.Add(passwordTextBox);
            encryptionOptionsPanel.Controls.Add(confirmPasswordLabel);
            encryptionOptionsPanel.Controls.Add(confirmPasswordTextBox);
            encryptionOptionsPanel.Controls.Add(showPasswordCheckBox);
            encryptionOptionsPanel.Controls.Add(encryptButton);
            encryptionOptionsPanel.Controls.Add(decryptButton);
            contentArea.Controls.Add(encryptionOptionsPanel);

            // Create status panel
            Panel statusPanel = new Panel
            {
                Width = contentArea.Width - (2 * standardPadding),
                Height = contentArea.Height - encryptionOptionsPanel.Bottom - (2 * standardPadding),
                Location = new Point(standardPadding, encryptionOptionsPanel.Bottom + standardPadding),
                BackColor = Color.White
            };

            // Add drop shadow
            statusPanel.Paint += (s, e) => {
                ControlPaint.DrawBorder(e.Graphics, statusPanel.ClientRectangle,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.LightGray, 1, ButtonBorderStyle.Solid,
                    Color.Gray, 1, ButtonBorderStyle.Solid);
            };

            Label statusTitleLabel = new Label
            {
                Text = "Status:",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Location = new Point(15, 15),
                AutoSize = true
            };

            Label statusMessageLabel = new Label
            {
                Text = "Ready",
                Font = regularFont,
                Location = new Point(15, 50),
                Size = new Size(statusPanel.Width - 30, 30),
                ForeColor = darkTextColor
            };

            // Progress bar for encryption/decryption
            ProgressBar progressBar = new ProgressBar
            {
                Location = new Point(15, 90),
                Width = statusPanel.Width - 30,
                Height = 25,
                Style = ProgressBarStyle.Continuous,
                Value = 0
            };

            Label encryptionDetailsLabel = new Label
            {
                Text = "Encryption Details: AES-256, CBC Mode, PKCS7 padding",
                Font = smallFont,
                Location = new Point(15, 130),
                AutoSize = true,
                ForeColor = Color.Gray
            };

            statusPanel.Controls.Add(statusTitleLabel);
            statusPanel.Controls.Add(statusMessageLabel);
            statusPanel.Controls.Add(progressBar);
            statusPanel.Controls.Add(encryptionDetailsLabel);
            contentArea.Controls.Add(statusPanel);

            // Encrypt button logic
            encryptButton.Click += (s, e) => {
                if (string.IsNullOrEmpty(filePathTextBox.Text) || !File.Exists(filePathTextBox.Text))
                {
                    MessageBox.Show("Please select a valid file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(passwordTextBox.Text))
                {
                    MessageBox.Show("Please enter a password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (passwordTextBox.Text != confirmPasswordTextBox.Text)
                {
                    MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string inputFilePath = filePathTextBox.Text;
                string outputFilePath = inputFilePath + ".encrypted";

                try
                {
                    statusMessageLabel.Text = "Encrypting file...";
                    statusMessageLabel.ForeColor = warningColor;
                    progressBar.Value = 0;
                    Application.DoEvents();

                    // Simple progress reporting
                    progressBar.Value = 10;
                    Application.DoEvents();

                    // Perform the encryption
                    EncryptFile(inputFilePath, outputFilePath, passwordTextBox.Text);

                    progressBar.Value = 100;
                    statusMessageLabel.Text = "Encryption complete! Output file: " + Path.GetFileName(outputFilePath);
                    statusMessageLabel.ForeColor = successColor;

                    LogEvent("INFO", $"File encrypted: {Path.GetFileName(inputFilePath)}", "Encryption");

                    // Ask if user wants to open the folder containing the encrypted file
                    if (MessageBox.Show("Encryption successful. Would you like to open the folder containing the encrypted file?",
                        "Encryption Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start("explorer.exe", "/select," + outputFilePath);
                    }
                }
                catch (Exception ex)
                {
                    progressBar.Value = 0;
                    statusMessageLabel.Text = $"Encryption failed: {ex.Message}";
                    statusMessageLabel.ForeColor = errorColor;

                    LogEvent("ERROR", $"Encryption failed: {ex.Message}", "Encryption");
                }
            };

            // Decrypt button logic
            decryptButton.Click += (s, e) => {
                if (string.IsNullOrEmpty(filePathTextBox.Text) || !File.Exists(filePathTextBox.Text))
                {
                    MessageBox.Show("Please select a valid file first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(passwordTextBox.Text))
                {
                    MessageBox.Show("Please enter a password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string inputFilePath = filePathTextBox.Text;
                string outputFilePath;

                // Check if the file has .encrypted extension
                if (inputFilePath.EndsWith(".encrypted"))
                {
                    outputFilePath = inputFilePath.Substring(0, inputFilePath.Length - ".encrypted".Length);
                }
                else
                {
                    outputFilePath = Path.Combine(
                        Path.GetDirectoryName(inputFilePath),
                        Path.GetFileNameWithoutExtension(inputFilePath) + "_decrypted" + Path.GetExtension(inputFilePath)
                    );
                }

                try
                {
                    statusMessageLabel.Text = "Decrypting file...";
                    statusMessageLabel.ForeColor = warningColor;
                    progressBar.Value = 0;
                    Application.DoEvents();

                    // Simple progress reporting
                    progressBar.Value = 10;
                    Application.DoEvents();

                    // Perform the decryption
                    DecryptFile(inputFilePath, outputFilePath, passwordTextBox.Text);

                    progressBar.Value = 100;
                    statusMessageLabel.Text = "Decryption complete! Output file: " + Path.GetFileName(outputFilePath);
                    statusMessageLabel.ForeColor = successColor;

                    LogEvent("INFO", $"File decrypted: {Path.GetFileName(inputFilePath)}", "Encryption");

                    // Ask if user wants to open the folder containing the decrypted file
                    if (MessageBox.Show("Decryption successful. Would you like to open the folder containing the decrypted file?",
                        "Decryption Complete", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        Process.Start("explorer.exe", "/select," + outputFilePath);
                    }
                }
                catch (CryptographicException)
                {
                    progressBar.Value = 0;
                    statusMessageLabel.Text = "Decryption failed: Incorrect password or the file is not encrypted.";
                    statusMessageLabel.ForeColor = errorColor;

                    LogEvent("WARNING", $"Decryption failed: Incorrect password or corrupt file - {Path.GetFileName(inputFilePath)}", "Encryption");
                }
                catch (Exception ex)
                {
                    progressBar.Value = 0;
                    statusMessageLabel.Text = $"Decryption failed: {ex.Message}";
                    statusMessageLabel.ForeColor = errorColor;

                    LogEvent("ERROR", $"Decryption failed: {ex.Message}", "Encryption");
                }
            };

            // Adjust UI on resize
            contentArea.Resize += (sender, e) => {
                // Resize header
                headerPanel.Width = contentArea.Width;

                // Resize panels
                fileSelectionPanel.Width = contentArea.Width - (2 * standardPadding);
                filePathTextBox.Width = fileSelectionPanel.Width - 400;
                browseButton.Location = new Point(filePathTextBox.Right + 10, 15);

                encryptionOptionsPanel.Width = contentArea.Width - (2 * standardPadding);

                statusPanel.Width = contentArea.Width - (2 * standardPadding);
                statusPanel.Height = contentArea.Height - encryptionOptionsPanel.Bottom - (2 * standardPadding);
                progressBar.Width = statusPanel.Width - 30;
            };
        }

        // File encryption methods
        private void EncryptFile(string inputFile, string outputFile, string password)
        {
            byte[] salt = new byte[16];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }

            // Derive key and IV from password and salt
            var key = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] keyBytes = key.GetBytes(32); // 256 bits for AES-256
            byte[] ivBytes = key.GetBytes(16);  // 128 bits for AES IV

            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
            {
                // Write the salt to the output file (needed for decryption)
                fsOutput.Write(salt, 0, salt.Length);

                using (var aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var cryptoStream = new CryptoStream(fsOutput, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = fsInput.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            cryptoStream.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        private void DecryptFile(string inputFile, string outputFile, string password)
        {
            using (FileStream fsInput = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
            {
                // Read the salt from the beginning of the file
                byte[] salt = new byte[16];
                fsInput.Read(salt, 0, salt.Length);

                // Derive key and IV from password and salt (same as encryption)
                var key = new Rfc2898DeriveBytes(password, salt, 10000);
                byte[] keyBytes = key.GetBytes(32); // 256 bits for AES-256
                byte[] ivBytes = key.GetBytes(16);  // 128 bits for AES IV

                using (FileStream fsOutput = new FileStream(outputFile, FileMode.Create, FileAccess.Write))
                using (var aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = ivBytes;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;

                    using (var cryptoStream = new CryptoStream(fsInput, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] buffer = new byte[4096];
                        int bytesRead;

                        while ((bytesRead = cryptoStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fsOutput.Write(buffer, 0, bytesRead);
                        }
                    }
                }
            }
        }

        // Helper method to get content area
        private Panel GetContentArea()
        {
            // Clear previous content
            contentPanel.Controls.Clear();

            // Create new content area
            Panel contentArea = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = lightColor
            };

            contentPanel.Controls.Add(contentArea);
            return contentArea;
        }
    }
}