using System.Text.RegularExpressions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TypeReferences;
using UnityEditor.Graphs;
using UnityEngine;

namespace GraphProcessor
{
    [Serializable]
    public class UnityPath
    {
        [SerializeField]
        private string path;
        [SerializeField]
        private string[] pathArray;
        private Dictionary<object, Info> pathAsMemberInfoArrayByOrigin = new();

        public class Info
        {
            public Info(UnityPath origin, string path, string[] pathArray, List<PathMemberInfo> pathAsMemberInfo, Type type = null, Nullable<bool> isCollection = null)
            {
                Origin = origin;
                Path = path;
                PathArray = pathArray;
                PathAsMemberInfo = pathAsMemberInfo;
                DisplayType = type ?? MemberInfo.GetUnderlyingType();
                IsCollection = isCollection ?? DisplayType.IsCollection();
            }

            public UnityPath Origin { get; private set; }
            public string Path { get; private set; }
            public string[] PathArray { get; private set; }
            public List<PathMemberInfo> PathAsMemberInfo { get; private set; }
            public MemberInfo MemberInfo => PathAsMemberInfo.Last().MemberInfo;
            public bool IsCollection { get; private set; }
            public Type DisplayType { get; private set; }
            public string FieldName => MemberInfo.Name;
        }

        public struct PathMemberInfo
        {
            public PathMemberInfo(MemberInfo memberInfo, int? arrayIndex = null)
            {
                MemberInfo = memberInfo;
                ArrayIndex = arrayIndex;
            }

            public MemberInfo MemberInfo { get; private set; }
            public int? ArrayIndex { get; private set; }
        }

        public UnityPath(string path)
        {
            this.path = path;
            this.pathArray = null;
        }

        public UnityPath(MemberInfo member)
        {
            this.path = member.Name;
            this.pathArray = null;
        }

        public UnityPath(IEnumerable<string> pathArray)
        {
            this.path = null;
            this.pathArray = pathArray.ToArray();
        }

        public UnityPath(IEnumerable<MemberInfo> members)
        {
            this.path = null;
            this.pathArray = members.Select(x => x.Name).ToArray();
        }

        public string Path => PropertyUtils.LazyLoad(ref path, BuildPathFromArray, (value) => string.IsNullOrEmpty(value));
        private string ReflectedPath => Regex.Replace(Path, @"\.Array\.data\[(\d+)\]", ".$1");
        public string[] PathArray => PropertyUtils.LazyLoad(ref pathArray, SplitPathIntoArray, (value) => value == null || value.Length == 0);
        public string[] ReflectedPathArray => ReflectedPath.Split('.');

        public static implicit operator string(UnityPath unityPath) => unityPath.Path;
        public static implicit operator string[](UnityPath unityPath) => unityPath.PathArray;

        public Info GatherInfo(object startValue)
        {
            if (pathAsMemberInfoArrayByOrigin.ContainsKey(startValue))
                return pathAsMemberInfoArrayByOrigin[startValue];

            bool? isCollection = false;
            Type displayType = null;

            List<PathMemberInfo> fieldInfoPath = new();
            object value = startValue;
            for (int i = 0; i < ReflectedPathArray.Length; i++)
            {
                // ReflectedPathArray uses ints after collection is signify an entry so we check if the next entry is an int
                if (isCollection.Value == true && int.TryParse(ReflectedPathArray[i], out int index))
                {
                    displayType = value.GetType().GetElementType();
                    isCollection = false;
                    if (i + 1 < ReflectedPathArray.Length)
                    {
                        value = (value as IList)[index];
                    }
                    fieldInfoPath[i - 1] = new PathMemberInfo(fieldInfoPath[i - 1].MemberInfo, index);
                    continue;
                }

                MemberInfo[] members = value.GetType().GetMember(ReflectedPathArray[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

                if (members.Length == 0)
                {
                    Debug.LogWarning($"{ReflectedPath} ({ReflectedPathArray[i]}) not found.");
                    return null;
                }

                MemberInfo member = members[0];
                displayType = member.GetUnderlyingType();
                isCollection = displayType.IsCollection();
                fieldInfoPath.Add(new PathMemberInfo(member));
                // We won't use the value if this is the last cycle so we skip to save performance
                if (i + 1 < ReflectedPathArray.Length)
                {
                    value = fieldInfoPath[i].MemberInfo.GetValue(value);
                }
            }

            Info info = new(this, Path, PathArray, fieldInfoPath, displayType, isCollection);
            pathAsMemberInfoArrayByOrigin[startValue] = info;
            return info;
        }

        public void SetValueOfMemberAtPath(object startingValue, object finalValue)
        {
            GatherInfo(startingValue).PathAsMemberInfo.SetValue(startingValue, finalValue);
        }

        public object GetValueOfMemberAtPath(object startingValue)
        {
            return GatherInfo(startingValue).PathAsMemberInfo.GetFinalValue(startingValue);
        }

        private string[] SplitPathIntoArray()
        {
            string[] paths = path.Split('.');
            return paths;
        }

        private string BuildPathFromArray()
        {
            string path = "";
            foreach (string fragment in pathArray)
            {
                path = string.Concat(path, fragment);
                path += ".";
            }
            return path;
        }
    }
}