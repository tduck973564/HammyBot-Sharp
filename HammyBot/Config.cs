using System;
using System.Text.Json;
using System.IO;

namespace HammyBot
{
    public class Config
    {
        public string Token { get; set; }

        public static Config Load(string path)
        {
            string serializedClass = File.ReadAllText(path);
            return JsonSerializer.Deserialize<Config>(serializedClass);
        }

        public void Save(string path)
        {
            string serializedClass = JsonSerializer.Serialize(this);
            File.WriteAllText(path, serializedClass);
        }
    }
}