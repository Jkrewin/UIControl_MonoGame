using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UIControl_MonoGame.Extra;
using UIControl_MonoGame.UIControl;

namespace UIControl_MonoGame
{
    /// <summary>
    /// For normal works, you will need MonoGame Framework C# project templates- https://marketplace.visualstudio.com/items?itemName=MonoGame.MonoGame-Templates-VSExtension
    /// Rebild Content.mgcb
    /// </summary>


    public class Game1 : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private UIControl.Group SelectedGroup; // Implements a way to switch between groups
        private UIControl.Group Group1;
        private UIControl.Group Group2;
        private Extra.FormEditor FormEditor ;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 852;
            _graphics.PreferredBackBufferHeight = 550;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            /// 1. Create a group. Allows you to manage multiple controls at once. Groups can contain other groups; the group must be specified in the methods Update and Draw
            Group1 = new Group(this, new Rectangle(10, 10, 800, 500), "Group1");

            /// 2. Create graphics and animations for which controls. Content It must contain textures and font.
            //
            // Animation picture 
            List<UIControl.Cordinator.UITexture.Frame> animation = [];
            for (int i = 0; i < 7; i++) animation.Add(new Cordinator.UITexture.Frame(this, "HowTo_anim", 200, new Rectangle(250 * i, 0, 250, 475)));
            //
            //Here are the textures for the check Button
            var uncheck = new Cordinator.UITexture(this, "checkButton", new Rectangle(0, 645, 1210, 645)); // The texture for the selected check button
            var check = new Cordinator.UITexture(this, "checkButton", new Rectangle(0, 0, 1210, 645));// The button the standard state of the texture
            //
            //Here are the textures for the button
            var txtButton = new Cordinator.UITexture(this, "ButtonTest", new Rectangle(0, 82, 302, 82));
            var txtButton_s = new Cordinator.UITexture(this, "ButtonTest", new Rectangle(0, 0, 302, 82));
            //
            //Side icon for ListItem
            Texture2D texture_icon = Content.Load<Texture2D>("icon");
            //A list for choosing the difficulty level
            List<TestRow> testRows =[];
            testRows.Add(new TestRow(texture_icon, "Easy"));
            testRows.Add(new TestRow(texture_icon, "Normal"));
            testRows.Add(new TestRow(texture_icon, "Hard"));
            
            /// 3. Add Controls
            Group1.Add(new ImageUI(this, "bg", new(10, 10, 800, 500), "dialogBox")); // Background of the dialog box
            Group1.Add(new LabelUI(this,"title", new Rectangle(300,100,200,20), "Title Form", "TitleFont")); // Add label 
            Group1.Add(new LabelUI(this, "label1", new Rectangle(100, 160, 200, 20), "Player", "LabelText"));
            Group1.Add(new LabelUI(this, "label2", new Rectangle(100, 240, 200, 20), "Save Score", "LabelText"));

            var t1 = new TextBoxUI(this, "playername", new Rectangle(250, 150, 300, 50), "LabelText", "");  // Text Box
            t1.MainTexture = new Cordinator.UITexture(this, "Textbox");// adding the main texture
            t1.Caption.ColorText = Color.DeepPink;      // Color text
            t1.Caption.Origin = new(-15,2); // shifting the text so that it does not go beyond the texture
            Group1.Add(t1);
            Group1.Add(new CheckBoxUI(this, "checkbox", new(280, 225, 100, 50), check, uncheck));    // Check Box
            var t2 = new ButtonUI(this, "button1", new(280, 390, 200, 50)) // Button
            {
                TextureButton = txtButton,
                SelectedTextureButton = txtButton_s,
                Caption = new Cordinator.UIText(this, "Start Game", "LabelText"),
            };
            t2.Caption.ColorText = Color.White;
            t2.OnClick += ButtonImg_OnClick;
            Group1.Add(t2);
            ListItemUI l = new (this, "listbox", new(500, 250, 200, 200), testRows, "LabelText"); // List box
            l.Сolumns[0].WidthRows = l.Сolumns[0].HeightRows; // We make this column to fit the texture size
            Group1.Add(l);
            Group1.Add(new ImageUI(this,"image2",new Rectangle(100,300,130,150), animation));    // Image
            SelectedGroup = Group1;

           // var r = new Editor(Group1);  
            //r.SaveXml();
            // r.Load();

            /* List<ListItemUI.StandartItem> standart = [];
             standart.Add(new ListItemUI.StandartItem("item1"));
             standart.Add(new ListItemUI.StandartItem("item2"));
             standart.Add(new ListItemUI.StandartItem("item3"));
             standart.Add(new ListItemUI.StandartItem("item4"));
             standart.Add(new ListItemUI.StandartItem("item5"));
             standart.Add(new ListItemUI.StandartItem("item6"));
             l.LoadItems(standart);
             Group2 = new Group(this, new Rectangle(10, 10, 800, 500), "Group2");
             Group2.ShowRedLine = true;
             t1.AnchorLocation = new Cordinator.Anchor(50, 300, HorizontalAlignment.Left, VerticalAlignment.Center, new Margin(50,66,150,0));
             Group2.Add(t1);*/
            SelectedGroup = Group1;

            Er();


        }

        private void FormEditor_OnChangeData(string data)
        {
            
        }

        /// <summary>
        /// To select from the list for the difficulty level
        /// </summary>
        public class TestRow(Texture2D t, string level)
        {
            public Texture2D Texture { get; set; } = t;
            public string Level { get; set; } = level;
        }

        private void ButtonImg_OnClick()
        {
           Exit();
        }

        private void Er() {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                FormEditor = new FormEditor(SelectedGroup);
                FormEditor.OnChangeData += FormEditor_OnChangeData; ;
            }
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
               
            }


            SelectedGroup.ControlEvent(Mouse.GetState(), Keyboard.GetState(), 0); // Manages the input device this group

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);
            _spriteBatch.Begin();

            SelectedGroup.Draw(gameTime, _spriteBatch);     // Manages graphics for elements within a group

            _spriteBatch.End();
            base.Draw(gameTime);
        }


       
    }
}
