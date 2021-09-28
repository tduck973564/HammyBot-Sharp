using System;
using System.Text.Json;
using System.IO;

namespace HammyBot
{
    public class Config: JsonConfigMethods
    {
        public string Token { get; set; }
    }
}