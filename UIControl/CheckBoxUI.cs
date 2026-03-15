using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace UIControl_MonoGame.UIControl
{
    public class CheckBoxUI : Cordinator, IControlUI,IToXml
    {
        public Vector2 Location { get => new(RectObjectUI.X, RectObjectUI.Y); set => RectObjectUI = new Rectangle((int)value.X, (int)value.Y, RectObjectUI.Width, RectObjectUI.Height); }
        public bool Visible { get; set; }
        public bool Focused { get; set; }
        public string Name { get; set; }
        public int Height { get => RectObjectUI.Height; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, RectObjectUI.Width, value); }
        public int Width { get => RectObjectUI.Width; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, value, RectObjectUI.Height); }
        public Anchor AnchorLocation { get; set; }

        public delegate void Click();
        /// <summary>
        /// Event when the mouse clicks
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
        /// Value checked or not checked
        /// </summary>
        public bool IsChecked { get; set; }
        /// <summary>
        /// Texture when not checked control
        /// </summary>
        public UITexture UnCheckTexture { get; set; }
        /// <summary>
        /// Texture when checked 
        /// </summary>
        public UITexture CheckTexture { get; set; }

        /// <summary>
        /// The current state of the texture
        /// </summary>
        private UITexture MainTexture
        {
            get
            {
                if (IsChecked) return CheckTexture;
                else return UnCheckTexture;
            }
        }

       

        public CheckBoxUI(Game game, string nameUI, Rectangle possRec, UITexture checkStatus, UITexture uncheckStatus) {
            if (string.IsNullOrEmpty(nameUI)) throw new ArgumentNullException(nameof(nameUI));

            _game = game;
            Name = nameUI;
            RectObjectUI = possRec;
            CheckTexture = checkStatus;
            UnCheckTexture = uncheckStatus;
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
                IsChecked = !IsChecked;
                OnSetFocuse?.Invoke();
                OnClick?.Invoke();
            }
            else if (getKey.IsKeyDown(Keys.Space) & Focused) {
                IsChecked = !IsChecked;
                OnClick?.Invoke();
            }
            else if (getMouse.LeftButton == ButtonState.Released & isHovered == false)
            {
                OnMouseLeave?.Invoke();
            }
            else if (isHovered)
            {
                OnMouseEnter?.Invoke();
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible == false) return;

            MainTexture.Display(spriteBatch, gameTime, RectObjectUI);
            Caption?.Display(spriteBatch,RectObjectUI);
        }

        public string ToXml() => INDENT + IToXml.ConvertXml(this)[..^2] + "\n" + INDENT + "</" + this.GetType().Name + ">";
    }
}
