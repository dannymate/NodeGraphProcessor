using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static GraphProcessor.NodeDelegates;

namespace GraphProcessor
{
    public static partial class PortGeneration
    {
        public struct PortMemberInfoDataForType
        {
            private readonly Type type;
            private readonly IEnumerable<MemberInfo> membersWithInputAttribute;
            private readonly IEnumerable<MemberInfo> membersWithOutputAttribute;
            private readonly IEnumerable<MemberInfo> membersWithNestedPortsAttribute;
            private readonly Dictionary<MemberInfo, CustomPortBehaviorDelegateInfo> customBehaviorInfoByMember;

            public PortMemberInfoDataForType(Type type, IEnumerable<MemberInfo> membersWithInputAttribute, IEnumerable<MemberInfo> membersWithOutputAttribute, IEnumerable<MemberInfo> membersWithNestedPortsAttribute, Dictionary<MemberInfo, CustomPortBehaviorDelegateInfo> customBehaviorInfoByMember)
            {
                this.type = type;
                this.membersWithInputAttribute = membersWithInputAttribute;
                this.membersWithOutputAttribute = membersWithOutputAttribute;
                this.membersWithNestedPortsAttribute = membersWithNestedPortsAttribute;
                this.customBehaviorInfoByMember = customBehaviorInfoByMember;
            }

            public Type Type => type;
            public IEnumerable<MemberInfo> MembersWithInputAttribute => membersWithInputAttribute;
            public IEnumerable<MemberInfo> MembersWithOutputAttribute => membersWithOutputAttribute;
            public IEnumerable<MemberInfo> MembersWithInputOrOutputAttribute => membersWithInputAttribute.Concat(membersWithOutputAttribute);
            public IEnumerable<MemberInfo> MembersWithNestedPortsAttribute => membersWithNestedPortsAttribute;
            public Dictionary<MemberInfo, CustomPortBehaviorDelegateInfo> CustomBehaviorInfoByMember => customBehaviorInfoByMember;
        }
    }
}