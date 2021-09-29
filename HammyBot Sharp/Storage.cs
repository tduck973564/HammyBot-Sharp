// HammyBot Sharp - HammyBot Sharp
//     Copyright (C) 2021 Thomas Duckworth <tduck973564@gmail.com>
// 
//     This program is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     This program is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with this program.  If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

#nullable enable
namespace HammyBot_Sharp
{
    public class Storage
    {
        private string? _folder;
        public Dictionary<int, GuildConfig>? GuildDict;

        public static Storage Load(string folder)
        {
            Storage output = new();

            output._folder = folder;
            output.GuildDict = new Dictionary<int, GuildConfig>();

            foreach (string file in Directory.GetFiles(folder))
            {
                string serializedClass = File.ReadAllText(file);
                var deserialisedClass = JsonSerializer.Deserialize<GuildConfig>(serializedClass);

                if (deserialisedClass == null) throw new NullReferenceException("Deserialized class was Null.");

                output.GuildDict.Add(int.Parse(file), deserialisedClass);
            }

            return output;
        }

        public void Save()
        {
            string previousDirectory = Directory.GetCurrentDirectory();

            Directory.SetCurrentDirectory(_folder!);

            foreach (var item in GuildDict!)
                //string serializedClass = File.ReadAllText(item.Key.ToString());
                File.WriteAllText(item.Key.ToString(), JsonSerializer.Serialize(item.Value));

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