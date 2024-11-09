using Bound.Models;
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
        public Dictionary<string, List<int>> AttributeMap = new Dictionary<string, List<int>>()
        {
            { "Health", new List<int>() { 4, 7 , 2, 3 } },
            { "MoveSpeed", new List<int>() { 2, 9, 7 , 4 } },
            { "Mana", new List<int>() { 7, 9, 8, 1 } },
            { "Stamina", new List<int>() { 2, 3, 1, 5} },
            { "Dashes", new List<int>() { 1, 4, 7 , 6} },
            { "Level", new List<int>() { 1, 9 , 4, 7, 3, 2, 14, 25, 12, 17, 19, 22} },

        };

        public Dictionary<string, int> DefaultAttributes = new Dictionary<string, int>
        {
            { "Health", 75},
            { "MoveSpeed", 10 },
            { "Mana", 50 },
            { "Stamina", 100 },
            { "Dashes", 0 },
        };

        public List<Save> Saves;

        public SaveManager()
        {
            Saves = new List<Save>() { null, null, null, null, null };

            //It already checks inside the method if it exists or not
            System.IO.Directory.CreateDirectory("Saves");

            LoadSaves();
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
                        Saves[i] = (Save.ImportSave(this, reader.ReadToEnd().Split("\n").ToList()));
                    }
                }
            }
        }

        public void UploadSave(int index)
        {
            //negative index means start from last index
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

        #endregion
    }
}
