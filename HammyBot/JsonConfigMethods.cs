using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace HammyBot
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
    public class JsonConfigMethods
    {
        public static T Load<T>(string path)
        {
            string serializedClass = File.ReadAllText(path);
            T? output = JsonSerializer.Deserialize<T>(serializedClass);
            if (output == null) throw new NullReferenceException("Deserialized class was Null.");
            return output;
        }

        public void Save(string path)
        {
            string serializedClass = JsonSerializer.Serialize(this);
            File.WriteAllText(path, serializedClass);
        }
        
        public object? Get(string property)
        {
            return GetType().GetProperty(property)?.GetValue(this, null);
        }

        public T? Get<T>(string property)
        {
            return (T?)GetType().GetProperty(property)?.GetValue(this, null);
        }
        
        public void Set<T>(string property, T value)
        {
            PropertyInfo? propertyInfo = GetType().GetProperty(property);
            if (Get(property)?.GetType() != propertyInfo?.GetType())
            {
                throw new TypeArgumentException($"The type provided in the setter was not equal to the type in the config.");
            }

            if (propertyInfo?.GetType() == null)
            {
                throw new NullReferenceException("The provided property does not exist on the class.");
            }
            propertyInfo.SetValue(this, value);
        }
    }
}