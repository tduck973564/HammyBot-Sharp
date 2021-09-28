using System;
using System.Reflection;

#nullable enable
namespace HammyBot.GuildStorage
{
    public class TypeArgumentException : Exception
    {
        public TypeArgumentException()
        {
        }
        public TypeArgumentException(string message)
            : base(message)
        {
        }
        public TypeArgumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    public class GuildStorage : JsonConfigMethods
    {
        
    }
}