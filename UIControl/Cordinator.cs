using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace UIControl_MonoGame.UIControl
{
    public class Cordinator
    {
        private const int CLICKTIMER = 150; //Delay at the next mouse click

        private bool _clicker = false;
        private readonly Stopwatch Stopwatch_clicker = new ();
        private Texture2D _RedPixel;

        /// <summary>
        /// Position and size of the control
        /// </summary>
        protected Rectangle RectObjectUI;
        protected Game _game;
        /// <summary>
        /// One tab symbol
        /// </summary>
        protected const string INDENT = "    ";

        protected Texture2D RedPixel
        {
            get
            {
                if (_RedPixel is null)
                {
                    _RedPixel = new Texture2D(_game.GraphicsDevice, 1, 1);
                    _RedPixel.SetData([Color.Red]);
                }
                return _RedPixel;
            }
        }
        protected bool Cliker
        {
            get
            {
                if (_clicker && Stopwatch_clicker.ElapsedMilliseconds >= CLICKTIMER) Stopwatch_clicker.Reset();
                return Stopwatch_clicker.IsRunning;
            }
            set
            {
                _clicker = true;
                Stopwatch_clicker.Start();
            }
        }

        protected Texture2D Rect2D(Rectangle rectangle)
        {            
            Texture2D rectTexture;
            Color[] data = new Color[rectangle.Width * rectangle.Height];
            rectTexture = new Texture2D(_game.GraphicsDevice, rectangle.Width, rectangle.Height);

            for (int i = 0; i < data.Length; ++i)
                data[i] = Color.White;

            rectTexture.SetData(data);

            return rectTexture;
        }

        /// <summary>
        /// Parameters for the text
        /// </summary>
        public class UIText : AdvancedSettings, IToXml
        {
            private SpriteFont _font;
            private string _fontName;
            private readonly Game _game;

            /// <summary>
            /// Displayed text
            /// </summary>
            public string Text { get; set; } = string.Empty;
            /// <summary>
            /// Text position
            /// </summary>
            public TextPositionEnum Position { get; set; } = TextPositionEnum.Center;
            /// <summary>
            /// Text color. Black by default
            /// </summary>
            public Color ColorText { get; set; } = Color.Black;
            /// <summary>
            /// The font that is uploaded to Content
            /// </summary>
            public string FontName
            {
                get => _fontName; 
                set
                {
                    _font= _game.Content.Load<SpriteFont>(value);
                    _fontName = value;
                }
            }
            public SpriteFont Font{ get => _font; }
           
            public UIText(Game game, string text, string fontContent)
            {
                _game = game;
                Text = text;
                FontName=fontContent;
            }

            public void Display(SpriteBatch spriteBatch, Rectangle rectObj )
            {               
                spriteBatch.DrawString(Font, Text, GetPosition(rectObj), ColorText, Rotation, Origin, Scale, Effects, Layer);
            }

            /// <summary>
            /// Defines the position of the text relative to the TextPositionEnum. The text offset should be used Origin
            /// </summary>
            public Vector2 GetPosition(Rectangle rectObj) {
                Vector2 fontOrigin;
                if (Text == string.Empty) fontOrigin = Font.MeasureString(" ");
                else fontOrigin = Font.MeasureString(Text);
                var poss = Position switch
                {
                    UIText.TextPositionEnum.Left => new(rectObj.X, (rectObj.Y + ((rectObj.Height - fontOrigin.Y) / 2))),
                    UIText.TextPositionEnum.Center => new(rectObj.X + ((rectObj.Width - fontOrigin.X) / 2), (rectObj.Y + ((rectObj.Height - fontOrigin.Y) / 2))),
                    UIText.TextPositionEnum.Right => new((rectObj.X + rectObj.Width) - fontOrigin.X, (rectObj.Y + ((rectObj.Height - fontOrigin.Y) / 2))),
                    _ => new Vector2(rectObj.X, rectObj.Y),
                };
                return poss;
            }

            public string ToXml()=> INDENT + INDENT + IToXml.ConvertXml(this)[..^2] + "/>";

            /// <summary>
            /// Text position
            /// </summary>
            public enum TextPositionEnum
            {
                Left, Center, Right
            }
        }

        /// <summary>
        /// Universal texture as animation or without it
        /// </summary>
        public class UITexture: AdvancedSettings, IToXml
        {     
            private int _indexDelay = 0;
            private readonly List<Frame> _frames = [];
            private double _elapsedTime = 0;

            public Color ColorFrame { get; set; } = Color.White;

            public UITexture(Game game, string textuteContent)
            {
                _frames.Add(new Frame(game,textuteContent, 0));
            }
            public UITexture(Game game, string textuteContent, Rectangle rectButton )
            {
                _frames.Add(new Frame(game, textuteContent, 0, rectButton));
            }
            public UITexture(Texture2D texture)
            {
                _frames.Add(new Frame(texture));
            }
            public UITexture(List<Frame> frames)
            {
                _frames = frames;
            }

            public void Display(SpriteBatch spriteBatch, GameTime gameTime, Rectangle rectDisplay)
            {
                if (_frames.Count == 0) { }
                else if (_frames.Count == 1)
                {
                    spriteBatch.Draw(_frames[0].TextureFrame, rectDisplay, _frames[0].RectangelFrame, ColorFrame);
                }                
                else
                {
                    Frame frame = _frames[_indexDelay];                    
                    spriteBatch.Draw(_frames[_indexDelay].TextureFrame, rectDisplay, _frames[_indexDelay].RectangelFrame, ColorFrame, Rotation, Origin, Effects, Layer);
                    _elapsedTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (_elapsedTime >= frame.Delay)
                    {
                        _elapsedTime = 0;                        
                        _indexDelay++;
                        if (_indexDelay >= _frames.Count) _indexDelay = 0;
                    }
                }
            }

            public string ToXml()
            {
                string deep = "\n" + INDENT + INDENT + INDENT+ "<Frames>";
                foreach (var frame in _frames) { 
                 deep+= frame.ToXml();
                }
                deep += "\n" + INDENT + INDENT + INDENT + "</Frames>";
                return INDENT + INDENT + IToXml.ConvertXml(this)[..^2] + ">" + deep + "\n" + INDENT + INDENT +  "</UITexture>";
            }

            /// <summary>
            /// Frame-by-frame animation
            /// </summary>
            public readonly struct Frame: IToXml
            {
                /// <summary>
                /// A texture or one large texture with multiple frames
                /// </summary>
                public readonly Texture2D TextureFrame;
                /// <summary>
                /// Delay between frames
                /// </summary>
                public readonly float Delay;
                /// <summary>
                /// It is used when there is a large texture from the selected frame. When one texture is used as the default texture size
                /// </summary>
                public readonly Rectangle RectangelFrame;
                /// <summary>
                /// The name of the texture is used to save
                /// </summary>
                public readonly string TextureName;

                /// <summary>
                /// Creates a frame
                /// </summary>
                /// <param name="texture">A texture or one large texture with multiple frames</param>
                /// <param name="delay">Delay between frames</param>
                /// <param name="rectangle">It is used when there is a large texture from the selected frame. When one texture is used as the default texture size</param>
                public Frame(Game game, string contentTexture, uint delay, Rectangle rectangle)
                {
                    TextureFrame = game.Content.Load<Texture2D >(contentTexture);
                    TextureName = contentTexture;
                    Delay = delay;
                    RectangelFrame = rectangle;
                }
                /// <summary>
                /// Creates a frame
                /// </summary>
                /// <param name="texture">A texture or one large texture with multiple frames</param>
                /// <param name="delay">Delay between frames</param>
                public Frame(Game game, string contentTexture, uint delay)
                {
                    TextureFrame = game.Content.Load<Texture2D>(contentTexture);
                    TextureName = contentTexture;
                    Delay = delay;
                    RectangelFrame = new Rectangle(0, 0, TextureFrame.Width, TextureFrame.Height);
                }
                /// <summary>
                /// Creates a frame
                /// </summary>
                public Frame(Texture2D texture)
                {
                    TextureFrame = texture;
                    TextureName = string.Empty;
                }

                public string ToXml() => "\n"+  INDENT + INDENT + INDENT + INDENT + "<Frame Delay=\""+ Delay  + "\" TextureName=\""+ TextureName + "\" RectangelFrame=\""+RectangelFrame.ToString()+"\"/>";
            }
        }

        /// <summary>
        /// Alignment setting
        /// </summary>
        public class Anchor(int height, int width, Anchor.HorizontalAlignment horizontal, Anchor.VerticalAlignment vertical, Anchor.Margin position) : IToXml
        {
            public HorizontalAlignment Horizontal { get; set; } = horizontal;
            public VerticalAlignment Vertical { get; set; } = vertical;
            public int Height { get; set; } = height;
            public int Width { get; set; } = width;
            public Margin Position { get; set; } = position;
            public bool Resize = false;

            public struct Margin(int top, int bottom, int left, int right)
            {
                public int Top = top;
                public int Bottom = bottom;
                public int Left = left;
                public int Right = right;

                public override readonly string ToString()=> "\"" + Left + ", " + Top + ", " + Right + ", " + Bottom + "\n";
                public static Margin MarginZero { get => new(0, 0, 0, 0); }
            }

            public string ToXml() => INDENT + INDENT + IToXml.ConvertXml(this)[..^2] ;

            public enum HorizontalAlignment
            {
                Center, Left, Right, Full
            }

            public enum VerticalAlignment
            {
                Top, Bottom, Center, Full
            }
        }



    }
}
