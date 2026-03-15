using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using static UIControl_MonoGame.UIControl.ListItemUI.Row;

namespace UIControl_MonoGame.UIControl
{
    public class ListItemUI : Cordinator, IControlUI, IToXml
    {
        private List<Сolumn> _сolumns;
        private readonly string DefaultText;

        /// <summary>
        /// Highlights a control in a red frame to make it easier to create a design.
        /// </summary>
        public bool ShowRedLine { get; set; } = false   ;
        public Vector2 Location { get => new(RectObjectUI.X, RectObjectUI.Y); set => RectObjectUI = new Rectangle((int)value.X, (int)value.Y, RectObjectUI.Width, RectObjectUI.Height); }
        public bool Visible { get; set; }
        public bool Focused { get; set; }
        public string Name { get; set; }
        public int SelectedRow { get; set; } = -1;
        public int Height { get => RectObjectUI.Height; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, RectObjectUI.Width, value); }
        public int Width { get => RectObjectUI.Width; set => RectObjectUI = new Rectangle(RectObjectUI.X, RectObjectUI.Y, value, RectObjectUI.Height); }
        public Anchor AnchorLocation { get; set; }

        /// <summary>
        /// Line thickness separating columns and rows
        /// </summary>
        public int Thickness { get; set; } = 2;
        [XmlIgnore] public List<Row> DataItems { get; set; }
        public Color SelectionColor { get; set; } = Color.LightBlue;

        public delegate void Click( );
        public event Click OnClick;
        public delegate void SelectItem(Row row);
        public event SelectItem OnSelectItem;
        public delegate void MouseEnter();
        public event MouseEnter OnMouseEnter;
        public delegate void MouseLeave();
        public event MouseLeave OnMouseLeave;
        public delegate void SetFocuse();
        public event SetFocuse OnSetFocuse;

        public Сolumn[] Сolumns 
        {
            get => [.. _сolumns];
        }

