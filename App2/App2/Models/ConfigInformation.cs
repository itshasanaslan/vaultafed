using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace App2.Models
{
    public class ConfigInformation
    {
        public string Language { get; set; }
        public string FilePath { get; set; }

        public ConfigInformation()
        {
            this.FilePath = ConfigInformation.GetConfigPath();
        }


        public static string GetConfigPath()
        {
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            return Path.Combine(folder, "config.json");
        }

        public static void Save(ConfigInformation config)
        {
            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(config.FilePath, json);
        }

        public static ConfigInformation Load(string filePath)
        {
            try
            {
                string json = File.ReadAllText(filePath);
                ConfigInformation config = JsonConvert.DeserializeObject<ConfigInformation>(json);
                if (config.Language == "" || config.Language == null) throw new Exception("");
                return config;
            }
            catch
            {
                ConfigInformation c2 = new ConfigInformation();
                c2.Language = "English";
                return c2;
            }
        }
    }
}
