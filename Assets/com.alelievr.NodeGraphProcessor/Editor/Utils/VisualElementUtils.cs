using System;
using UnityEngine.UIElements;

namespace GraphProcessor.Utils
{
    public static class VisualElementUtils
    {
        /// <summary>
        /// Make visible and affects layout.
        /// </summary>
        public static T Show<T>(this T ve) where T : VisualElement
        {
            ve.style.display = DisplayStyle.Flex;
            return ve;
        }

        /// <summary>
        /// Make invisible and absent from layout.
        /// </summary>
        public static T Hide<T>(this T ve) where T : VisualElement
        {
            ve.style.display = DisplayStyle.None;
            return ve;
        }

        public static bool IsShowing(this VisualElement ve)
        {
            return ve.style.display == DisplayStyle.Flex;
        }

        public static T SetOffset<T>(this T ve, float top, float bottom, float left, float right) where T : VisualElement
        {
            ve.style.top = top;
            ve.style.bottom = bottom;
            ve.style.left = left;
            ve.style.right = right;
            return ve;
        }

        public static T SetSize<T>(this T ve, float width, float height) where T : VisualElement
        {
            ve.style.width = width;
            ve.style.height = height;
            return ve;
        }

        public static T SetOpacity<T>(this T ve, float opacity) where T : VisualElement
        {
            ve.style.opacity = opacity;
            return ve;
        }

        public static T SetMargin<T>(this T ve, float top, float bottom, float left, float right) where T : VisualElement
        {
            ve.style.marginTop = top;
            ve.style.marginBottom = bottom;
            ve.style.marginLeft = left;
            ve.style.marginRight = right;
            return ve;
        }

        public static T SetAlignment<T>(this T ve, Align alignment) where T : VisualElement
        {
            ve.style.alignSelf = alignment;
            return ve;
        }

        public static T SetPosition<T>(this T ve, Position position) where T : VisualElement
        {
            ve.style.position = position;
            return ve;
        }
    }
}