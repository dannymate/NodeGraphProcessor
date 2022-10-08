using System;
using System.Linq;
using System.Collections.Generic;

namespace GraphProcessor
{
    public static class PropertyUtils
    {
        public static T LazyLoad<T>(ref T backingField, Func<T> getter)
        {
            if (backingField == null)
                backingField = getter();

            return backingField;
        }

        public static T LazyLoad<T>(ref T backingField, Func<T> getter, Predicate<T> getterPredicate)
        {
            if (getterPredicate.Invoke(backingField))
                backingField = getter();

            return backingField;
        }
    }
}