using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bound.Models
{
    public class Input
    {
        //I use a dict so that I can easily iterate through and get the name of the input and the assigned value
        //This is for the KeyInput control during the Settings state
        public Dictionary<string, string> Keys;
        public KeyboardState PreviousKeyboardState;
        public KeyboardState CurrentKeyboardState;
        public MouseState PreviousMouseState;
        public MouseState CurrentMouseState;

        public Rectangle MouseRectangle
        {
            get
            {
                return new Rectangle(CurrentMouseState.X, CurrentMouseState.Y, 1, 1);
            }
        }

        public bool IsLeftClick
        {
            get
            {
                return (CurrentMouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released);
            }
        }

        public bool LeftClickReleased
        {
            get
            {
                return (CurrentMouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed);
            }
        }

        public Input(Dictionary<string, string> keys, Game1 game)
        {
            Keys = keys;

            foreach (var kvp in game.Settings.DefaultInputValues)
            {
                if (!keys.ContainsKey(kvp.Key))
                    Keys.Add(kvp.Key, kvp.Value);
            }
        }

        public bool IsPressed(string key, bool IsHoldable)
        {
            if (!Keys.ContainsKey(key))
                return false;
            key = Keys[key];
            //If it is a mouse button
            //TODO: Refactor this rats' nest, i don't even know if it works
            if (key.Substring(0, 1) == "M" && key.Length == 2)
            {
                bool pButtonState;
                bool cButtonState;

                switch (key.Substring(1, 1))
                {
                    case "1":
                        pButtonState = PreviousMouseState.LeftButton == ButtonState.Pressed;
                        cButtonState = CurrentMouseState.LeftButton == ButtonState.Pressed;
                        break;
                    case "2":
                        pButtonState = PreviousMouseState.RightButton == ButtonState.Pressed;
                        cButtonState = CurrentMouseState.RightButton == ButtonState.Pressed;
                        break;
                    case "3":
                        pButtonState = PreviousMouseState.MiddleButton == ButtonState.Pressed;
                        cButtonState = CurrentMouseState.MiddleButton == ButtonState.Pressed;
                        break;
                    case "4":
                        pButtonState = PreviousMouseState.XButton1 == ButtonState.Pressed;
                        cButtonState = CurrentMouseState.XButton1 == ButtonState.Pressed;
                        break;
                    case "5":
                        pButtonState = PreviousMouseState.XButton2 == ButtonState.Pressed;
                        cButtonState = CurrentMouseState.XButton2 == ButtonState.Pressed;
                        break;
                    default:
                        pButtonState = false;
                        cButtonState = false;
                        break;
                }

                if (IsHoldable && cButtonState)
                    return true;
                else if (pButtonState && cButtonState)
                    return true;
                else return false;
            }

            else
            {
                var inputMode = GetKey(key);

                if (IsHoldable && CurrentKeyboardState.IsKeyDown(inputMode))
                    return true;
                else if (PreviousKeyboardState.IsKeyUp(inputMode) && CurrentKeyboardState.IsKeyDown(inputMode))
                    return true;
                else return false;
                
            }
        }

        public void Update()
        {
            PreviousMouseState = CurrentMouseState;
            CurrentMouseState = Mouse.GetState();

            PreviousKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }

        public Keys GetKey(string key)
        {
            Keys inputMode;
            if (KeysFromSpecialKey.Keys.Contains(key))
                inputMode = (Keys)Enum.Parse(typeof(Keys), KeysFromSpecialKey[key], true);
            else
                inputMode = (Keys)((int)(char.ToUpper(key.ToCharArray()[0])));

            return inputMode;
        }

        #region Statics
        //I hate all keyboards now
        public static Dictionary<string, string> SpecialKeyMap = new Dictionary<string, string>
        {
            { "CapsLock", "CAPS"},
            { "Space", "SPACE"},
            { "Decimal", "Dcml"},
            { "Delete", "DEL"},
            { "Divide", "DIV"},
            { "Execure", "EXE"},
            { "Inser", "INS" },
            { "Kanji", "Knji"},
            { "LeftALt", "LALT" },
            { "LeftControl", "LCTRL"},
            { "LeftShift", "LSHFT"},
            { "OemBlackslash", "\\" },
            { "OemCloseBrackets", "]" },
            { "OemComma", "," },
            { "OemClear", "CLR"},
            { "OemCopy", "CPY"},
            { "OemMinus", "-" },
            { "OemOpenBrackets", "[" },
            { "OemPeriod", "." },
            { "OemPipe", "|" },
            { "OemPlus", "+" },
            { "OemQuestion", "?" },
            { "OemQuotes", "\"" },
            { "OemSemicolon", ";" },
            { "OemTilde", "~" },
            { "PageDown", "PDWN" },
            { "PageUp", "PUP" },
            { "PrintScreen", "PSCRN" },
            { "RightAlt", "RALT" },
            { "RightControl", "RCTRL" },
            { "RightShift", "RSHFT" },
            { "Multiply", "MLTPLY" },
            { "Tab", "TAB" },
            { "Separator", "SEP"}, //idk what this is. from stack overflow, apparently used on some brazilian/far east
            { "D1", "1"},
            { "D2", "2"},
            { "D3", "3"},
            { "D4", "4"},
            { "D5", "5"},
            { "D6", "6"},
            { "D7", "7"},
            { "D8", "8"},
            { "D9", "9"},
            { "D0", "0"},
        };

        //im sure there is a better way to do this
        public static Dictionary<string, string> KeysFromSpecialKey = new Dictionary<string, string>();
        #endregion
    }
}
