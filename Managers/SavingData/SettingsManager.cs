using Bound.Models;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Bound.Managers
{
    public class SettingsManager
    {
        private static string _filename = "settings.txt"; //since we dont give a path this'll go to the bin folder
        public SettingsModel Settings { get; private set; }

        public Dictionary<string, string> DefaultInputValues = new Dictionary<string, string>()
        {
            {"Up", "W" },
            {"Down", "S" },
            {"Left", "A" },
            {"Right", "D" },
            {"Jump", "SPACE" },
            {"Use", "M1" },
            {"Dash", "LSHFT" },
            {"Reset", "R" },
            {"Menu", "TAB"},
            {"Hotbar 1", "1"},
            {"Hotbar 2", "2"},
            {"Hotbar 3", "3"},
            {"Hotbar 4", "4"},
            {"Hotbar 5", "5"},
            {"Hotbar 6", "6"},
        };

        public SettingsManager()
        {
            Init();
        }

        public SettingsManager(string output)
        {
            try
            {
                var settings = output.ReplaceLineEndings().Split("\r\n\r\n")
                                .Select(x => x.Split("\r\n"))
                                    .Select(x => x.Select(y => y.Split(": ").ToList()).ToList()).ToList();

                Settings = new SettingsModel()
                {
                    General = new Dictionary<string, string>(),
                    InputValues = new Dictionary<string, string>()
                };

                foreach (var kvp in settings[0])
                {
                    Settings.General.Add(kvp[0], kvp[1]);
                }

                foreach (var kvp in settings[1])
                {
                    if (kvp.Count == 1)
                    {
                        break;
                    }
                    Settings.InputValues.Add(kvp[0], kvp[1]);
                }

                foreach (var kvp in DefaultInputValues)
                {
                    if (!Settings.InputValues.ContainsKey(kvp.Key))
                        Settings.InputValues.Add(kvp.Key, kvp.Value);
                }
            }
            catch (Exception)
            {
                Init();
            }

        }

        public void Init()
        {
            Settings = new SettingsModel()
            {
                General = new Dictionary<string, string>()
                {
                    { "Resolution", GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width.ToString() + "x" + GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height.ToString() },
                    { "Fullscreen", "Yes" },
                    { "MasterVolume", "50" },
                    { "MusicVolume", "50" },
                    { "EnemyVolume", "50" },
                    { "PlayerVolume", "50" },
                    { "MostRecentSave", "-1" }
                },
                InputValues = DefaultInputValues
            };
        }

        public static SettingsManager Load()
        {
            if (!File.Exists(_filename))
                return new SettingsManager(); //create new instance if it doesn't exist

            using (var reader = new StreamReader(new FileStream(_filename, FileMode.Open)))
            {
                return new SettingsManager(reader.ReadToEnd());
            }
        }

        public static void Save(SettingsManager manager)
        {
            //Overrides the file if it already exists
            using (var writer = new StreamWriter(new FileStream(_filename, FileMode.Create)))
            {
                writer.Write(manager.Settings);
            }
        }

        public void UpdateResolution(int width, int height)
        {
            Settings.General["Resolution"] = width.ToString() + "x" + height.ToString();
        }

    }
}

