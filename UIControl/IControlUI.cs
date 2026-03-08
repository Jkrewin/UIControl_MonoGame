using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

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
        /// Must always be in <i>protected override void Update(GameTime gameTime)</i> 
        /// </summary>
        /// <param name="getMouse">Mouse.GetState())</param>
        /// <param name="getKey">Keyboard.GetState()</param>
        /// <param name="getJoy">--</param>
        public void ControlEvent(MouseState getMouse , KeyboardState getKey , uint  getJoy  = 1);
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
    public class AdvancedSettings {
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
}
