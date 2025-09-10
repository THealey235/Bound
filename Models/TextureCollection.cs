using Bound.Managers;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;

namespace Bound.Models
{
    public class TextureCollection
    {
        private int _defaultWidth = 0;

        public static Texture2D MissingItemTexture;
        public Texture2D BlankItemTexture;
        public Dictionary<string, Texture2D> Statics = new Dictionary<string, Texture2D>();
        public Dictionary<string, Texture2D> Sheets = new Dictionary<string, Texture2D>();
        public Dictionary<string, int> SheetEntityWidths = new Dictionary<string, int>(); //this gives the width of each frame for a sheet.

        public int DefaultWidth
        {
            get { return _defaultWidth; }
            set
            {
                if (value >= 0)
                _defaultWidth = value;
            }
        }

        public TextureCollection(ContentManager content, Dictionary<string, string> statics, Dictionary<string, string> sheets)
        {
            Statics = statics.ToDictionary(x => x.Key, x => content.Load<Texture2D>((x.Value).Contains("Content/") ? x.Value.Replace("Content/", string.Empty) : x.Value));
            Sheets = sheets.ToDictionary(x => x.Key, x => content.Load<Texture2D>((x.Value).Contains("Content/") ? x.Value.Replace("Content/", string.Empty) : x.Value));

            if (statics.Count > 0)
                _defaultWidth = Statics[Statics.Keys.First()].Width;
        }
        public TextureCollection(ContentManager content, Dictionary<string, string> statics, Dictionary<string, string> sheets, Dictionary<string, int> sheetEntityWidths)
            :this(content, statics, sheets)
        {
            SheetEntityWidths = sheetEntityWidths;
        }

        public TextureCollection()
        {
            //creates a blank texture collection
        }

        public int FrameCount(string key)
        {
            var width = SheetEntityWidths.ContainsKey(key) ? SheetEntityWidths[key] : _defaultWidth;

            if (!Sheets.ContainsKey(key) || width == 0)
                return 1;
            else
                return Sheets[key].Width / width;
        }

        public Texture2D GetIcon(bool nullAsMissing = true) 
            => Statics.ContainsKey("Icon") ? Statics["Icon"] : (nullAsMissing || BlankItemTexture != null) ? MissingItemTexture : BlankItemTexture;

    }
}
