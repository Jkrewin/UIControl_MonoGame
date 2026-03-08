using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace UIControl_MonoGame.UIControl
{
    public class TextBoxUI : Cordinator, IControlUI
    {
        private bool ShowCursor;
        private readonly Stopwatch CursorTimer = new();
        private KeyboardState _previousKeyboardState;
        private int CursorPosition;
        private bool _focus = false;

        public Vector2 Location { get => new(RectObjectUI.X, RectObjectUI.Y); set => RectObjectUI = new Rectangle((int)value.X, (int)value.Y, RectObjectUI.Width, RectObjectUI.Height); }
        public bool Visible { get; set; }
        public bool Focused
        {
            get => _focus; set
            {
                if (value) CursorTimer.Start();
                else CursorTimer.Stop();
                _focus = value;
            }
        }
        public string Name { get; set; }
        public int MaxLength { get; set; }
        public int Height { get => RectObjectUI.Height; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, RectObjectUI.Width, value); }
        public int Width { get => RectObjectUI.Width; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, value, RectObjectUI.Height); }

        public delegate void ChangeText(string str);
        /// <summary>
        /// The text has been changed
        /// </summary>
        public event ChangeText UpdateText;
        public delegate void SetFocuse();
        /// <summary>
        /// Control in focus for text input
        /// </summary>
        public event SetFocuse OnFocus;

        /// <summary>
        /// Text 
        /// </summary>
        public UIText Caption { get; set; }
        /// <summary>
        /// Texture or animation is a standard texture
        /// </summary>
        public UITexture MainTexture { get; set; }

        public TextBoxUI(Game game, string nameUI, Rectangle recPoss, string fontContent, string text)
        {
            if (string.IsNullOrEmpty(nameUI)) throw new ArgumentNullException(nameof(nameUI));

            _game = game;
            Name = nameUI;
            RectObjectUI = recPoss;
            Caption = new UIText(_game, text, fontContent);
            Caption.Position = UIText.TextPositionEnum.Left;            
            MaxLength = (int)(recPoss.Width / (Caption.Font.MeasureString(" ").X +7));
            MainTexture = new UITexture(Rect2D(recPoss));
            CursorPosition = text.Length;
            Visible = true;
        }

        public void ControlEvent(MouseState getMouse, KeyboardState getKey, uint getJoy = 1)
        {
            if (Visible == false) return;


            bool isHovered = getMouse.X >= RectObjectUI.X && getMouse.X <= RectObjectUI.X + RectObjectUI.Width &&
                             getMouse.Y >= RectObjectUI.Y && getMouse.Y <= RectObjectUI.Y + RectObjectUI.Height;

            if (getMouse.LeftButton == ButtonState.Pressed & isHovered & Cliker == false)
            {
                Cliker = true;
                Focused = true;
                OnFocus?.Invoke();
            }


            if (_focus)
            {
                if (CursorTimer.ElapsedMilliseconds >= 100)
                {
                    CursorTimer.Restart();
                    ShowCursor = !ShowCursor;
                }

                if (getKey.IsKeyDown(Keys.Back) &&
                    _previousKeyboardState.IsKeyUp(Keys.Back))
                {
                    if (CursorPosition > 0)
                    {
                        Caption.Text = Caption.Text.Remove(CursorPosition - 1, 1);
                        CursorPosition--;
                    }
                }

                if (getKey.IsKeyDown(Keys.Delete) &&
                    _previousKeyboardState.IsKeyUp(Keys.Delete))
                {
                    if (CursorPosition < Caption.Text.Length)
                    {
                        Caption.Text = Caption.Text.Remove(CursorPosition, 1);
                    }
                }

                if (getKey.IsKeyDown(Keys.Left) &&
                    _previousKeyboardState.IsKeyUp(Keys.Left))
                {
                    if (CursorPosition > 0) CursorPosition--;
                }

                if (getKey.IsKeyDown(Keys.Right) &&
                    _previousKeyboardState.IsKeyUp(Keys.Right))
                {
                    if (CursorPosition < Caption.Text.Length) CursorPosition++;
                }

                if (getKey.IsKeyDown(Keys.Home) &&
                    _previousKeyboardState.IsKeyUp(Keys.Home))
                {
                    CursorPosition = 0;
                }

                if (getKey.IsKeyDown(Keys.End) &&
                    _previousKeyboardState.IsKeyUp(Keys.End))
                {
                    CursorPosition = Caption.Text.Length;
                }

                foreach (Keys key in getKey.GetPressedKeys())
                {
                    if (_previousKeyboardState.IsKeyUp(key))
                    {
                        if (key == Keys.Enter)
                        {
                            Focused = false;
                            break;
                        }

                        if (key == Keys.Space && Caption.Text.Length < MaxLength)
                        {
                            Caption.Text = Caption.Text.Insert(CursorPosition, " ");
                            CursorPosition++;
                        }
                        else if (key >= Keys.A && key <= Keys.Z)
                        {
                            if (Caption.Text.Length < MaxLength)
                            {
                                bool shift = getKey.IsKeyDown(Keys.LeftShift) ||
                                            getKey.IsKeyDown(Keys.RightShift);
                                char c = shift ? (char)('A' + (key - Keys.A)) : (char)('a' + (key - Keys.A));
                                Caption.Text = Caption.Text.Insert(CursorPosition, c.ToString());
                                CursorPosition++;
                            }
                        }
                        else if (key >= Keys.D0 && key <= Keys.D9)
                        {
                            if (Caption.Text.Length < MaxLength)
                            {
                                bool shift = getKey.IsKeyDown(Keys.LeftShift) ||
                                            getKey.IsKeyDown(Keys.RightShift);
                                char c = shift ? ")"[key - Keys.D0] : (char)('0' + (key - Keys.D0));
                                Caption.Text = Caption.Text.Insert(CursorPosition, c.ToString());
                                CursorPosition++;
                            }
                        }
                    }
                }

                UpdateText?.Invoke(Caption.Text);
            }
            else
            {
                ShowCursor = false;
            }

            _previousKeyboardState = getKey;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible == false) return;
            MainTexture?.Display(spriteBatch, gameTime, RectObjectUI);
            Caption.Display(spriteBatch, RectObjectUI);

            if (_focus && ShowCursor)
            {
                var local = Caption.GetPosition(RectObjectUI);
                string textBeforeCursor = Caption.Text[..CursorPosition];
                spriteBatch.DrawString(Caption.Font, "|", new Vector2(local.X + Caption.Font.MeasureString(textBeforeCursor).X, local.Y),
                                                 Caption.ColorText, Caption.Rotation, Caption.Origin, Caption.Scale, Caption.Effects, 0.5f);
            }
        }




    }
}
