using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace KaraDownloader
{
    /// <summary>
    /// Login form - first-time activation with password and license duration selection
    /// </summary>
    public class LoginForm : Form
    {
        private TextBox txtPassword;
        private ComboBox cboLicenseDays;
        private Button btnLogin;
        private Label lblTitle;
        private Label lblPassword;
        private Label lblLicense;
        private Label lblStatus;
        private Panel panelMain;

        public LoginForm()
        {
            Logger.Info("LoginForm opened – awaiting credentials");
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // ====== Form Settings ======
            this.Text = "P-Computer Services - เข้าสู่ระบบ";
            this.Size = new Size(520, 520);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(20, 20, 35);
            this.Font = new Font("Segoe UI", 12F, FontStyle.Regular);

            // ====== Main Panel with gradient-like styling ======
            panelMain = new Panel();
            panelMain.Size = new Size(440, 420);
            panelMain.Location = new Point(30, 30);
            panelMain.BackColor = Color.FromArgb(30, 30, 55);
            panelMain.Paint += PanelMain_Paint;
            this.Controls.Add(panelMain);

            // ====== Title ======
            lblTitle = new Label();
            lblTitle.Text = "🎤 Extreme Karaoke\nระบบดาวน์โหลดอัตโนมัติ";
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(0, 200, 255);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            lblTitle.Size = new Size(400, 80);
            lblTitle.Location = new Point(20, 20);
            panelMain.Controls.Add(lblTitle);

            // ====== Password Label ======
            lblPassword = new Label();
            lblPassword.Text = "🔑 รหัสผ่าน:";
            lblPassword.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblPassword.ForeColor = Color.White;
            lblPassword.Size = new Size(200, 35);
            lblPassword.Location = new Point(30, 120);
            panelMain.Controls.Add(lblPassword);

            // ====== Password TextBox ======
            txtPassword = new TextBox();
            txtPassword.Font = new Font("Segoe UI", 16F);
            txtPassword.Size = new Size(380, 40);
            txtPassword.Location = new Point(30, 160);
            txtPassword.PasswordChar = '●';
            txtPassword.BackColor = Color.FromArgb(45, 45, 75);
            txtPassword.ForeColor = Color.White;
            txtPassword.BorderStyle = BorderStyle.FixedSingle;
            txtPassword.TextAlign = HorizontalAlignment.Center;
            panelMain.Controls.Add(txtPassword);

            // ====== License Duration Label ======
            lblLicense = new Label();
            lblLicense.Text = "📅 เลือกอายุการใช้งาน:";
            lblLicense.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblLicense.ForeColor = Color.White;
            lblLicense.Size = new Size(380, 35);
            lblLicense.Location = new Point(30, 215);
            panelMain.Controls.Add(lblLicense);

            // ====== License ComboBox ======
            cboLicenseDays = new ComboBox();
            cboLicenseDays.Font = new Font("Segoe UI", 16F);
            cboLicenseDays.Size = new Size(380, 40);
            cboLicenseDays.Location = new Point(30, 255);
            cboLicenseDays.DropDownStyle = ComboBoxStyle.DropDownList;
            cboLicenseDays.BackColor = Color.FromArgb(45, 45, 75);
            cboLicenseDays.ForeColor = Color.White;
            cboLicenseDays.Items.AddRange(new object[] {
                "30 วัน (1 เดือน)",
                "60 วัน (2 เดือน)",
                "90 วัน (3 เดือน)",
                "160 วัน (5+ เดือน)",
                "360 วัน (1 ปี)"
            });
            cboLicenseDays.SelectedIndex = 0;
            panelMain.Controls.Add(cboLicenseDays);

            // ====== Login Button ======
            btnLogin = new Button();
            btnLogin.Text = "🔓  เข้าสู่ระบบ";
            btnLogin.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            btnLogin.Size = new Size(380, 55);
            btnLogin.Location = new Point(30, 315);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.BackColor = Color.FromArgb(0, 150, 200);
            btnLogin.ForeColor = Color.White;
            btnLogin.Cursor = Cursors.Hand;
            btnLogin.Click += BtnLogin_Click;
            btnLogin.MouseEnter += (s, e) => btnLogin.BackColor = Color.FromArgb(0, 180, 240);
            btnLogin.MouseLeave += (s, e) => btnLogin.BackColor = Color.FromArgb(0, 150, 200);
            panelMain.Controls.Add(btnLogin);

            // ====== Status Label ======
            lblStatus = new Label();
            lblStatus.Text = "";
            lblStatus.Font = new Font("Segoe UI", 11F);
            lblStatus.ForeColor = Color.FromArgb(255, 100, 100);
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            lblStatus.Size = new Size(380, 30);
            lblStatus.Location = new Point(30, 380);
            panelMain.Controls.Add(lblStatus);

            // Enter key on password triggers login
            txtPassword.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    BtnLogin_Click(s, e);
                }
            };
        }

        private void PanelMain_Paint(object? sender, PaintEventArgs e)
        {
            // Draw rounded border
            using var pen = new Pen(Color.FromArgb(0, 150, 200), 2);
            var rect = new Rectangle(1, 1, panelMain.Width - 3, panelMain.Height - 3);
            using var path = RoundedRect(rect, 15);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawPath(pen, path);
        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void BtnLogin_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                Logger.Warn("Login attempt with empty password");
                lblStatus.Text = "⚠️ กรุณาใส่รหัสผ่าน";
                return;
            }

            if (txtPassword.Text.Trim() != AppConfig.APP_PASSWORD)
            {
                Logger.Warn("Login attempt with wrong password");
                lblStatus.Text = "❌ รหัสผ่านไม่ถูกต้อง";
                txtPassword.SelectAll();
                txtPassword.Focus();
                return;
            }

            // Parse license days from combobox
            int[] dayOptions = { 30, 60, 90, 160, 360 };
            int selectedDays = dayOptions[cboLicenseDays.SelectedIndex];

            Logger.Info($"Login OK – license {selectedDays} days, machine={AppConfig.GetMachineId()}");

            // Create and save license
            var license = new LicenseData
            {
                MachineId = AppConfig.GetMachineId(),
                ActivatedDate = DateTime.Now,
                LicenseDays = selectedDays,
                ExpiryDate = DateTime.Now.AddDays(selectedDays),
                IsActivated = true
            };
            AppConfig.SaveLicense(license);

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
