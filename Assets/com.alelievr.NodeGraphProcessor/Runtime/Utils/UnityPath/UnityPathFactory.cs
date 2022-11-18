using System.Reflection;

namespace GraphProcessor
{
    /// <summary>
    /// Simplifies to construction of UnityPath.
    /// To get start use the static Init() method.
    /// </summary>
    public class UnityPathFactory
    {
        private string path;

        private UnityPathFactory()
        {
            this.path = string.Empty;
        }

        private UnityPathFactory(string path)
        {
            this.path = path;
        }

        public static UnityPathFactory Init() => new();

        private static string Append(string basePath, string value)
            => $"{basePath}{(string.IsNullOrEmpty(basePath) ? string.Empty : '.')}{value}";

        public UnityPathFactory Append(string value)
        {
            path = Append(path, value);
            return this;
        }

        public UnityPathFactory Append(MemberInfo value)
        {
            path = Append(path, value.Name);
            return this;
        }

        /// <summary>
        /// Returns UnityPath with value appended to path.
        /// This method does not affect the factory path.
        /// </summary>
        public UnityPath Assemble(string value) => new(Append(path, value));

        /// <summary>
        /// Returns UnityPath with value appended to path.
        /// This method does not affect the factory path.
        /// </summary>
        public UnityPath Assemble(MemberInfo value) => new(Append(path, value.Name));

        /// <summary>
        /// Effectively clones the factory.
        /// Use when multiple children require the same base path.
        /// </summary>
        public UnityPathFactory Branch() => new(path);
    }
}