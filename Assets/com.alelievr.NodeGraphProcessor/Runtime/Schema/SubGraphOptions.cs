using UnityEngine;

namespace GraphProcessor
{
    [System.Serializable]
    public struct SubGraphOptions
    {
        public const string DisplayNameFieldName = nameof(displayName);
        public const string RenameOptionsFieldName = nameof(renameOptions);

        [SerializeField]
        private string displayName;

        [SerializeField]
        private NodeRenameOptions renameOptions;

        public string DisplayName => displayName;
        public NodeRenameOptions RenameOptions => renameOptions;
    }
}