using System;
using System.Linq;

namespace GraphProcessor
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Check against a set of enum options for a given value
        /// </summary>
        /// <typeparam name="T">Enum Type</typeparam>
        /// <param name="value">The enum value to check for</param>
        /// <param name="options">params list of options to check against</param>
        /// <returns>True if any option is equal to value"</returns>
        public static bool IsAny<T>(this T value, params T[] options) where T : Enum
        {
            return options.Contains(value);
        }

        public static bool Is<T>(this T value, T option) where T : Enum
        {
            return value.Equals(option);
        }
    }
}