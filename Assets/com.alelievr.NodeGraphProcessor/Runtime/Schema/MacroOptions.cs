using UnityEngine;

namespace GraphProcessor
{
    [System.Serializable]
    public struct MacroOptions
    {
        [SerializeField]
        private string menuLocation;

        public string MenuLocation => menuLocation;
    }
}