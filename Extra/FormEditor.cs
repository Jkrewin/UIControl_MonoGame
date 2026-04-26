using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.Versioning;
using System.Text;
using System.Timers;
using System.Xml;
using System.Xml.Linq;
using UIControl_MonoGame.UIControl;

namespace UIControl_MonoGame.Extra
{
    [SupportedOSPlatform("windows")]
    public  class FormEditor: IDisposable
    {
        private const string MapName = "FileEditUI";
        private const long Size = 65536;
        private const int FlagOffset = 0;
        private const int DataOffset = 4;

        private readonly MemoryMappedFile _mmf;
        private bool _disposed;
        private readonly Timer _timer;
        private readonly Group EditGroup;

        public delegate void OnChange(string data);
        public event OnChange OnChangeData;

        public string XML_Content { get => "" + EditGroup.ToXmlTitle() + "" + EditGroup.ToXml() + "\n</" + EditGroup.GetType().Name + ">\n" + ContentList() ; } //<?xml version=\"1.0\" encoding=\"UTF-8\"?> \n 

        public FormEditor(Group Groupedit = null)
        {
            EditGroup= Groupedit;
            if (Groupedit is null)
            {
                // Connect to file 
                _mmf = MemoryMappedFile.CreateOrOpen(MapName, Size);
            }
            else {
                _mmf = MemoryMappedFile.OpenExisting(MapName);
                WriteAndSignal(XML_Content);
                _timer = new Timer(1000);
                _timer.Elapsed += _timer_Elapsed;
                _timer.AutoReset = true;
                _timer.Start();
            }
        }

        private static string ContentList()
        {
            var fonts = "<Sprites>";
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

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {           
            using var accessor = _mmf.CreateViewAccessor();
            accessor.Read(FlagOffset, out int flag);

            if (flag == 1)
            {
                byte[] buffer = new byte[Size - DataOffset];
                accessor.ReadArray(DataOffset, buffer, 0, buffer.Length);
                string data = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
                accessor.Write(FlagOffset, 0);

                OnChangeData?.Invoke(data);
            }
        }

        public void WriteAndSignal(string data)
        {
            using var accessor = _mmf.CreateViewAccessor();
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            accessor.WriteArray(DataOffset, bytes, 0, bytes.Length);
            accessor.Write(FlagOffset, 1);
        }

        public static bool ValidateXml(string xml, out string ErrorMessage)
        {
            try
            {
                XDocument.Parse(xml);
                ErrorMessage = string.Empty;
                return  true ;
            }
            catch (XmlException ex)
            {
                ErrorMessage = ex.Message;
                return false;                
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Неизвестная ошибка: {ex.Message}";
                return false;
            }
        }

        public string ReadData()
        {            
            using var accessor = _mmf.CreateViewAccessor();
            byte[] bytes = new byte[Size];
            accessor.ReadArray(0, bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        }


        public void Dispose()
        {
            if (!_disposed)
            {
                _mmf?.Dispose();
                _disposed = true;
            }
        }
    }
}
