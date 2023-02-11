using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GraphProcessor
{
    public static class MemberInfoExtension
    {
        public static Type GetUnderlyingType(this MemberInfo member)
        {
            return member.MemberType switch
            {
                MemberTypes.Event => ((EventInfo)member).EventHandlerType,
                MemberTypes.Field => ((FieldInfo)member).FieldType,
                MemberTypes.Method => ((MethodInfo)member).ReturnType,
                MemberTypes.Property => ((PropertyInfo)member).PropertyType,
                _ => throw new ArgumentException
                                    (
                                     "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                                    ),
            };
        }

        public static string GetPath(this IList<MemberInfo> list)
        {
            string path = "";
            for (int i = 0; i < list.Count; i++)
            {
                if (i > 0) path += ".";
                path += list[i].Name;
            }
            return path;
        }

        public static bool HasCustomAttribute<T>(this MemberInfo memberInfo)
        {
            return Attribute.IsDefined(memberInfo, typeof(T));
        }

        public static object GetValue(this MemberInfo memberInfo, object forObject)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).GetValue(forObject);
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetValue(forObject);
                default:
                    throw new NotImplementedException();
            }
        }

        public static void SetValue(this MemberInfo memberInfo, object forObject, object value)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    ((FieldInfo)memberInfo).SetValue(forObject, value);
                    break;
                case MemberTypes.Property:
                    if (((PropertyInfo)memberInfo).GetSetMethod(true) == null) break;
                    ((PropertyInfo)memberInfo).SetValue(forObject, value);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public static bool IsPublic(this MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)memberInfo).IsPublic;
                case MemberTypes.Property:
                    return ((PropertyInfo)memberInfo).GetAccessors().Any(MethodInfo => MethodInfo.IsPublic);
                default:
                    return false;
            }
        }

        public static bool IsField(this MemberInfo memberInfo)
        {
            return memberInfo != null && memberInfo.MemberType == MemberTypes.Field;
        }

        public static object GetFinalValue(this IList<UnityPath.PathMemberInfo> list, object startingValue)
        {
            object currentValue = startingValue;
            for (int i = 0; i < list.Count; i++)
            {
                currentValue = list[i].MemberInfo.GetValue(currentValue);

                if (list[i].ArrayIndex.HasValue)
                {
                    currentValue = (currentValue as IList)[list[i].ArrayIndex.Value];
                }
            }
            return currentValue;
        }

        public static void SetValue(this IList<UnityPath.PathMemberInfo> list, object startingValue, object finalValue)
        {
            object currentValue = startingValue;
            for (int i = 0; i < list.Count; i++)
            {
                if (i + 1 == list.Count)
                {
                    if (list[i].ArrayIndex.HasValue)
                    {
                        currentValue = list[i].MemberInfo.GetValue(currentValue);
                        (currentValue as IList)[list[i].ArrayIndex.Value] = finalValue;
                    }
                    else list[i].MemberInfo.SetValue(currentValue, finalValue);
                    break;
                }

                currentValue = list[i].MemberInfo.GetValue(currentValue);

                if (list[i].ArrayIndex.HasValue)
                {
                    currentValue = (currentValue as IList)[list[i].ArrayIndex.Value];
                }
            }
        }

        public static object GetValueAt(this IList<MemberInfo> list, object startingValue, int index)
        {
            object currentValue = startingValue;
            for (int i = 0; i < list.Count; i++)
            {
                currentValue = list[i].GetValue(currentValue);
                if (i == index) break;
            }
            return currentValue;
        }

        public static bool IsValid(this IList<MemberInfo> list)
        {
            return list.Any(x => x == null);
        }
    }
}