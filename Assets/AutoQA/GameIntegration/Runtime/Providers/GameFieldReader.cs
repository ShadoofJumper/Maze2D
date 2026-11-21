using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace AutoQA.Maze2D
{
    /// <summary>Read-only, cached access to known game fields without adding QA accessors to game classes.</summary>
    internal static class GameFieldReader
    {
        static readonly Dictionary<(Type, string), FieldInfo> Fields = new Dictionary<(Type, string), FieldInfo>();
        public static T Read<T>(object owner, string name)
        {
            if (owner == null) throw new ArgumentNullException(nameof(owner));
            var key = (owner.GetType(), name);
            if (!Fields.TryGetValue(key, out var field))
            {
                for (var type = key.Item1; type != null && field == null; type = type.BaseType)
                    field = type.GetField(name, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (field == null) throw new MissingFieldException(key.Item1.FullName, name);
                if (!typeof(T).IsAssignableFrom(field.FieldType)) throw new InvalidOperationException("Unexpected field type: " + name);
                Fields.Add(key, field);
            }
            return (T)field.GetValue(owner);
        }
    }
}
