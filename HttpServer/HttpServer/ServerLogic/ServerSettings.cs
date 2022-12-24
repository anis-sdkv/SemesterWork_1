using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HttpServer.ServerLogic
{
    internal class ServerSettings
    {
        public string IP { get; set; }
        public int Port { get; set; }
        public string DataDirectory { get; set; }
        public  string ConnectionString { get; set; }

        public static ServerSettings ReadFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(path);
            var buffer = File.ReadAllBytes(path);
            var settings = JsonSerializer.Deserialize<ServerSettings>(buffer);
            if (settings == null)
                throw new ArgumentException("Не удалось считать файл.");
            return settings;
        }
    }
}
