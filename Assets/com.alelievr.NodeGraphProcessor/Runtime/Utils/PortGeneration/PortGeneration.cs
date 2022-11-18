using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using static GraphProcessor.BaseNode;
using static GraphProcessor.NodeDelegates;

namespace GraphProcessor
{
    public static partial class PortGeneration
    {
        #region Reflection Generation Of Ports

        internal static List<NodeFieldInformation> GetAllPortInformation(object owner, UnityPathFactory proxiedFieldPath = null)
        {
            if (proxiedFieldPath == null)
                proxiedFieldPath = UnityPathFactory.Init();

            List<NodeFieldInformation> nodePortInformation = new();

            Type dataType = owner.GetType();

            PortMemberInfoDataForType portMemberInfoDataForType = GeneratePortMemberInfoDataForType(dataType, owner);
            foreach (var member in portMemberInfoDataForType.MembersWithInputOrOutputAttribute)
            {
                portMemberInfoDataForType.CustomBehaviorInfoByMember.TryGetValue(member, out CustomPortBehaviorDelegateInfo customBehavior);
                nodePortInformation.Add(new NodeFieldInformation(owner, member, customBehavior, proxiedFieldPath.Assemble(member)));
            }

            foreach (var member in portMemberInfoDataForType.MembersWithNestedPortsAttribute)
            {
                nodePortInformation.AddRange(GetAllPortInformation(member.GetValue(owner), proxiedFieldPath.Branch().Append(member)));
            }

            return nodePortInformation;
        }

        internal static PortMemberInfoDataForType GeneratePortMemberInfoDataForType(Type type, object owner)
        {
            MemberInfo[] members = type.GetInstanceFieldsAndProperties();
            IEnumerable<MemberInfo> membersWithInputAttribute = members.Where(x => x.HasCustomAttribute<InputAttribute>());
            IEnumerable<MemberInfo> membersWithOutputAttribute = members.Where(x => x.HasCustomAttribute<OutputAttribute>());
            IEnumerable<MemberInfo> membersWithNestedPortsAttribute = members.Where(x => x.HasCustomAttribute<NestedPortsAttribute>());
            MethodInfo[] methodsWithCustomPortBehavior = type.GetInstanceMethodsByAttribute<CustomPortBehaviorAttribute>();

            Dictionary<MemberInfo, CustomPortBehaviorDelegateInfo> customBehaviorInfoByMember = new();
            foreach (var customPortBehaviorMethod in methodsWithCustomPortBehavior)
            {
                var customPortBehaviorAttribute = customPortBehaviorMethod.GetCustomAttribute<CustomPortBehaviorAttribute>();

                MemberInfo field = membersWithInputAttribute.Concat(membersWithOutputAttribute).FirstOrDefault(x => x.Name == customPortBehaviorAttribute.fieldName);

                if (field == null)
                {
                    // InvalidCustomPortBehaviorFieldNameErrorMessage
                    Debug.LogError($"Invalid field name for custom port behavior: {customPortBehaviorMethod}, {customPortBehaviorAttribute.fieldName}");
                    continue;
                }

                try
                {
                    Delegate deleg = Delegate.CreateDelegate(typeof(CustomPortBehaviorDelegate), owner, customPortBehaviorMethod, true);
                    CustomPortBehaviorDelegateInfo delegInfo = new(deleg as CustomPortBehaviorDelegate, customPortBehaviorAttribute.cloneResults);
                    customBehaviorInfoByMember.Add(field, delegInfo);
                }
                catch
                {
                    // InvalidCustomPortBehaviorSignatureErrorMessage
                    Debug.LogError($"The function {customPortBehaviorMethod} cannot be converted to the required delegate format: {typeof(CustomPortBehaviorDelegate)}");
                    continue;
                }
            }
            return new PortMemberInfoDataForType(type, membersWithInputAttribute, membersWithOutputAttribute, membersWithNestedPortsAttribute, customBehaviorInfoByMember);
        }
        #endregion
    }
}