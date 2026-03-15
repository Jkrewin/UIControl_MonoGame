using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using UIControl_MonoGame.UIControl;
using static UIControl_MonoGame.UIControl.Cordinator.UIText;


namespace UIControl_MonoGame.Extra
{
    public  class Editor 
    {
        private FileSystemWatcher _watcher;
        private string SFile = AppContext.BaseDirectory + "editor.xml";
        private bool _saveFile = false;
        private DateTime _lastRead;
        private readonly Grup MainGrup;

        public Editor(Grup grup) 
        {            
            string directory = Path.GetDirectoryName(AppContext.BaseDirectory);
            string fileName = Path.GetFileName(SFile);
            MainGrup = grup; 

             _watcher = new FileSystemWatcher(directory, fileName)
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };
                      
            _watcher.Changed += OnChanged;
        }

        private void OnChanged(object sender, FileSystemEventArgs e)
        {           
            if (File.Exists(SFile) )
            {
                var lastWrite = File.GetLastWriteTime(SFile);
                if (lastWrite > _lastRead)
                {
                    _lastRead = lastWrite;
                    Task.Delay(100).ContinueWith(_ =>
                    {
                        try
                        {
                            // read file
                            if (_saveFile == true)
                            {
                                _saveFile = false;
                                return;
                            }

                            Load();

                        }
                        catch  { throw; }
                    });
                }
            }
        }


        public void Load() {
            const string errro = "The control with the name cannot be found";
            
            var doc = XDocument.Load(SFile);
            var root = doc.Root;

            if (root == null) return;

            foreach (XElement elem in root.Elements())
            {
                if (elem.Name.LocalName == nameof(LabelUI)) {
                    var label = MainGrup.FindControl<LabelUI>(elem.Attribute("Name")?.Value) ?? throw new Exception(errro + " " + elem.Attribute("Name")?.Value);
                    SetClass(label, elem);

                    foreach (XElement elem2 in elem.Elements())
                    {
                        var c = elem2.Attribute("Object");
                        if (c != null) SetClass(label.GetType().GetProperty(c.Value).GetValue(label), elem2);
                    }
                }
                else throw new Exception("Unregistered Elements " + elem.Name);
            }
        }


        public void SaveXml() {
            if (MainGrup is null) return;
            _saveFile = true;
            File.WriteAllText(SFile, 
                "<?xml version=\"1.0\" encoding=\"UTF-8\"?> \n <"+ MainGrup.GetType().Name + ">" + MainGrup.ToXml() + "\n"+ ContentList() + "\n</"+ MainGrup.GetType().Name+">", System.Text.Encoding.Default);
            
        }


        private static string ContentList() {
            var fonts= "<Sprites>";
            var contentDirectory = AppContext.BaseDirectory + "Content\\";
            if (!Directory.Exists(contentDirectory)) return "";               

            var xnbFiles = Directory.GetFiles(contentDirectory, "*.spritefont", SearchOption.AllDirectories);
            foreach (var file in xnbFiles)
            {
                string assetName = Path.GetRelativePath(contentDirectory, file).Replace(".spritefont", "").Replace("\\", "/");
                fonts += "\n      <SpriteFont>" + assetName + "</SpriteFont>";
            }

            xnbFiles = Directory.GetFiles(contentDirectory, "*.png", SearchOption.AllDirectories);
            foreach (var file in xnbFiles)
            {
                string assetName = Path.GetRelativePath(contentDirectory, file).Replace(".png", "").Replace("\\", "/");
                fonts += "\n      <Texture2D>" + assetName + "</Texture2D>";
            }

            return fonts + "\n</Sprites>";
        }

        private void SetClass(object ui, XElement x)
        {
            foreach (var item in ui.GetType().GetProperties().Where(x => x.CanRead & x.CanWrite))
            {
                if (item.Name == "Name" | item.Name == "Object") continue;
                if (x.Attribute(item.Name) != null) {
                    switch (item.PropertyType.Name)
                    {
                        case nameof(Vector2):
                            item.SetValue(ui, vector2Parser(x.Attribute(item.Name)?.Value));
                            break;
                        case nameof(Microsoft.Xna.Framework.Rectangle):                            
                            item.SetValue(ui, parseRectangle(x.Attribute(item.Name)?.Value));
                            break;
                        case nameof(Boolean):
                            item.SetValue(ui, bool.Parse(x.Attribute(item.Name)?.Value));
                            break;
                        case nameof(Int32):
                            item.SetValue(ui, int.Parse(x.Attribute(item.Name)?.Value));
                            break;
                        case nameof(String):
                            item.SetValue(ui, x.Attribute(item.Name)?.Value);
                            break;
                        case nameof(TextPositionEnum):
                            item.SetValue(ui, Enum.Parse(typeof(TextPositionEnum), x.Attribute(item.Name)?.Value));
                            break;
                        case nameof(Microsoft.Xna.Framework.Color):
                            item.SetValue(ui, parseColor(x.Attribute(item.Name)?.Value));
                            break;
                        case nameof(Single):
                            item.SetValue(ui, Single.Parse( x.Attribute(item.Name)?.Value));
                            break;
                        case nameof(SpriteEffects):
                            item.SetValue(ui, Enum.Parse(typeof(SpriteEffects), x.Attribute(item.Name)?.Value));
                            break;
                        default:
                            throw new Exception("Unregistered type " + item.PropertyType.Name);
                    }                
                }
            }


            Vector2 vector2Parser(string pos)
            {
                int startInd = pos.IndexOf("X:") + 2;
                float aXPosition = float.Parse(pos.Substring(startInd, pos.IndexOf(" Y") - startInd));
                startInd = pos.IndexOf("Y:") + 2;
                float aYPosition = float.Parse(pos.Substring(startInd, pos.IndexOf("}") - startInd));
                return new Vector2(aXPosition, aYPosition);
            }
            Microsoft.Xna.Framework.Rectangle parseRectangle(string input)
            {
                string[] parts = input.Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length != 4)
                    throw new ArgumentException("Incorrect string format for Rectangle");
                return new Microsoft.Xna.Framework.Rectangle(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]), int.Parse(parts[3]));
            }
            Microsoft.Xna.Framework.Color parseColor(string value)
            {
                if (value == string.Empty) return Microsoft.Xna.Framework.Color.White;
                value = value.Trim('{', '}');
                var parts = value.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                byte r = 0, g = 0, b = 0, a = 255;

                foreach (var part in parts)
                {
                    var kv = part.Split(':');
                    if (kv.Length != 2) continue;

                    string key = kv[0].Trim();
                    byte val = byte.Parse(kv[1].Trim());

                    switch (key)
                    {
                        case "R": r = val; break;
                        case "G": g = val; break;
                        case "B": b = val; break;
                        case "A": a = val; break;
                    }
                }

                return new Microsoft.Xna.Framework.Color(r, g, b, a);
            }
        }
        

}
}
    

