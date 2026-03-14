using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace UIControl_MonoGame.UIControl
{
    public class ButtonUI : Cordinator, IControlUI
    {
        private string _name;
        private UITexture _nowTextur;
        private UITexture _textureButton;
        private UITexture _selectedTextureButton;

        public Vector2 Location { get => new(RectObjectUI.X, RectObjectUI.Y); set => RectObjectUI = new Rectangle((int)value.X, (int)value.Y, RectObjectUI.Width, RectObjectUI.Height); }
        public bool Visible { get; set; } = false;
        public bool Focused { get; set; }
        public string Name { get => _name; set => _name = value; }
        public int Height { get => RectObjectUI.Height; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, RectObjectUI.Width, value); }
        public int Width { get => RectObjectUI.Width; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, value, RectObjectUI.Height); }
        public Anchor AnchorLocation { get; set; }

        public delegate void Click();
        /// <summary>
        /// Event when the mouse clicks on the button
        /// </summary>
        public event Click OnClick;
        public delegate void MouseEnter();
        /// <summary>
        /// Event at the moment when the mouse entered the control 
        /// </summary>
        public event MouseEnter OnMouseEnter;
        public delegate void MouseLeave();
        /// <summary>
        /// Event when the mouse left the control 
        /// </summary>
        public event MouseLeave OnMouseLeave;
        public delegate void SetFocuse();
        /// <summary>
        /// Control in focus
        /// </summary>
        public event SetFocuse OnSetFocuse;
        /// <summary>
        /// Text on the object
        /// </summary>
        public UIText Caption { get; set; }
        /// <summary>
        /// Texture or animation is a standard texture
        /// </summary>
        public UITexture TextureButton { get => _textureButton; set { _nowTextur = value; _textureButton = value; } }
        /// <summary>
        /// Texture or animation when highlighted
        /// </summary>
        public UITexture SelectedTextureButton { get => _selectedTextureButton; set => _selectedTextureButton = value; }

        public ButtonUI(Game game, string nameUI, Rectangle possRec)
        {
            if (string.IsNullOrEmpty(nameUI)) throw new ArgumentNullException(nameof(nameUI));

            _game = game;
            RectObjectUI = possRec;
            _name = nameUI;
            TextureButton = new UITexture(Rect2D(possRec));
            SelectedTextureButton = new UITexture(Rect2D(possRec)) { ColorFrame = Color.LightBlue };
            Visible = true;
        }
        public ButtonUI(Game game, string nameUI, Rectangle possRec, string fontName, string text)
        {
            if (string.IsNullOrEmpty(nameUI)) throw new ArgumentNullException(nameof(nameUI));

            _game = game;
            RectObjectUI = possRec;
            _name = nameUI;
            TextureButton = new UITexture(Rect2D(possRec));
            SelectedTextureButton = new UITexture(Rect2D(possRec)) { ColorFrame = Color.LightBlue };
            Caption = new UIText(_game, text, fontName);
            Visible = true;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible == false) return;

            _nowTextur.Display(spriteBatch, gameTime, RectObjectUI);

            Caption?.Display(spriteBatch, RectObjectUI);

        }
        public void ControlEvent(MouseState getMouse, KeyboardState getKey, uint getJoy = 1)
        {
            if (!Visible) return;

            bool isHovered = getMouse.X >= RectObjectUI.X && getMouse.X <= RectObjectUI.X + RectObjectUI.Width &&
             getMouse.Y >= RectObjectUI.Y && getMouse.Y <= RectObjectUI.Y + RectObjectUI.Height;

            if (getMouse.LeftButton == ButtonState.Pressed & isHovered & Cliker == false)
            {
                Cliker = true;
                Focused = true;
                OnSetFocuse?.Invoke();
                OnClick?.Invoke();
            }
            else if (getMouse.LeftButton == ButtonState.Released & isHovered == false)
            {
                OnMouseLeave?.Invoke();
                _nowTextur = TextureButton;
            }
            else if (isHovered)
            {
                OnMouseEnter?.Invoke();
                _nowTextur = SelectedTextureButton;
            }
        }


    }
}
