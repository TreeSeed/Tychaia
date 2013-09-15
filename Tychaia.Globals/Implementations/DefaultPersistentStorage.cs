// ====================================================================== //
// This source code is licensed in accordance with the licensing outlined //
// on the main Tychaia website (www.tychaia.com).  Changes to the         //
// license on the website apply retroactively.                            //
// ====================================================================== //
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Tychaia.Globals
{
    public class DefaultPersistentStorage : IPersistentStorage
    {
        private FileStream m_Stream;
        private StreamReader m_Reader;
        private StreamWriter m_Writer;

        public dynamic Settings
        {
            get;
            private set;
        }

        public DirectoryInfo SaveDirectory
        {
            get { return new DirectoryInfo(this.GetSavePath()); }
        }

        public DefaultPersistentStorage()
        {
            this.m_Stream = new FileStream(this.GetSettingsPath(), FileMode.OpenOrCreate);
            this.m_Stream.Lock(0, this.m_Stream.Length);
            this.m_Reader = new StreamReader(this.m_Stream);
            this.m_Writer = new StreamWriter(this.m_Stream);

            // Load existing settings.
            this.Settings = JsonConvert.DeserializeObject<DynamicDictionary>(
                this.m_Reader.ReadToEnd(),
                new ExpandoObjectConverter());
            if (this.Settings == null)
                this.Settings = new DynamicDictionary();

            ((INotifyPropertyChanged)this.Settings).PropertyChanged += (sender, e) =>
            {
                // Save the ExpandoObject to disk.
                this.m_Stream.Seek(0, SeekOrigin.Begin);
                this.m_Stream.SetLength(0);
                this.m_Writer.Write(JsonConvert.SerializeObject(this.Settings));
                this.m_Writer.Flush();
            };
        }

        private string GetBasePath()
        {
            // Look under %appdata%/.tychaia.
            var appdata = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var path = Path.Combine(appdata, ".tychaia");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }

        private string GetSettingsPath()
        {
            // Look under %appdata%/.tychaia/settings.
            return Path.Combine(this.GetBasePath(), "settings");
        }

        private string GetSavePath()
        {
            // Look under %appdata%/.tychaia/saves.
            var path = Path.Combine(this.GetBasePath(), "saves");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}

