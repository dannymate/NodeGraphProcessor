using UnityEngine;

namespace GraphProcessor
{
    [System.Serializable]
    public struct SubGraphOptions
    {
        public const string DisplayNameFieldName = nameof(displayName);
        public const string IsRenamableFieldName = nameof(isRenamable);

        [SerializeField]
        private string displayName;

        [SerializeField]
        private bool isRenamable;

        public string DisplayName => displayName;
        public bool IsRenamable => isRenamable;
    }
}