using UnityEngine;

namespace GraphProcessor
{
    [System.Serializable]
    public struct SubGraphOptions
    {
        public const string DisplayNameFieldName = nameof(displayName);
        public const string RenamePolicyFieldName = nameof(renamePolicy);

        [SerializeField]
        private string displayName;

        [SerializeField]
        private NodeRenamePolicy renamePolicy;

        public string DisplayName => displayName;
        public NodeRenamePolicy RenamePolicy => renamePolicy;
    }
}