using System;
using UnityEngine;

namespace GraphProcessor
{
    public static class Property
    {
        public static T Getter<T>(ref T backingField, Action actionIfNull = null) where T : new()
        {
            if (backingField == null)
            {
                backingField = new();
                actionIfNull?.Invoke();
            }
            return backingField;
        }
    }
}
