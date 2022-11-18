using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        private Dictionary<object, List<MemberInfo>> pathAsMemberInfoArrayByOrigin = new();

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
        public string[] PathArray => PropertyUtils.LazyLoad(ref pathArray, SplitPathIntoArray, (value) => value == null || value.Length == 0);
        public string LastFieldName => PathArray.Last();

        public static implicit operator string(UnityPath unityPath) => unityPath.Path;
        public static implicit operator string[](UnityPath unityPath) => unityPath.PathArray;

        public List<MemberInfo> GetPathAsMemberInfoList(object startValue)
        {
            if (pathAsMemberInfoArrayByOrigin.ContainsKey(startValue))
                return pathAsMemberInfoArrayByOrigin[startValue];

            List<MemberInfo> fieldInfoPath = new();
            object value = startValue;
            for (int i = 0; i < PathArray.Length; i++)
            {
                MemberInfo[] members = value.GetType().GetMember(PathArray[i], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                if (members.Length == 0)
                {
                    Debug.LogWarning(Path + " " + "(" + PathArray[i] + ")" + " not found.");
                    return null;
                }
                MemberInfo info = members[0];
                fieldInfoPath.Add(info);
                if (i + 1 < PathArray.Length)
                {
                    value = fieldInfoPath[i].GetValue(value);
                }
            }

            pathAsMemberInfoArrayByOrigin[startValue] = fieldInfoPath;
            return fieldInfoPath;
        }

        public void SetValueOfMemberAtPath(object startingValue, object finalValue)
        {
            GetPathAsMemberInfoList(startingValue).SetValue(startingValue, finalValue);
        }

        public object GetValueOfMemberAtPath(object startingValue)
        {
            return GetPathAsMemberInfoList(startingValue).GetFinalValue(startingValue);
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