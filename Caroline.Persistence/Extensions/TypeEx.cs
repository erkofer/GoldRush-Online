using System;
using System.Collections.Generic;
using System.Reflection;
using JetBrains.Annotations;

namespace Caroline.Persistence.Extensions
{
    public static class TypeEx
    {
        /// <summary>
        /// Returns all properties that are defined within the inheritance branch segment of baseClass to derivedClass, excluding derivedClass
        /// </summary>
        /// <param name="derivedClass">The derivedClass object. All properties exclusively defined below derivedClass are returned.</param>
        /// <param name="propertyType">Only include properties of this type. Null to return properties of any type.</param>
        public static List<PropertyInfo> GetProperties([NotNull] this Type derivedClass, [CanBeNull] Type propertyType = null)
        {
            var selected = new List<PropertyInfo>();
            var currentType = derivedClass;
            var allowedTypes = new HashSet<Type>();

            while (currentType != null)
            {
                allowedTypes.Add(currentType);
                currentType = currentType.BaseType;
            }

            var properties = derivedClass.GetProperties();
            for (var i = 0; i < properties.Length; i++)
            {
                var info = properties[i];
                if (propertyType != null && propertyType != info.PropertyType)
                    continue;
                if (info.ReflectedType == null || !allowedTypes.Contains(info.ReflectedType))
                    continue;
                selected.Add(info);
            }

            return selected;
        }
    }
}
