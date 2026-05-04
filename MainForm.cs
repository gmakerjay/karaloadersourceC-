using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using SharpCompress.Archives;
using SharpCompress.Common;

namespace KaraDownloader
{
    public class MainForm : Form
    {
        private Button btnDownload;
        private Button btnHelp;
        private ProgressBar progressBar;
        private Label lblProgress;
        private Label lblTitle;
        private Label lblInfo;
        private Label lblVersion;
        private Label lblExpiry;
        private StatusStrip statusStrip;
        private ToolStripStatusLabel statusLabel;
        private Panel panelMain;
        private GistConfig? gistConfig;

        public MainForm()
        {
            Logger.Info("MainForm initializing...");
            InitUI();
            this.Shown += MainForm_Shown;
        }

        private void InitUI()
        {
            this.Text = "P-Computer Services - Extreme Karaoke Downloader";
            this.Size = new Size(620, 580);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(18, 18, 32);
            this.Font = new Font("Segoe UI", 12F);

            panelMain = new Panel { Size = new Size(560, 440), Location = new Point(20, 15), BackColor = Color.FromArgb(28, 28, 50) };
            this.Controls.Add(panelMain);

            lblTitle = new Label { Text = "🎤 Extreme Karaoke\nระบบดาวน์โหลด && แตกไฟล์อัตโนมัติ", Font = new Font("Segoe UI", 20F, FontStyle.Bold), ForeColor = Color.FromArgb(0, 210, 255), TextAlign = ContentAlignment.MiddleCenter, Size = new Size(520, 90), Location = new Point(20, 15) };
            panelMain.Controls.Add(lblTitle);

            lblInfo = new Label { Text = "📞 ติดต่อ: 084-8400908 | P-Computer Services", Font = new Font("Segoe UI", 12F), ForeColor = Color.FromArgb(180, 180, 200), TextAlign = ContentAlignment.MiddleCenter, Size = new Size(520, 30), Location = new Point(20, 110) };
            panelMain.Controls.Add(lblInfo);

            lblVersion = new Label { Text = "เวอร์ชัน: -", Font = new Font("Segoe UI", 11F), ForeColor = Color.FromArgb(120, 220, 120), Size = new Size(250, 25), Location = new Point(20, 148) };
            panelMain.Controls.Add(lblVersion);

            lblExpiry = new Label { Text = "สิทธิ์คงเหลือ: - วัน", Font = new Font("Segoe UI", 11F), ForeColor = Color.FromArgb(255, 200, 80), TextAlign = ContentAlignment.MiddleRight, Size = new Size(250, 25), Location = new Point(290, 148) };
            panelMain.Controls.Add(lblExpiry);

            btnDownload = new Button { Text = "⬇️  ดาวน์โหลด && ติดตั้ง", Font = new Font("Segoe UI", 18F, FontStyle.Bold), Size = new Size(520, 70), Location = new Point(20, 190), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(0, 160, 80), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnDownload.FlatAppearance.BorderSize = 0;
            btnDownload.Click += BtnDownload_Click;
            btnDownload.MouseEnter += (s, e) => btnDownload.BackColor = Color.FromArgb(0, 200, 100);
            btnDownload.MouseLeave += (s, e) => btnDownload.BackColor = Color.FromArgb(0, 160, 80);
            panelMain.Controls.Add(btnDownload);

            progressBar = new ProgressBar { Size = new Size(520, 35), Location = new Point(20, 275), Style = ProgressBarStyle.Continuous, Minimum = 0, Maximum = 100 };
            panelMain.Controls.Add(progressBar);

            lblProgress = new Label { Text = "พร้อมใช้งาน", Font = new Font("Segoe UI", 13F, FontStyle.Bold), ForeColor = Color.White, TextAlign = ContentAlignment.MiddleCenter, Size = new Size(520, 35), Location = new Point(20, 318) };
            panelMain.Controls.Add(lblProgress);

            btnHelp = new Button { Text = "❓  ขอความช่วยเหลือ (Facebook)", Font = new Font("Segoe UI", 14F, FontStyle.Bold), Size = new Size(520, 55), Location = new Point(20, 368), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(55, 80, 160), ForeColor = Color.White, Cursor = Cursors.Hand };
            btnHelp.FlatAppearance.BorderSize = 0;
            btnHelp.Click += BtnHelp_Click;
            btnHelp.MouseEnter += (s, e) => btnHelp.BackColor = Color.FromArgb(70, 100, 200);
            btnHelp.MouseLeave += (s, e) => btnHelp.BackColor = Color.FromArgb(55, 80, 160);
            panelMain.Controls.Add(btnHelp);

            statusStrip = new StatusStrip { BackColor = Color.FromArgb(15, 15, 28) };
            statusLabel = new ToolStripStatusLabel { Text = "พร้อมใช้งาน | P-Computer Services", ForeColor = Color.FromArgb(150, 150, 180), Font = new Font("Segoe UI", 10F) };
            statusStrip.Items.Add(statusLabel);
            this.Controls.Add(statusStrip);
        }

        // ───── Startup: fetch gist, show notifications ─────
        private async void MainForm_Shown(object? sender, EventArgs e)
        {
            int daysLeft = AppConfig.GetDaysRemaining();
            lblExpiry.Text = $"สิทธิ์คงเหลือ: {daysLeft} วัน";
            if (daysLeft <= 7) lblExpiry.ForeColor = Color.FromArgb(255, 80, 80);
            Logger.Info($"License days remaining: {daysLeft}");

            statusLabel.Text = "กำลังโหลดข้อมูลจากเซิร์ฟเวอร์...";
            gistConfig = await AppConfig.FetchGistConfigAsync();

            if (gistConfig == null)
            {
                Logger.Warn("Failed to fetch Gist config – offline mode");
                statusLabel.Text = "⚠️ ไม่สามารถเชื่อมต่อเซิร์ฟเวอร์ได้ (ออฟไลน์)";
                lblVersion.Text = $"เวอร์ชันติดตั้ง: {AppConfig.GetInstalledVersion()}";
                return;
            }
            Logger.Info($"Gist config loaded – version={gistConfig.app_meta.latest_version}");

            if (gistConfig.app_meta.maintenance_mode)
            {
                Logger.Warn("Server is in maintenance mode");
                MessageBox.Show("ระบบอยู่ระหว่างปรับปรุง กรุณาลองใหม่ภายหลัง", "🔧 ปิดปรับปรุงระบบ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                btnDownload.Enabled = false;
                statusLabel.Text = "ระบบปิดปรับปรุง";
                return;
            }

            string installed = AppConfig.GetInstalledVersion();
            lblVersion.Text = $"เวอร์ชันล่าสุด: {gistConfig.app_meta.latest_version} | ติดตั้ง: {(installed == "" ? "ยังไม่ติดตั้ง" : installed)}";

            foreach (var n in gistConfig.notifications)
            {
                if (n.show_on_startup)
                    MessageBox.Show(n.message, n.title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            statusLabel.Text = "พร้อมใช้งาน | P-Computer Services";
        }

        // ───── Download → Extract → Shortcut → Delete RAR ─────
        private async void BtnDownload_Click(object? sender, EventArgs e)
        {
            if (MessageBox.Show("ระบบจะทำงานอัตโนมัติ โปรดรอจนกว่าแถบสีเขียวจะเต็ม\n\nกด OK เพื่อเริ่มดาวน์โหลด",
                    "⬇️ เริ่มดาวน์โหลด", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) != DialogResult.OK)
                return;

            btnDownload.Enabled = false;
            btnHelp.Enabled = false;
            progressBar.Value = 0;
            string tempFile = "";

            try
            {
                // 1 – Folder
                string dest = AppConfig.DesktopFolder;
                Directory.CreateDirectory(dest);
                Logger.Info($"Destination folder ready: {dest}");
                statusLabel.Text = "สร้างโฟลเดอร์ ExtremeKara...";
                lblProgress.Text = "กำลังเตรียมระบบ...";

                // 2 – URL
                string url = gistConfig?.downloads.primary_url
                    ?? "https://github.com/gmakerjay/KARAOKELOADERFILE_6-4-69/releases/download/karaloader/eXtreme.Karaoke.rar";
                string fname = gistConfig?.downloads.filename ?? "eXtreme.Karaoke.rar";
                tempFile = Path.Combine(dest, fname);
                Logger.Info($"Download URL: {url}");
                Logger.Info($"RAR file will be saved to: {tempFile}");

                // 3 – Download
                statusLabel.Text = "กำลังดาวน์โหลด...";
                lblProgress.Text = "⬇️ กำลังดาวน์โหลด... 0%";
                bool ok = await DownloadFileAsync(url, tempFile);

                // Backup URLs
                if (!ok && gistConfig?.downloads.backup_urls != null)
                {
                    foreach (var bu in gistConfig.downloads.backup_urls)
                    {
                        if (string.IsNullOrEmpty(bu) || bu.Contains("link1.com") || bu.Contains("link2.com") || bu.Contains("link3.com"))
                            continue;
                        Logger.Info($"Trying backup URL: {bu}");
                        progressBar.Value = 0;
                        ok = await DownloadFileAsync(bu, tempFile);
                        if (ok) break;
                    }
                }

                if (!ok)
                {
                    Logger.Error("All download URLs failed");
                    MessageBox.Show("ไม่สามารถดาวน์โหลดไฟล์ได้ กรุณาตรวจสอบอินเทอร์เน็ต\nหรือติดต่อ 084-8400908",
                        "❌ ดาวน์โหลดล้มเหลว", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Logger.Info("Download completed successfully");

                // 4 – Extract
                statusLabel.Text = "กำลังแตกไฟล์...";
                lblProgress.Text = "📦 กำลังแตกไฟล์... โปรดรอ";
                progressBar.Value = 0;
                Logger.Info("Starting RAR extraction...");
                await Task.Run(() => ExtractRar(tempFile, dest));
                Logger.Info("Extraction completed");

                // 5 – Shortcut
                Logger.Info("Creating desktop shortcut...");
                statusLabel.Text = "สร้างทางลัด...";
                CreateShortcut(dest);
                Logger.Info("Shortcut created");

                // 6 – Version file
                string ver = gistConfig?.app_meta.latest_version ?? "1.0";
                AppConfig.SaveVersionFile(ver);
                lblVersion.Text = $"เวอร์ชันล่าสุด: {ver} | ติดตั้ง: {ver}";
                Logger.Info($"Version file saved: {ver}");

                // 7 – DELETE RAR
                DeleteTempFile(tempFile);
                tempFile = ""; // prevent double-delete in finally

                // Done
                progressBar.Value = 100;
                lblProgress.Text = "✅ ดำเนินการเสร็จสิ้น 100%";
                statusLabel.Text = "ติดตั้งเสร็จสมบูรณ์!";
                Logger.Info("=== Installation completed successfully ===");

                MessageBox.Show("ดำเนินการเสร็จสิ้น!\n\nไฟล์ถูกติดตั้งที่: Desktop\\ExtremeKara\nShortcut ถูกสร้างเรียบร้อยแล้ว",
                    "✅ สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Close app after successful install
                Logger.Info("User clicked OK – closing application");
                Application.Exit();
            }
            catch (Exception ex)
            {
                Logger.Error("Unhandled error during download/install", ex);
                MessageBox.Show($"เกิดข้อผิดพลาด:\n{ex.Message}\n\nกรุณาติดต่อ 084-8400908\n\nดูรายละเอียดที่:\n{Logger.GetLogPath()}",
                    "❌ ข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
                statusLabel.Text = "เกิดข้อผิดพลาด";
            }
            finally
            {
                // Always try to clean up temp RAR even on error
                if (!string.IsNullOrEmpty(tempFile)) DeleteTempFile(tempFile);
                btnDownload.Enabled = true;
                btnHelp.Enabled = true;
            }
        }

        // ───── Helpers ─────

        private void DeleteTempFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                    Logger.Info($"Temp RAR deleted: {path}");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to delete temp RAR: {path}", ex);
            }
        }

        private async Task<bool> DownloadFileAsync(string url, string destPath)
        {
            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromMinutes(30);
                Logger.Info($"HTTP GET {url}");
                using var resp = await http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                resp.EnsureSuccessStatusCode();

                long? total = resp.Content.Headers.ContentLength;
                Logger.Info($"Content-Length: {total?.ToString() ?? "unknown"}");
                using var src = await resp.Content.ReadAsStreamAsync();
                using var dst = new FileStream(destPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192);

                byte[] buf = new byte[8192];
                long read = 0;
                int n;
                while ((n = await src.ReadAsync(buf, 0, buf.Length)) > 0)
                {
                    await dst.WriteAsync(buf, 0, n);
                    read += n;
                    if (total > 0)
                    {
                        int pct = (int)((read * 100) / total.Value);
                        this.Invoke(() =>
                        {
                            progressBar.Value = Math.Min(pct, 100);
                            lblProgress.Text = $"⬇️ กำลังดาวน์โหลด... {pct}%  ({read / 1048576}MB / {total.Value / 1048576}MB)";
                            statusLabel.Text = $"ดาวน์โหลด {pct}%";
                        });
                    }
                }
                Logger.Info($"Download finished – {read} bytes written");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Download failed for {url}", ex);
                return false;
            }
        }

        private void ExtractRar(string rarPath, string destFolder)
        {
            try
            {
                var opts = new SharpCompress.Readers.ReaderOptions { Password = AppConfig.RAR_PASSWORD };
                Logger.Info("Trying extraction with password...");
                using var arc = ArchiveFactory.Open(rarPath, opts);
                int total = 0;
                foreach (var _ in arc.Entries) total++;

                using var arc2 = ArchiveFactory.Open(rarPath, opts);
                int cur = 0;
                foreach (var entry in arc2.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        entry.WriteToDirectory(destFolder, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                    }
                    cur++;
                    int pct = total > 0 ? (cur * 100) / total : 0;
                    this.Invoke(() =>
                    {
                        progressBar.Value = Math.Min(pct, 100);
                        lblProgress.Text = $"📦 กำลังแตกไฟล์... {pct}%  ({cur}/{total})";
                        statusLabel.Text = $"แตกไฟล์ {pct}%";
                    });
                }
                Logger.Info($"Extracted {cur} entries with password");
            }
            catch (Exception ex1)
            {
                Logger.Warn($"Password extraction failed: {ex1.Message} – retrying without password");
                try
                {
                    using var arc = ArchiveFactory.Open(rarPath);
                    int cur = 0;
                    foreach (var entry in arc.Entries)
                    {
                        if (!entry.IsDirectory)
                            entry.WriteToDirectory(destFolder, new ExtractionOptions { ExtractFullPath = true, Overwrite = true });
                        cur++;
                    }
                    Logger.Info($"Extracted {cur} entries without password");
                }
                catch (Exception ex2)
                {
                    Logger.Error("Extraction failed completely", ex2);
                    throw;
                }
            }
        }

        private void CreateShortcut(string targetFolder)
        {
            try
            {
                string exePath = Path.Combine(targetFolder, "eXtreme_Karaoke.exe");
                string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string lnk = Path.Combine(desktop, "eXtreme Karaoke.lnk");

                string ps = $@"$ws = New-Object -ComObject WScript.Shell; $sc = $ws.CreateShortcut('{lnk.Replace("'", "''")}'); $sc.TargetPath = '{exePath.Replace("'", "''")}'; $sc.WorkingDirectory = '{targetFolder.Replace("'", "''")}'; $sc.Description = 'eXtreme Karaoke'; $sc.Save()";
                var psi = new ProcessStartInfo("powershell", $"-NoProfile -Command \"{ps}\"") { CreateNoWindow = true, UseShellExecute = false };
                Process.Start(psi)?.WaitForExit(5000);
                Logger.Info($"Shortcut created at {lnk}");
            }
            catch (Exception ex)
            {
                Logger.Error("Shortcut creation failed", ex);
            }
        }

        private void BtnHelp_Click(object? sender, EventArgs e)
        {
            string url = gistConfig?.external_links.fb_support ?? AppConfig.FB_SUPPORT;
            Logger.Info("Help button clicked");
            if (MessageBox.Show($"📞 เบอร์ติดต่อ: {AppConfig.PHONE_NUMBER}\n\n🌐 Facebook: P-Computer Services\n\nกด OK เพื่อเปิด Facebook",
                    "❓ ขอความช่วยเหลือ", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                try { Process.Start(new ProcessStartInfo(url) { UseShellExecute = true }); }
                catch (Exception ex) { Logger.Error("Failed to open FB URL", ex); }
            }
        }
    }
}
