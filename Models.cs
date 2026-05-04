using System;
using System.Collections.Generic;

namespace KaraDownloader
{
    /// <summary>
    /// Data models matching the Gist JSON structure
    /// </summary>
    public class GistConfig
    {
        public AppMeta app_meta { get; set; } = new AppMeta();
        public List<Notification> notifications { get; set; } = new List<Notification>();
        public Downloads downloads { get; set; } = new Downloads();
        public ExternalLinks external_links { get; set; } = new ExternalLinks();
    }

    public class AppMeta
    {
        public string latest_version { get; set; } = "1.0";
        public bool force_update { get; set; } = false;
        public bool maintenance_mode { get; set; } = false;
    }

    public class Notification
    {
        public bool show_on_startup { get; set; } = true;
        public string title { get; set; } = "";
        public string message { get; set; } = "";
    }

    public class Downloads
    {
        public string filename { get; set; } = "";
        public string primary_url { get; set; } = "";
        public List<string> backup_urls { get; set; } = new List<string>();
    }

    public class ExternalLinks
    {
        public string fb_support { get; set; } = "";
        public string fb_video { get; set; } = "";
    }

    /// <summary>
    /// Local license / machine registration data
    /// </summary>
    public class LicenseData
    {
        public string MachineId { get; set; } = "";
        public DateTime ActivatedDate { get; set; }
        public int LicenseDays { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsActivated { get; set; } = false;
    }
}
