using System;
using UnityEngine;

namespace GraphProcessor.EdgeProcessing
{
    [Serializable]
    public class EdgeProcessOrderKey : IEquatable<string>, IEquatable<EdgeProcessOrderKey>
    {
        public const string ValueFieldName = nameof(_value);

        [SerializeField, HideInInspector]
        private string _value;
        public string Value => _value;

        public EdgeProcessOrderKey(string key)
        {
            this._value = key;
        }

        public static implicit operator string(EdgeProcessOrderKey edgeProcessOrderKey) => edgeProcessOrderKey.Value;
        public static implicit operator EdgeProcessOrderKey(string key) => new(key);

        public bool Equals(string other) => string.Equals(_value, other);
        public bool Equals(EdgeProcessOrderKey other) => Value == other.Value;
        public override bool Equals(object obj) => base.Equals(obj);
        public override int GetHashCode() => Value != null ? Value.GetHashCode() : 0;
        public override string ToString() => Value;
        public static bool operator ==(EdgeProcessOrderKey lhs, EdgeProcessOrderKey rhs) => lhs.Equals(rhs);
        public static bool operator !=(EdgeProcessOrderKey lhs, EdgeProcessOrderKey rhs) => !lhs.Equals(rhs);


    }
}