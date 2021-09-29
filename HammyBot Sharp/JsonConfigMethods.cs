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
using System.IO;
using System.Text.Json;

namespace HammyBot_Sharp
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
            var output = JsonSerializer.Deserialize<T>(serializedClass);
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
            return (T?) GetType().GetProperty(property)?.GetValue(this, null);
        }

        public void Set<T>(string property, T value)
        {
            var propertyInfo = GetType().GetProperty(property);
            if (Get(property)?.GetType() != propertyInfo?.GetType())
                throw new TypeArgumentException(
                    "The type provided in the setter was not equal to the type in the config.");

            if (propertyInfo?.GetType() == null)
                throw new NullReferenceException("The provided property does not exist on the class.");
            propertyInfo.SetValue(this, value);
        }
    }
}