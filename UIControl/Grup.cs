using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Linq;

namespace UIControl_MonoGame.UIControl
{
    public class Grup : Cordinator, IControlUI, IToXml
    {
        private readonly List<IControlUI> Controls  = [];   //Controls in this group

        /// <summary>
        /// Highlights a control in a red frame to make it easier to create a design.
        /// </summary>
        public bool ShowRedLine { get; set; } = false;      
        public Vector2 Location
        {
            get => new(RectObjectUI.X, RectObjectUI.Y); set
            {
                foreach (var control in Controls)
                {
                    Vector2 v = control.Location;
                    control.Location = new Vector2(value.X + (v.X - RectObjectUI.X), value.Y + (v.Y - RectObjectUI.Y));
                }
                RectObjectUI = new Rectangle((int)value.X, (int)value.Y, RectObjectUI.Width, RectObjectUI.Height);

                foreach (var item in Controls) {
                    if (item.AnchorLocation is not null) {
                        item.AnchorLocation.Resize = false;
                    }
                }                
            }
        }
        public bool Visible { get; set; } = true;
        public bool Focused { get; set; }
        public string Name { get; set; }
        public int Height { get => RectObjectUI.Height ; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y , RectObjectUI.Width, value); }
        public int Width { get => RectObjectUI.Width; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, value, RectObjectUI.Height); }
        public Anchor AnchorLocation { get; set; }

        public Grup(Game game, Rectangle recPoss, string name)
        {
            RectObjectUI = recPoss;
            Name = name;
            _game = game;
        }

        public T FindControl<T>(string name) where T : IControlUI
        {
            foreach (var control in Controls)
            {
                if (control.Name == name.Trim() & control is T)
                {
                    return (T)control;
                }
            }
            throw new System.Exception("The control with the name " + name + " [" + typeof(T).Name + "] was not found ");
        }

        public void Add(IControlUI control) {
            if (string.IsNullOrEmpty(control.Name)) throw new System.Exception("The name of the control is not specified");
            if (Controls.Any(x => x.Name == control.Name)) throw new System.Exception("A control with that name already exists in the group.");

            
            Controls.Add(control);
        }

        public void Remove(string name)
        {
            foreach (var control in Controls)
            {
                if (control.Name == name.Trim())
                {
                    Remove(control);
                }
            }
        }
        public void Remove(IControlUI control) => Controls.Remove(control);

        public string[] ListNameControls() {
            List<string> ls = [];

            foreach (var item in Controls)
            {
                ls.Add(item.Name + " " + item .GetType().Name);
            }
            return [.. ls];
        }

        public void ControlEvent(MouseState getMouse, KeyboardState getKey, uint getJoy = 1)
        {
            if (Visible)
            {
                foreach (var control in Controls) control.ControlEvent(getMouse, getKey, getJoy);
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Visible)
            {

                foreach (var item in Controls) ResizeAnchor(item);

                foreach (var control in Controls) control.Draw(gameTime, spriteBatch);
                if (ShowRedLine)
                {
                    
                    Color color = Color.White;
                    int thickness = 5;
                    spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X, RectObjectUI.Y, RectObjectUI.Width, thickness), color);
                    spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X, RectObjectUI.Y + RectObjectUI.Height - thickness, RectObjectUI.Width, thickness), color);
                    spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X, RectObjectUI.Y, thickness, RectObjectUI.Height), color);
                    spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X + RectObjectUI.Width - thickness, RectObjectUI.Y, thickness, RectObjectUI.Height), color);
                }
            }
        }

        public string ToXml() {
            string deep="";
            foreach (var item in Controls)
            {
                if (item is IToXml xml) deep += "\n" + xml.ToXml();
            }
            return deep;
        }

        private void ResizeAnchor(IControlUI control ) {
            if (control.AnchorLocation is null) return;
            if (control.AnchorLocation.Resize) return;

            var an = control.AnchorLocation;
            switch (an.Horizontal)
            {
                case Anchor.HorizontalAlignment.Center:
                    control.Location = new Vector2(((Width / 2) - (an.Width / 2) + an.Position.Left) - an.Position.Right, Location.Y);
                    break;
                case Anchor.HorizontalAlignment.Left:
                    control.Location = new Vector2((Location.X + an.Position.Left) - an.Position.Right, Location.Y);
                    break;
                case Anchor.HorizontalAlignment.Right:
                    control.Location = new Vector2(RectObjectUI.Right- an.Width - an.Position.Right, Location.Y);
                    break;
                case Anchor.HorizontalAlignment.Full:
                    control.Location = new Vector2(Location.X, Location.Y);
                    control .Width = an.Width;
                    break;
            }
            switch (an.Vertical)
            {
                case Anchor.VerticalAlignment.Top:
                    control.Location = new Vector2(control.Location.X, Location.Y + an.Position.Top );
                    break;
                case Anchor.VerticalAlignment.Bottom:
                    control.Location = new Vector2(control.Location.X, RectObjectUI.Bottom -  an.Height - an.Position.Bottom);
                    break;
                case Anchor.VerticalAlignment.Center:
                    control.Location = new Vector2(control.Location.X, ((Height / 2) - (an.Height / 2) - an.Position.Top) + an.Position.Bottom);
                    break;
                case Anchor.VerticalAlignment.Full:
                    control.Location = new Vector2(control.Location.X, Location.Y);
                    control.Height = an.Height;
                    break;
            }

            an.Resize = true;
        }

      

    }
}
