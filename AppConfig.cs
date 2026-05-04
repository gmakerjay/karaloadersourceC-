using System;
using System.IO;
using System.Management;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace KaraDownloader
{
    /// <summary>
    /// Handles license persistence, machine ID, and gist config loading
    /// </summary>
    public static class AppConfig
    {
        // Gist URL for remote config
        public const string GIST_URL = "https://gist.githubusercontent.com/gmakerjay/5c8d5eed7686bacbf4f69d2f35d49063/raw/a8262527eb85ed4825b89712122300b8e19c0daf/gistfile1.txt";
        public const string APP_PASSWORD = "p1234455";
        public const string RAR_PASSWORD = "1234";
        public const string PHONE_NUMBER = "084-8400908";
        public const string FB_SUPPORT = "https://www.facebook.com/profile.php?id=100076719678393";

        private static readonly string AppDataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "KaraDownloader");

        private static readonly string LicenseFile = Path.Combine(AppDataPath, "license.dat");

        public static string DesktopFolder => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "ExtremeKara");

        /// <summary>
        /// Generate a unique machine identifier based on hardware
        /// </summary>
        public static string GetMachineId()
        {
            try
            {
                string raw = Environment.MachineName + "|" + Environment.UserName;

                // Try to get CPU ID for stronger identification
                try
                {
                    using var searcher = new ManagementObjectSearcher("SELECT ProcessorId FROM Win32_Processor");
                    foreach (var obj in searcher.Get())
                    {
                        raw += "|" + obj["ProcessorId"]?.ToString();
                        break;
                    }
                }
                catch { /* WMI may not be available */ }

                using var sha = SHA256.Create();
                var hash = sha.ComputeHash(Encoding.UTF8.GetBytes(raw));
                return Convert.ToBase64String(hash).Substring(0, 16);
            }
            catch
            {
                return Environment.MachineName;
            }
        }

        /// <summary>
        /// Save license data locally (encrypted-ish via base64 for simple protection)
        /// </summary>
        public static void SaveLicense(LicenseData data)
        {
            Directory.CreateDirectory(AppDataPath);
            string json = JsonConvert.SerializeObject(data);
            string encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
            File.WriteAllText(LicenseFile, encoded);
        }

        /// <summary>
        /// Load license data from local storage
        /// </summary>
        public static LicenseData? LoadLicense()
        {
            try
            {
                if (!File.Exists(LicenseFile)) return null;
                string encoded = File.ReadAllText(LicenseFile);
                string json = Encoding.UTF8.GetString(Convert.FromBase64String(encoded));
                return JsonConvert.DeserializeObject<LicenseData>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Check if a valid (non-expired) license exists for this machine
        /// </summary>
        public static bool HasValidLicense()
        {
            var license = LoadLicense();
            if (license == null || !license.IsActivated) return false;
            if (license.MachineId != GetMachineId()) return false;
            if (DateTime.Now > license.ExpiryDate) return false;
            return true;
        }

        /// <summary>
        /// Get days remaining on license
        /// </summary>
        public static int GetDaysRemaining()
        {
            var license = LoadLicense();
            if (license == null) return 0;
            var remaining = (license.ExpiryDate - DateTime.Now).Days;
            return remaining > 0 ? remaining : 0;
        }

        /// <summary>
        /// Fetch remote Gist config
        /// </summary>
        public static async Task<GistConfig?> FetchGistConfigAsync()
        {
            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(15);
                string json = await http.GetStringAsync(GIST_URL);
                return JsonConvert.DeserializeObject<GistConfig>(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Save version file after successful installation
        /// </summary>
        public static void SaveVersionFile(string version)
        {
            string versionFile = Path.Combine(DesktopFolder, "version.txt");
            File.WriteAllText(versionFile, version);
        }

        /// <summary>
        /// Read installed version
        /// </summary>
        public static string GetInstalledVersion()
        {
            string versionFile = Path.Combine(DesktopFolder, "version.txt");
            if (File.Exists(versionFile))
                return File.ReadAllText(versionFile).Trim();
            return "";
        }
    }
}
