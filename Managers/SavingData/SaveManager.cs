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

        public Dictionary<string, int> DefaultAttributes = new Dictionary<string, int>
        {
            { "Ammo Handling", 5 },
            { "Arcane", 5 },
            { "Dexterity" , 5 },
            { "Focus", 0 },
            { "Vigor", 8},
            { "MoveSpeed", 10 },
            { "Mind", 8 },
            { "Precision", 5 },
            { "Season Attunement", 0 },
            { "Endurance", 8 },
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
                index += Saves.Count;

            var save = Saves[index];
            var path = GetPath(index);


            if (Saves[index] == null)
            {
                File.Delete(path);
                return;
            }

            if (index == _game.SaveIndex)
                save.Position = _game.Player.Position;

            using (var writer = new StreamWriter(new FileStream(path, FileMode.Create)))
            {
                writer.Write(save);//save.ToString() is called
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

            _game.Player.UpdateAttributes(saveIndex);
            _game.Player.Position = Saves[saveIndex].Position;

            return Saves[saveIndex].Level switch
            {
                "newgame" => new CharacterInit(game, content, saveIndex),
                "levelzero" => new Level0(game, content),
                _ => new MainMenu(game, content, graphics)
            };
        }

        #endregion
    }
}
