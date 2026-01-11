using Bound.Controls;
using Bound.Controls.Settings;
using Bound.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.States.Popups.Game
{
    public class Stats : Popup
    {
        private List<Component> _components;
        private List<MultiChoiceBox> _attributes = new List<MultiChoiceBox>();
        private List<(string Text, Vector2 Position)> _playerStats = new List<(string Text, Vector2 Position)>();
        private List<int> _attrIndexes = new List<int>();
        private int _stringLength;
        private int _exp;
        private Vector2 _expPosition;
        private Button _apply;
        private bool _previousShowApply;
        private Vector2[] _backPositions;

        public float Layer;
        public Color PenColor = Game1.MenuColorPalette[2];

        private bool _showApply
        {
            get { return _exp != _game.ActiveSave.EXP; }
        }
        
        public Stats(Game1 game, ContentManager content, State parent) : base(game, content, parent)
        {
            Name = Game1.Names.StatsWindow;
            Layer = 0.79f;
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var component in _components)
                component.Draw(gameTime, spriteBatch);

            if (_showApply)
                _apply.Draw(gameTime, spriteBatch);

            foreach (var mcb in _attributes)
                mcb.Draw(gameTime, spriteBatch);

            foreach (var stat in _playerStats)
                spriteBatch.DrawString(_game.Textures.Font, stat.Text, stat.Position, PenColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);
            spriteBatch.DrawString(_game.Textures.Font, _exp.ToString(), _expPosition, (_exp < _game.ActiveSave.EXP ? Color.DarkRed : PenColor), 0f, Vector2.Zero, 1f, SpriteEffects.None, Layer + 0.001f);
        }

        public override void LoadContent()
        {
            var eigthWidth = Game1.ScreenWidth / 8;
            var eigthHeight = Game1.ScreenHeight / 8;
            var statsBoxPos = new Vector2(eigthWidth + Game1.V2Transform.X, eigthHeight + Game1.V2Transform.Y);

            var statsBackground = new BorderedBox
            (
                _game.Textures.BaseBackground,
                _game.GraphicsDevice,
                Game1.MenuColorPalette[0],
                statsBoxPos,
                Layer,
                (int)(eigthWidth * 2.5),
                eigthHeight  * 6
            );

            var border = 5 * Game1.ResScale;
            var textStartingPosition = new Vector2(statsBoxPos.X + border, statsBoxPos.Y + border);
            _stringLength = (int)(statsBackground.Width / (_game.Textures.Font.MeasureString("X").X * 1.05));
            var fullWidth = (int)(statsBackground.Width - (Game1.ResScale   ) - (textStartingPosition.X - statsBackground.Position.X));
            _playerStats.Add(($"Name{_game.ActiveSave.PlayerName.PadLeft(_stringLength - 4)}", new Vector2(textStartingPosition.X + 3 * Game1.ResScale, textStartingPosition.Y) ));
            _exp = _game.ActiveSave.EXP;

            var i = 2;
            var spacing = _game.Textures.Font.MeasureString("t").Y + 1 * Game1.ResScale;
            for (int j = 0; j < 2; j++)
            {
                var attribute = _game.Player.Attributes.Dictionary.ToArray()[j];
                _playerStats.Add(
                    ($"{attribute.Key}{attribute.Value.ToString().PadLeft(_stringLength - attribute.Key.Length)}",
                    new Vector2(textStartingPosition.X + 3 * Game1.ResScale, textStartingPosition.Y + spacing * i)));
                i++;
            }

            i++;
            _playerStats.Add(("Exp", new Vector2(textStartingPosition.X + 3 * Game1.ResScale, textStartingPosition.Y + spacing * i)));
            _expPosition = _playerStats[^1].Position + new Vector2(_game.Textures.Font.MeasureString("X").X * ($"EXP{_exp.ToString().PadLeft(_stringLength - "EXP".Length)}".Length - 2.75f), 0);
            i += 2;

            for (int j = 2; j < _game.ActiveSave.Attributes.Count; j++)
            {
                var attribute = _game.Player.Attributes.Dictionary.ToArray()[j];
                _attributes.Add(new MultiChoiceBox
                    (
                        _game.Textures.BaseBackground,
                        _game.Textures.ArrowLeft,
                        _game.Textures.Font,
                        0,
                        false,
                        true,
                        Color.ForestGreen,
                        fullWidth
                    )
                {
                    Text = attribute.Key,
                    Choices = Enumerable.Range((int)attribute.Value, 1 + MaxIncrements((int)attribute.Value, _exp)).Select(x => x.ToString()).ToList(),
                    Layer = Layer + 0.01f,
                    Type = "Attribute",
                    PenColour = Color.White,
                }
                );

                _attributes[^1].LoadContent(_game, new Vector2(textStartingPosition.X, textStartingPosition.Y + spacing * i));
                _attributes[^1].BackgroundColour = statsBackground.Colour;
                _attrIndexes.Add(0);
                i++;
            }

            var gap = 5 * Game1.ResScale;
            var textureScale = 0.5f;
            var scale = textureScale * Game1.ResScale;
            _backPositions = new Vector2[] {
                new Vector2((statsBackground.Width - _game.Textures.Button.Width * scale) / 2, (statsBackground.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale),
                new Vector2((statsBackground.Width - gap) / 2f - (_game.Textures.Button.Width * scale), (statsBackground.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale)
            };

            _components = new List<Component>() 
            { 
                statsBackground,
                new Button(_game.Textures.Button, _game.Textures.Font)
                {
                    Text = "Back",
                    Click = new EventHandler(Button_Back_Clicked),
                    Layer = Layer + 0.001f,
                    TextureScale = textureScale,
                    RelativePosition = _backPositions[0],
                    ToCenter = true,
                    ButtonColour = Color.White,
                    Parent = statsBackground
                }
            };

            _apply = new Button(_game.Textures.Button, _game.Textures.Font)
            {
                Text = "Apply",
                Click = new EventHandler(Button_Apply_Clicked),
                Layer = Layer + 0.001f,
                TextureScale = textureScale,
                RelativePosition = new Vector2((statsBackground.Width + gap) / 2f, (statsBackground.Height - _game.Textures.Button.Height * Game1.ResScale * 0.75f) - 5 * Game1.ResScale),
                ToCenter = true,
                ButtonColour = Color.White,
                Parent = statsBackground
            };
            
        }

        public override void Update(GameTime gameTime)
        {
            foreach (var component in _components)
                component.Update(gameTime);

            if (_showApply)
                _apply.Update(gameTime);

            foreach (var mcb in _attributes)
                mcb.Update(gameTime);

            if (_game.PlayerKeys.CurrentKeyboardState.IsKeyUp(Keys.Escape) 
                && _game.PlayerKeys.PreviousKeyboardState.IsKeyDown(Keys.Escape))
                Button_Back_Clicked(new object(), new EventArgs());

            for (int i = 0; i < _attrIndexes.Count; i++)
            {
                if (_attributes[i].CurIndex != _attrIndexes[i])
                {
                    _exp += (_attributes[i].CurIndex < _attrIndexes[i]) ?  (AttributeIncrementCost(_attributes[i].CurIndex)) : -1 * (AttributeIncrementCost(_attrIndexes[i]));

                    for (int j = 0; j < _attrIndexes.Count; j++)
                    {
                        if (j == i) continue;
                        _attributes[j].Choices = Enumerable.Range(int.Parse(_attributes[j].Choices[0]), 1 + MaxIncrements(int.Parse(_attributes[j].Choices[0]), _exp)).Select(x => x.ToString()).ToList(); 
                    }
                    
                    _attrIndexes[i] = _attributes[i].CurIndex;
                    break;
                }
            }

            if (_previousShowApply != _showApply)
            {
                _previousShowApply = _showApply;
                (_components[1] as Button).RelativePosition = _showApply ? _backPositions[1] : _backPositions[0];
            }


        }

        private void Button_Back_Clicked(object v, EventArgs eventArgs)
        {
            Parent.Popups.Remove(this);
        }

        private void Button_Apply_Clicked(object v, EventArgs eventArgs)
        {
            _game.ActiveSave.EXP = _exp;
            for (int i = 0; i < _attributes.Count; i++)
            {
                var attr = _attributes[i];
                if (attr.CurIndex != 0)
                {
                    attr.Choices = attr.Choices.Skip(attr.Choices.IndexOf(attr.CurrentChoice)).ToList();
                    attr.CurIndex = 0;
                    _attrIndexes[i] = 0;
                }
            }
        }

        private int AttributeIncrementCost(int currentLevel)
        {
            var level = currentLevel + 1;

            if (level < 10)
                return 3 * level;
            else if (level < 20)
                return 30 + (5 * level); //cost of last level + new scaling
            else if (level < 40)
                return 80 + (8 * level);
            else if (level < 99)
                return 240 + (12 * level);
            else
                return int.MaxValue; //Maximum Attribute level is 99
        }

        private int MaxIncrements(int level, int exp)
        { 
            int end = level;
            while (true)
            {
                exp -= AttributeIncrementCost(end);
                if (exp > 0)
                    end++;
                else break;
            }

            return end - level;
        }

    }
}