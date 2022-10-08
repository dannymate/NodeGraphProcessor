using UnityEngine;

namespace GraphProcessor
{
    [CreateAssetMenu(fileName = "PortIdentifier", menuName = "NGP/Port Identifier")]
    public class PortIdentifier : ScriptableObject
    {
        [SerializeField]
        private string identifier;

        public string Identifier => identifier;

        public static implicit operator string(PortIdentifier portIdentifier)
        {
            return portIdentifier.Identifier;
        }
    }
}