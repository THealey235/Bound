using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Bound.Models
{
    public class Input
    {
        //I use a dict so that I can easily iterate through and get the name of the input and the assigned value
        //This is for the KeyInput control during the Settings state
        public Dictionary<string, string> Keys;

        public Input(Dictionary<string, string> keys)
        {
            Keys = keys;
        }

        //c = current, p = previous
        public static bool IsPressed(string key, KeyboardState pkState, KeyboardState ckState, MouseState pmState, MouseState cmState, bool IsHoldable)
        {
            //If it is a keyboard key
            if (key.Length == 1)
            {
                var inputMode = (Keys)((int)(char.ToUpper(key.ToCharArray()[0])));

                if (IsHoldable && ckState.IsKeyDown(inputMode))
                    return true;
                else if (pkState.IsKeyUp(inputMode) && ckState.IsKeyDown(inputMode))
                    return true;
                else return false;
            }

            //If it is a mouse button
            else
            {
                bool pButtonState;
                bool cButtonState;

                
                switch (key.Substring(1, 1))
                {
                    case "1":
                        pButtonState = pmState.LeftButton == ButtonState.Pressed;
                        cButtonState = pmState.LeftButton == ButtonState.Pressed;
                        break;
                    case "2":
                        pButtonState = pmState.RightButton == ButtonState.Pressed;
                        cButtonState = pmState.RightButton == ButtonState.Pressed;
                        break;
                    case "3":
                        pButtonState = pmState.MiddleButton == ButtonState.Pressed;
                        cButtonState = pmState.MiddleButton == ButtonState.Pressed;
                        break;
                    case "4":
                        pButtonState = pmState.XButton1 == ButtonState.Pressed;
                        cButtonState = pmState.XButton1 == ButtonState.Pressed;
                        break;
                    case "5":
                        pButtonState = pmState.XButton2 == ButtonState.Pressed;
                        cButtonState = pmState.XButton2 == ButtonState.Pressed;
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
            
        }
    }
}
