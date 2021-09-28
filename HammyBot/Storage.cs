using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

#nullable enable
namespace HammyBot
{
    public class Storage
    {
        public Dictionary<int, GuildConfig>? GuildDict;
        private string? _folder;

        public static Storage Load(string folder)
        {
            Storage output = new Storage();

            output._folder = folder;
            output.GuildDict = new Dictionary<int, GuildConfig>();

            foreach (string file in Directory.GetFiles(folder))
            {
                string serializedClass = File.ReadAllText(file);
                GuildConfig? deserialisedClass = JsonSerializer.Deserialize<GuildConfig>(serializedClass);
                
                if (deserialisedClass == null) throw new NullReferenceException("Deserialized class was Null.");
                
                output.GuildDict.Add(int.Parse(file), deserialisedClass);
            }

            return output;
        }

        public void Save()
        {
            string previousDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(_folder!);
            
            foreach (KeyValuePair<int, GuildConfig> item in GuildDict!)
            {
                //string serializedClass = File.ReadAllText(item.Key.ToString());
                File.WriteAllText(item.Key.ToString(), JsonSerializer.Serialize(item.Value));
            }
            
            Directory.SetCurrentDirectory(previousDirectory);
        }

        public GuildConfig Get(int guildId)
        {
            CreateIfNotExists(guildId);
            return GuildDict![guildId];
        }

        public void Set(int guildId, GuildConfig guildConfig)
        {
            GuildConfig? _;
            if (!GuildDict!.TryGetValue(guildId, out _))
                Program.Logger.Warn("Set GuildConfig that does not exist.");
                    
            GuildDict[guildId] = guildConfig;
        }

        private void CreateIfNotExists(int guildId)
        {
            GuildConfig? _;
            if (!GuildDict!.TryGetValue(guildId, out _))
                GuildDict.Add(guildId, new GuildConfig());
        }
    }

    public class GuildConfig : JsonConfigMethods
    {
        public int Muterole;
    }
}