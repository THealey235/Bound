using Bound.Controls;
using Bound.Controls.Game;
using Bound.Controls.Settings;
using Bound.Models.Items;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Bound.States.Game
{
    public class CharacterInit : State
    {
        private List<Component> _components;
        private List<MultiChoiceBox> _choiceBoxes;
        private MultiChoiceBox _difficulty;
        private SpriteFont _font;
        private List<Vector2> _leftSidePositions;
        private Color _penColour;
        private Dictionary<string, int> _attributes;
        private string _seasonAttunement;

        private List<string> _rawRows;

        private List<string> _rows
        {
            get
            {
                _rawRows[0] = "Name : " + (_components[0] as TextInput).Text;
                string rowIdentifier;
                for (int i = 1; i < _rawRows.Count; i++)
                {
                    if (_rawRows[i] == " ")
                        continue;

                    rowIdentifier = _rawRows[i].Split(" :")[0];
                    if (rowIdentifier == "Heirloom")
                    {
                        var box = _choiceBoxes[1];
                        _rawRows[i] = rowIdentifier + " :" + box.Choices[box.CurIndex];
                    }
                    else if (rowIdentifier == "Season Attunement")
                    {
                        _rawRows[i] = rowIdentifier + " : " + _seasonAttunement;
                    }
                    else
                        _rawRows[i] = rowIdentifier + " :" + _attributes[rowIdentifier];
                }
                return _rawRows;
            }
        }

        public int SaveIndex;

        public CharacterInit(Game1 game, ContentManager content, int saveIndex) : base(game, content)
        {
            SaveIndex = _game.SaveIndex = saveIndex;
            Name = "newgame";
            _penColour = Color.White;
        }

        public override void LoadContent()
        {
            var center = new Vector2(Game1.ScreenWidth / 2, Game1.ScreenHeight / 2);
            _font = _game.Textures.Font;
            var layer = 0.1f;
            var textureScale = 1.5f;
            var scale = textureScale * Game1.ResScale;

            LoadInputs(_font, layer, scale);

            _rawRows = new List<string>()
            {
                "Name : ",
                "Heirloom : ",
                " ",
                "HP : ",
                "MP : ",
                "Stamina : ",
                "Season Attunement : ",
                " ",
                "Strength : ",
                "Dexterity : ",
                "Ammo Handling : ",
                "Precision : ",
                "Arcane : ",
                "Focus : "
            };

            _attributes = new Dictionary<string, int>();
            var blacklist = new List<string>() { "Name : ", "Heirloom : ", " "};
            foreach (var str in _rawRows)
            {
                if (blacklist.Contains(str))
                    continue;

                _attributes.Add(str.Split(" :")[0], 0);
            }

            _seasonAttunement = _choiceBoxes[3].Choices[_choiceBoxes[3].CurIndex];
            
            _leftSidePositions = new List<Vector2>();
            var height = _font.MeasureString("S").Y;
            var origin = new Vector2((Game1.ScreenWidth / 20) * 11, (Game1.ScreenHeight / 3) - (height * _rawRows.Count) / 2);
            for (int i = 0; i < _rawRows.Count; i++)
                _leftSidePositions.Add(new Vector2(origin.X, origin.Y + (height * i)));
        }



        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _game.GraphicsDevice.Clear(Color.Black);

            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            foreach (var mcb in _choiceBoxes)
                mcb.Draw(gameTime, spriteBatch);


            for (int i = 0; i < Popups.Count; i++)
                Popups[i].Draw(gameTime, spriteBatch);

            var rows = _rows;
            for (int i = 0; i < rows.Count; i++)
                spriteBatch.DrawString(_font, rows[i], _leftSidePositions[i], _penColour);
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            foreach (var mcb in _choiceBoxes)
                mcb.Update(gameTime);

            for (int i = 0; i < Popups.Count; i++)
                Popups[i].Update(gameTime);

            SetDificultyColour();
            SetSeasonColour();

            //not very efficient but should do for now
            //TODO: only have the run whenever you change the class
            ChangeClass();
            _seasonAttunement = _choiceBoxes[3].Choices[_choiceBoxes[3].CurIndex];

        }

        public override void PostUpdate(GameTime gameTime)
        {

        }


        #region Clicked Methods

        private void Button_Start_Clicked(object sender, EventArgs e)
        {
            _game.SavesManager.Saves[SaveIndex].PlayerName = (_components[0] as TextInput).Text;


            foreach (var mcb in _choiceBoxes)
                mcb.OnApply?.Invoke(mcb, new EventArgs());

            _game.SavesManager.ActiveSave.Level = "levelzero";
            _game.ChangeState(new Level0(_game, _content, _game.Player));

            _game.SavesManager.UploadSave(SaveIndex);
        }
        private void Class_Apply(object sender, EventArgs e)
        {
            var attributes = new Dictionary<string, Models.Attribute>()
            {
                { "MoveSpeed", new Models.Attribute("MoveSpeed", 10) }
            };

            foreach (var att in _attributes)
                attributes.Add(att.Key, new Models.Attribute(att.Key, att.Value));

            _game.SavesManager.Saves[_game.RecentSave].Attributes = attributes;
        }
        private void Heirloom_Apply(object sender, EventArgs e)
        {
            var box = _choiceBoxes[1];
            var code = _game.ItemCodes[box.Choices[box.CurIndex]];
            _game.SavesManager.ActiveSave.Inventory.Add(code, _game.Items[code]);
        }
        private void Difficulty_Apply(object sender, EventArgs e)
        {
            
        }
        private void Season_Apply(object sender, EventArgs e)
        {
            
        }

        #endregion

        #region Private Methods

        private void SetDificultyColour()
        {
            switch (_difficulty.Choices[_difficulty.CurIndex])
            {
                case "Breeze":
                    _difficulty.ChoicePenColour = Color.Green; break;
                case "Gale":
                    _difficulty.ChoicePenColour = Color.Orange; break;
                case "Storm":
                    _difficulty.ChoicePenColour = Color.SteelBlue; break;
                case "Hurricane":
                    _difficulty.ChoicePenColour = Color.Crimson; break;
            };
        }

        private void SetSeasonColour()
        {
            var box = _choiceBoxes[3];
            switch (box.Choices[box.CurIndex])
            {
                case "Winter":
                    box.ChoicePenColour = Color.LightSteelBlue; break;
                case "Spring":
                    box.ChoicePenColour = Color.LawnGreen; break;
                case "Summer":
                    box.ChoicePenColour = Color.CornflowerBlue; break;
                case "Autumn":
                    box.ChoicePenColour = Color.Orange; break;
            };
        }

        private void ChangeAttrs(int hp, int mp, int stam, int str, int dex, int ammo, int prec, int arc, int skill)
        {
            _attributes["HP"] = hp;
            _attributes["MP"] = mp;
            _attributes["Stamina"] = stam;
            _attributes["Strength"] = str;
            _attributes["Dexterity"] = dex;
            _attributes["Ammo Handling"] = ammo;
            _attributes["Precision"] = prec;
            _attributes["Arcane"] = arc;
            _attributes["Focus"] = skill;
        }

        private void ChangeClass()
        {
            var role = _choiceBoxes[0].Choices[_choiceBoxes[0].CurIndex];
            switch (role)
            {
                case "Warrior":
                    ChangeAttrs(150, 50, 100, 12, 12, 7, 8, 5, 10);
                    break;
                case "Beserker":
                    ChangeAttrs(200, 25, 150, 15, 8, 5, 5, 5, 10);
                    break;
                case "Mage":
                    ChangeAttrs(100, 150, 75, 5, 7, 5, 5, 15, 10);
                    break;
                case "Vagabond":
                    ChangeAttrs(100, 75, 75, 8, 8, 8, 8, 8, 8);
                    break;
            }
        }

        private void LoadInputs(SpriteFont font, float layer, float scale)
        {
            var origin = new Vector2(Game1.ScreenWidth / 4, Game1.ScreenHeight / 8);

            _difficulty = new MultiChoiceBox(_game.Textures.Buttons["B&W"], _game.Textures.ArrowLeft, font, 0)
            {
                Text = "Difficulty",
                Choices = new List<string>()
                {
                    "Breeze",
                    "Gale",
                    "Storm",
                    "Hurricane",
                },
                Layer = layer,
                OnApply = new EventHandler(Difficulty_Apply),
                Type = "CharInit",
            };

            _components = new List<Component>()
            {
                new TextInput(font, "Name ")
                {
                    PenColour = Color.White,
                    BackColour = Color.Black,
                    BorderColour = Color.White,
                },
                new Button(_game.Textures.Buttons["B&W"], font)
                {
                    Text = "Start",
                    Click = new EventHandler(Button_Start_Clicked),
                    Layer = layer,
                    TextureScale = 0.8f,
                    Position = new Vector2(Game1.ScreenWidth / 2 - (_game.Textures.Buttons["B&W"].Width * scale) / 2, origin.Y * 6),
                    PenColour = Color.White,
                },
            };

            _choiceBoxes = new List<MultiChoiceBox>()
            {
                new MultiChoiceBox(_game.Textures.Buttons["B&W"], _game.Textures.ArrowLeft, font, 0)
                {
                    Text = "Class",
                    Choices = new List<string>()
                    {
                        "Warrior",
                        "Beserker",
                        "Mage",
                        "Vagabond"
                    },
                    Layer = layer,
                    OnApply = new EventHandler(Class_Apply),
                    Type = "CharInit",
                },
                new MultiChoiceBox(_game.Textures.Buttons["B&W"], _game.Textures.ArrowLeft, font, 0)
                {
                    Text = "Heirloom",
                    Choices = new List<string>()
                    {
                        "Crimson Blessing",
                        "Silver Powder",
                        "Lesser Guinn",
                        "Fast Feet",
                    },
                    Layer = layer,
                    OnApply = new EventHandler(Heirloom_Apply),
                    Type = "CharInit",
                },
                _difficulty,
                new MultiChoiceBox(_game.Textures.Buttons["B&W"], _game.Textures.ArrowLeft, font, 0)
                {
                    Text = "Season Attunement",
                    Choices = new List<string>()
                    {
                        "Winter",
                        "Spring",
                        "Summer",
                        "Autumn",
                    },
                    Layer = layer,
                    OnApply = new EventHandler(Season_Apply),
                    Type = "CharInit",
                },
            };

            (_components[0] as TextInput).LoadContent(_game, origin);
            for (int i = 0; i < _choiceBoxes.Count; i++)
                _choiceBoxes[i].LoadContent(_game, new Vector2(origin.X, origin.Y + ((25f * Game1.ResScale) * (i + 2))));
        }


        #endregion
    }
}
