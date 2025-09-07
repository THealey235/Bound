using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Models
{
    public class SpriteTextures
    {
        private int _entityWidth = 0;

        public Dictionary<string, Texture2D> Statics = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Sheets = new Dictionary<string, Texture2D>();
        public int Width
        {
            get { return _entityWidth; }
        }

        public SpriteTextures(ContentManager content, List<string> statics, List<string> sheets)
        {
            foreach (var texture in statics)
                Statics.Add(texture.Split("\\")[2], content.Load<Texture2D>(texture));

            if (statics.Count > 0)
                _entityWidth = Statics[Statics.Keys.First()].Width;

            foreach (var texture in sheets)
                Sheets.Add(texture.Split("\\")[2].Replace("-Sheet", string.Empty), content.Load<Texture2D>(texture));
        }

        public int FrameCount(string key)
        {
            if (!Sheets.ContainsKey(key))
                return 1;
            else
                return Sheets[key].Width / _entityWidth;
        }
    }
}