        public ListItemUI(Game game, string nameUI, Rectangle recPoss, IList list, string contentFont) {
            if (string.IsNullOrEmpty(nameUI)) throw new ArgumentNullException(nameof(nameUI));

            _game = game;
            Name = nameUI;
            RectObjectUI = recPoss;
            DefaultText = contentFont;
            LoadItems( list);
            Visible = true;
        }
        /// <summary>
        /// Change the column height
        /// </summary>
        /// <param name="height"></param>
        public void ResizeColumns_Height(int height) {
            foreach (var item in _сolumns)
            {
                item.HeightRows = height;
            }
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
                OnSetFocuse?.Invoke();
                OnClick?.Invoke();
                if (_сolumns.Count != 0) {
                    var cursor = new Rectangle(getMouse.X, getMouse.Y, 2, 2);
                    for (int i = 0; i < DataItems.Count; i++)
                    {
                        var up = DataItems[i].Column[0].HeightRows;
                        var rec = new Rectangle(RectObjectUI.X, RectObjectUI.Y + (up * i), RectObjectUI.Width, up);
                        if (rec.Intersects(cursor))
                        {
                            SelectedRow = i;
                            OnSelectItem?.Invoke(DataItems[i]);
                            break;
                        }
                    }
                }
            }
            else if (Focused & getKey.IsKeyDown(Keys.Down) & Cliker == false) {
                Cliker = true;
                SelectedRow++;
                if (SelectedRow == DataItems.Count) SelectedRow=0;
                OnSelectItem?.Invoke(DataItems[SelectedRow]);
            }
            else if (Focused & getKey.IsKeyDown(Keys.Up) & Cliker == false)
            {
                Cliker = true;
                SelectedRow--;
                if (SelectedRow == -1) SelectedRow = DataItems.Count-1;
                OnSelectItem?.Invoke(DataItems[SelectedRow]);
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

            int start = RectObjectUI.Y;
            Color sel;
            int t = 0;
            foreach (var row in DataItems)
            {                
                int first = RectObjectUI.X;
                if (SelectedRow == t) sel = SelectionColor;
                else sel = Color.White;
                for (int i = 0; i < row.Items.Length; i++)
                {
                    if (row.Items[i].Background != null) { spriteBatch.Draw(row.Items[i].Background, new Rectangle(first, start, row.Column[i].WidthRows, row.Column[i].HeightRows), null, sel); }
                    row.Items[i].Caption.Display(spriteBatch, new Rectangle(first, start, row.Column[i].WidthRows, row.Column[i].HeightRows));
                    first += row.Column[i].WidthRows+ Thickness;
                }
                start += row.Column[0].HeightRows + Thickness;
                t++;
                if (start > (RectObjectUI.Height + RectObjectUI.Y- row.Column[0].HeightRows + Thickness)) break; // the rows went beyond the control
            }

            if (ShowRedLine)
            {               
                Color color = Color.White;
                spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X, RectObjectUI.Y, RectObjectUI.Width+ Thickness, Thickness), color);
                spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X, RectObjectUI.Y + RectObjectUI.Height - Thickness, RectObjectUI.Width, Thickness), color);
                spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X, RectObjectUI.Y, Thickness, RectObjectUI.Height), color);
                spriteBatch.Draw(RedPixel, new Rectangle(RectObjectUI.X + RectObjectUI.Width , RectObjectUI.Y, Thickness, RectObjectUI.Height), color);
            }
        }

        /// <summary>
        /// Loads a list of classes where properties ({ get; set; } or { get; }) are used as columns
        /// properties Texture2D - Automatically used as a background
        /// ElementItems - Contains a reference to the object to read
        /// </summary>
        /// <param name="list">List - Class </param>
        public void LoadItems(IList list ) {
            if (list.Count==0) return;
            DataItems = [];
            _сolumns = [];
            var ls = list[0].GetType().GetProperties().Where(x => x.CanRead);

            foreach (var t in ls)
            {               
                var c = new Сolumn()
                {
                    Name = t.Name,
                    WidthRows = RectObjectUI.Width / ls.Count()
                };
                _сolumns.Add(c);
            }

            foreach (var item in list) 
            {
                Item[] items = new Item[_сolumns.Count];
                int i = 0;
                foreach (var value in item.GetType().GetProperties().Where(x => x.CanRead) )
                {
                    if (value.GetValue(item) is Texture2D texture)
                    {
                        items[i] = new Item(_game, DefaultText, " ", texture);
                    }
                    else 
                    {
                        string deep;
                        if (value.GetValue(item) is null) deep = string.Empty;
                        else deep = value.GetValue(item).ToString();
                        items[i] = new Item(_game, DefaultText, deep, Rect2D(new Rectangle(0, 0, 10, 10))); 
                    }                        
                    i++;
                }
                DataItems.Add(new Row(item, _сolumns, items));
            }

            // Sets default Height
            try
            {
                var s = DataItems[0].Items[0].Caption;
                ResizeColumns_Height((int)(s.Font.MeasureString(s.Text).Y + 15));
            }
            catch 
            {
                ResizeColumns_Height(30);
            }
        }

        public string ToXml()
        {
            string deep = "\n" + INDENT + INDENT  + "<DataItems>";
            foreach (var item in  DataItems )
            {
                deep += "\n" + INDENT + INDENT+ INDENT + "<Items>" + item.ToString() + "</Items>";
            }
            deep += "\n"+ INDENT + INDENT+ "</DataItems>";

            return $"{INDENT}{IToXml.ConvertXml(this)[..^2]}>{deep}\n{INDENT}</{this.GetType().Name}>";
        }

        public class Сolumn
        {
            public string Name { get; set; }            
            public int HeightRows { get; set; }
            public int WidthRows { get; set; }
        }

        public class Row(object obj, List<Сolumn> сolumn, Item[] items )
        {
            /// <summary>
            /// Link to the class with the data
            /// </summary>
            public object ElementItems { get; set; } = obj;
            /// <summary>
            /// Columns in the configuration table
            /// </summary>
            public readonly List<Сolumn> Column = сolumn;
            /// <summary>
            /// Cells in the row
            /// </summary>
            public readonly Item[] Items = items;

            public override bool Equals(object obj)=> ElementItems == obj;
            public override int GetHashCode() =>ElementItems.GetHashCode();

            public class Item(Game game, string fontContent, string value, Texture2D texture)
            {
                /// <summary>
                /// The background can also be used as an icon in this row.
                /// </summary>
                public Texture2D Background { get; } = texture;
                /// <summary>
                /// The value that will be displayed in the table
                /// </summary>
                public string Value { get => Caption.Text;  }
                /// <summary>
                /// Text settings in the table for this item
                /// </summary>
                public UIText Caption { get; } = new UIText(game, value, fontContent);

                public override string ToString() => Caption.Text;
            }
        }
        
    }
}
