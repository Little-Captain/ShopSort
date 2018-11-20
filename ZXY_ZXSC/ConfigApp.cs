using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace ZXY_ZXSC
{
    class ConfigApp
    {
        public static void addItem(string keyName, string keyValue)
        {
            if (existItem(keyName))
            {
                removeItem(keyName);
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add(keyName, keyValue);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static bool existItem(string keyName)
        {
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if (key == keyName)
                {
                    return true;
                }
            }
            return false;
        }

        public static string valueItem(string keyName)
        {
            return ConfigurationManager.AppSettings[keyName];
        }

        public static void modifyItem(string keyName, string keyValue)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keyName].Value = keyValue;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public static void removeItem(string keyName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(keyName);
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
