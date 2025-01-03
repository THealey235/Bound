using Bound.Models;
using Bound.States;
using Bound.States.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Managers
{
    public class SaveManager
    {

        private Game1 _game;

        public KeyValuePair<string, List<int>> LevelMap = new KeyValuePair<string, List<int>>("Level", new List<int>() { 1, 9, 4, 7, 3, 2, 14, 25, 12, 17, 19, 22 } );
        public Dictionary<string, List<int>> AttributeMap = new Dictionary<string, List<int>>()
        {
            { "HP", new List<int>() { 4, 7 , 2, 3 } },
            { "MoveSpeed", new List<int>() { 2, 9, 7 , 4 } },
            { "MP", new List<int>() { 7, 9, 8, 1 } },
            { "Stamina", new List<int>() { 2, 3, 1, 5} },
            { "Dashes", new List<int>() { 1, 4, 7 , 6} },
            { "Level", new List<int>() { 1, 9, 4, 7, 3, 2, 14, 25, 12} },
            { "Season Attunement", new List<int>{ 5 } },
            { "Strength", new List<int>{8, 5} },
            { "Dexterity" , new List<int>{9, 2} },
            { "Ammo Handling", new List<int>{6, 1} },
            { "Precision", new List<int>{10, 7} },
            { "Arcane", new List<int>{14, 5} },
            { "Focus", new List<int>{10, 4} },
        };

        public Dictionary<string, int> DefaultAttributes = new Dictionary<string, int>
        {
            { "Ammo Handling", 5 },
            { "Arcane", 5 },
            { "Dexterity" , 5 },
            { "Focus", 0 },
            { "HP", 75},
            { "MoveSpeed", 10 },
            { "MP", 50 },
            { "Precision", 5 },
            { "Season Attunement", 0 },
            { "Stamina", 100 },
            { "Strength", 5 },
        };


        public static List<int> InventoryCode
        {
            get
            {
                return new List<int>() { 15, 6, 2, 10 };
            }
        }

        public Save ActiveSave
        {
            get
            {
                return Saves[_game.SaveIndex];
            }
        }

        public List<Save> Saves;

        public SaveManager(Game1 game)
        {
            _game = game;
            Saves = new List<Save>() { null, null, null, null, null };

            //It already checks inside the method if it exists or not
            System.IO.Directory.CreateDirectory("Saves");

            LoadSaves();

            //if the most recently accesed save doesn't exist remove the continue button
            int index = int.Parse(game.Settings.Settings.General["MostRecentSave"]);
            if (index != -1 && Saves[index] == null)
            {
                game.Settings.Settings.General["MostRecentSave"] = "-1";
            }
        }

        #region Save Management
        private void LoadSaves()
        {
            // i < Max number of possible saves
            string path;
            for (int i = 0; i < 5; i++)
            {
                path = "Saves/Save" + i.ToString() + ".wld";
                if (File.Exists(path))
                {
                    using (var reader = new StreamReader(new FileStream(path, FileMode.Open)))
                    {
                        Saves[i] = (Save.ImportSave(this, reader.ReadToEnd().Split("\n").ToList(), _game));
                    }
                }
            }
        }

        public void UploadSave(int index)
        {
            //negative index means start couting from last index, python type shit
            if (index < 0)
            {
                index = Saves.Count + index;
            }

            var save = Saves[index];
            var path = GetPath(index);

            if (Saves[index] == null)
            {
                File.Delete(path);
                return;
            }

            using (var writer = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                writer.Write(save);
            }
        }

        public void UploadAll()
        {
            for (int i = 0; i < Saves.Count; i++)
            {
                if (Saves[i] == null)
                {
                    File.Delete(GetPath(i));
                }
                UploadSave(i);
            }
        }

        private static string GetPath(int i)
        {
            return "Saves/Save" + i.ToString() + ".wld";
        }

        public State GetState(int saveIndex, Game1 game, ContentManager content, GraphicsDeviceManager graphics)
        {
            //removes the save file if it doesn't exist
            if (Saves[saveIndex] == null)
            {
                game.RecentSave = -1;
                game.Settings.Settings.General["MostRecentSave"] = "-1";
                SettingsManager.Save(game.Settings);
                return new MainMenu(game, content, graphics);
            }

            return Saves[saveIndex].Level switch
            {
                "newgame" => new CharacterInit(game, content, saveIndex),
                "levelzero" => new Level0(game, content, _game.Player),
                _ => new MainMenu(game, content, graphics)
            };
        }

        #endregion
    }
}
