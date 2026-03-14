using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace UIControl_MonoGame.UIControl
{
    public class LabelUI : Cordinator, IControlUI, IToXml
    {
        public Vector2 Location { get => new(RectObjectUI.X, RectObjectUI.Y); set => RectObjectUI = new Rectangle((int)value.X, (int)value.Y, RectObjectUI.Width, RectObjectUI.Height); }
        public bool Visible { get ; set ; }
        public bool Focused { get { return false; } set { } }
        public string Name { get; set; }
        public int Height { get => RectObjectUI.Height; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, RectObjectUI.Width, value); }
        public int Width { get => RectObjectUI.Width; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, value, RectObjectUI.Height); }
        public Anchor AnchorLocation { get; set; }

        public delegate void MouseEnter();
        /// <summary>
        /// Occurs when the mouse is in the control UI
        /// </summary>
        public event MouseEnter OnMouseEnter;
        public delegate void MouseLeave();
        /// <summary>
        /// Occurs when the mouse leaves the control UI
        /// </summary>
        public event MouseLeave OnMouseLeave;

        /// <summary>
        /// Background texture
        /// </summary>
        public UITexture Background { get ; set ; }
        /// <summary>
        /// Text label
        /// </summary>
        public UIText Caption { get; set; }

        public LabelUI(Game game, string nameUI, Rectangle recPoss, string labelText, string fontContent) {
            if (string.IsNullOrEmpty(nameUI)) throw new ArgumentNullException(nameof(nameUI));

            _game = game;
            Name = nameUI;
            RectObjectUI= recPoss;
            Caption = new UIText(_game, labelText, fontContent);
            Caption.Position = UIText.TextPositionEnum.Left;
            Visible = true;
        }

        public void ControlEvent(MouseState getMouse, KeyboardState getKey, uint getJoy = 1)
        {
            if (Visible == false) return;
            bool isHovered = getMouse.X >= RectObjectUI.X && getMouse.X <= RectObjectUI.X + RectObjectUI.Width &&
            getMouse.Y >= RectObjectUI.Y && getMouse.Y <= RectObjectUI.Y + RectObjectUI.Height;

             if (getMouse.LeftButton == ButtonState.Released & isHovered == false)
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

            Background?.Display(spriteBatch, gameTime, RectObjectUI);
            Caption.Display(spriteBatch, RectObjectUI);
        }

        public string ToXml() => INDENT +  IToXml.ConvertXml(this)[..^2] + "\n" + INDENT + "</"+  this.GetType ().Name + ">";
    }
}
