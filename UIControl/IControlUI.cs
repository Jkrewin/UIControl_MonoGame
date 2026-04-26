using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using static UIControl_MonoGame.UIControl.Cordinator;

namespace UIControl_MonoGame.UIControl
{
    public interface IControlUI
    {
        /// <summary>
        /// Сoordinates of the UI object
        /// </summary>
        public Vector2 Location { get; set; }
        /// <summary>
        /// Is the object visible or not
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// The object in focus
        /// </summary>
        public bool Focused { get; set; }
        /// <summary>
        /// Name of the object
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Height of the control
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Width of the control
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Gets or sets the alignment characteristics applied to this element when it is composed within a parent Group, such as a panel or items control
        /// In this case, it excludes the use of Location
        /// </summary>
        public Anchor AnchorLocation { get; set; }

        /// <summary>
        /// Must always be in <i>protected override void Update(GameTime gameTime)</i> 
        /// </summary>
        /// <param name="getMouse">Mouse.GetState())</param>
        /// <param name="getKey">Keyboard.GetState()</param>
        /// <param name="getJoy">--</param>
        public void ControlEvent(MouseState getMouse, KeyboardState getKey, uint getJoy = 1);
        /// <summary>
        /// For protected override void Draw(GameTime gameTime)
        /// </summary>
        /// <param name="gameTime">gameTime</param>
        /// <param name="spriteBatch">_spriteBatch</param>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }

    /// <summary>
    /// Advanced settings for Draw or DrawString
    /// </summary>
    public class AdvancedSettings
    {
        /// <summary>
        /// The origin of the string. Specify (0,0) for the upper-left corner.
        /// </summary>
        public Vector2 Origin { get; set; } = Vector2.Zero;
        /// <summary>
        /// The angle, in radians, to rotate the text around the origin.
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// Vector containing separate scalar multiples for the x- and y-axes of the sprite.
        /// </summary>
        public float Scale { get; set; } = 1.0f;
        /// <summary>
        /// Rotations to apply prior to rendering.
        /// </summary>
        public SpriteEffects Effects { get; set; } = SpriteEffects.None;
        /// <summary>
        /// The sorting depth of the sprite, between 0 (front) and 1 (back).
        /// </summary>
        public float Layer { get; set; } = 0.5f;
    }

    /// <summary>
    /// Suitable for creating a file XML
    /// </summary>
    public interface IToXml
    {
        /// <summary>
        /// A string for XML
        /// </summary>
        /// <returns>XML</returns>
        string ToXml();

        static public string ConvertXml(object obj)
        {
            string xml = "<" + obj.GetType().Name;
            string preXml = "";
            foreach (var item in obj.GetType().GetProperties().Where(x => x.CanRead & x.CanWrite))
            {
                if (!item.GetCustomAttributes(true).Any(x => x is XmlIgnoreAttribute))
                {
                    if (item.GetValue(obj) is null) { }
                    else if (item.GetValue(obj) is IToXml toXml)
                    {
                        // class
                        string p = toXml.ToXml();
                        if (p == string.Empty) continue;
                        string ps = p.Split("<")[1];
                        string spen = string.Concat("<", ps.AsSpan(0, ps.IndexOf(' ')));
                        string[] pn = p.Split(spen);
                        p = pn[0] + spen + " Object=\"" + item.Name + "\" " + pn[1];
                        preXml += "\n" + p + "  ";
                    }
                    else {
                        var tt = item.PropertyType;
                        if (tt.Name == nameof(Int32)) xml += " " + item.Name + "=\"" + ((Int32)item.GetValue(obj)).ToString(CultureInfo.InvariantCulture) + "\" ";
                        else if (tt.Name == nameof(Decimal)) xml += " " + item.Name + "=\"" + ((Decimal)item.GetValue(obj)).ToString(CultureInfo.InvariantCulture) + "\" ";
                        else if (tt.Name == nameof(Single)) xml += " " + item.Name + "=\"" + ((Single)item.GetValue(obj)).ToString(CultureInfo.InvariantCulture) + "\" ";
                        else if (tt.Name == nameof(Int16)) xml += " " + item.Name + "=\"" + ((Int16)item.GetValue(obj)).ToString(CultureInfo.InvariantCulture) + "\" ";
                        else if (tt.Name == nameof(Int64)) xml += " " + item.Name + "=\"" + ((Int64)item.GetValue(obj)).ToString(CultureInfo.InvariantCulture) + "\" ";
                        else if (tt.Name == nameof(Int128)) xml += " " + item.Name + "=\"" + ((Int128)item.GetValue(obj)).ToString(CultureInfo.InvariantCulture) + "\" ";
                        else xml += " " + item.Name + "=\"" + item.GetValue(obj).ToString() + "\" ";
                    }
                }
            }

            return xml[..^1] + ">" + preXml + "\n";
        }
    }
}
