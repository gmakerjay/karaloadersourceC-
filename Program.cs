using System;
using System.Windows.Forms;

namespace KaraDownloader
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Logger.Info("========== Application Starting ==========");
            ApplicationConfiguration.Initialize();

            // Global exception handler – log before crash
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                Logger.Error("UNHANDLED UI THREAD EXCEPTION", e.Exception);
                MessageBox.Show($"เกิดข้อผิดพลาดร้ายแรง:\n{e.Exception.Message}\n\nดู log ที่:\n{Logger.GetLogPath()}",
                    "❌ Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex)
                    Logger.Error("UNHANDLED DOMAIN EXCEPTION", ex);
            };

            // Check license
            if (!AppConfig.HasValidLicense())
            {
                var existing = AppConfig.LoadLicense();
                if (existing != null && existing.IsActivated)
                {
                    Logger.Warn($"License expired on {existing.ExpiryDate:yyyy-MM-dd}");
                    MessageBox.Show(
                        "สิทธิ์การใช้งานหมดอายุแล้ว\nกรุณาติดต่อร้านเพื่อต่ออายุ\n\n📞 084-8400908",
                        "⚠️ หมดอายุการใช้งาน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                Logger.Info("No valid license – showing login form");
                using var login = new LoginForm();
                if (login.ShowDialog() != DialogResult.OK)
                {
                    Logger.Info("User cancelled login");
                    return;
                }
            }
            else
            {
                Logger.Info($"Valid license found – {AppConfig.GetDaysRemaining()} days remaining");
            }

            Logger.Info("Launching MainForm");
            Application.Run(new MainForm());
            Logger.Info("========== Application Exiting ==========");
        }
    }
}