using UnityEngine.UIElements;

namespace GraphProcessor.Utils
{
    public static class VisualElementUtils
    {
        /// <summary>
        /// Make visible and affects layout.
        /// </summary>
        public static VisualElement Show(this VisualElement ve)
        {
            ve.style.display = DisplayStyle.Flex;
            return ve;
        }

        /// <summary>
        /// Make invisible and absent from layout.
        /// </summary>
        public static VisualElement Hide(this VisualElement ve)
        {
            ve.style.display = DisplayStyle.None;
            return ve;
        }

        public static bool IsShowing(this VisualElement ve)
        {
            return ve.style.display == DisplayStyle.Flex;
        }

        public static VisualElement SetOffset(this VisualElement ve, float top, float bottom, float left, float right)
        {
            ve.style.top = top;
            ve.style.bottom = bottom;
            ve.style.left = left;
            ve.style.right = right;
            return ve;
        }

        public static VisualElement SetSize(this VisualElement ve, float width, float height)
        {
            ve.style.width = width;
            ve.style.height = height;
            return ve;
        }

        public static VisualElement SetOpacity(this VisualElement ve, float opacity)
        {
            ve.style.opacity = opacity;
            return ve;
        }

        public static VisualElement SetMargin(this VisualElement ve, float top, float bottom, float left, float right)
        {
            ve.style.marginTop = top;
            ve.style.marginBottom = bottom;
            ve.style.marginLeft = left;
            ve.style.marginRight = right;
            return ve;
        }

        public static VisualElement SetAlignment(this VisualElement ve, Align alignment)
        {
            ve.style.alignSelf = alignment;
            return ve;
        }
    }
}