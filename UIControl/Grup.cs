using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace UIControl_MonoGame.UIControl
{
    internal class Grup : Cordinator, IControlUI
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
            }
        }
        public bool Visible { get; set; } = true;
        public bool Focused { get; set; }
        public string Name { get; set; }
        public int Height { get => RectObjectUI.Height ; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y , RectObjectUI.Width, value); }
        public int Width { get => RectObjectUI.Width; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, value, RectObjectUI.Height); }

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

            if (control is UIControl.RadioButtonUI r) {
                r.OnResetRadioButton += ResetRadioButton;
            }

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

        public void Remove(IControlUI control) 
        {
            if (control is UIControl.RadioButtonUI r) r.OnResetRadioButton -= ResetRadioButton;
            Controls.Remove(control);
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

        private void ResetRadioButton(RadioButtonUI r ) {
            foreach (var item in Controls)
            {
                if (item is RadioButtonUI cntr)
                {
                    if (r != cntr) cntr.IsChecked=false;
                }
            }
        }

    }
}
