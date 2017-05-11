using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
namespace uTikDownloadHelper
{
    public class Settings
    {
        private IniFile ini;
        private String section = "Settings";

        

        public Settings()
        {
            if (!Directory.Exists(Common.SettingsPath)) Directory.CreateDirectory(Common.SettingsPath);

            ini = new IniFile(Path.Combine(Common.SettingsPath, "settings.ini"));
        }

        public String lastSelectedRegion
        {
            get
            {
                return ini.GetString(section, "lastSelectedRegion", "Any");
            }
            set
            {
                ini.WriteString(section, "lastSelectedRegion", value);
            }
        }
        public String ticketWebsite
        {
            get
            {
                return ini.GetString(section, "ticketWebsite", "");
            }
            set
            {
                ini.WriteString(section, "ticketWebsite", value);
            }
        }
        public String lastPath
        {
            get
            {
                return ini.GetString(section, "lastPath", "Any");
            }
            set
            {
                ini.WriteString(section, "lastPath", value);
            }
        }

        public int downloadTries
        {
            get
            {
                return ini.GetInteger(section, "downloadTries", 5);
            }
            set
            {
                ini.WriteInteger(section, "downloadTries", value);
            }
        }

        public bool hideWget
        {
            get
            {
                return ini.GetBoolean(section, "hideWget", true);
            }
            set
            {
                ini.WriteBoolean(section, "wgetDebug", value);
            }
        }

        public bool shellExecute
        {
            get
            {
                return ini.GetBoolean(section, "shellExecute", false);
            }
            set
            {
                ini.WriteBoolean(section, "shellExecute", value);
            }
        }

        public Dictionary<string, string> cachedSizes
        {
            get
            {
                var iniString = ini.GetString(section, "cachedSizes", "");
                if (iniString != null && iniString.Length > 0)
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(iniString);

                return new Dictionary<string, string>();
            }
            set
            {
                ini.WriteString(section, "cachedSizes", JsonConvert.SerializeObject(value, Formatting.None));
            }
        }
    }
}
