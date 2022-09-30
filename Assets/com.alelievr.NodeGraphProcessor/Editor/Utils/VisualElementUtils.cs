using UnityEngine.UIElements;

namespace GraphProcessor.Utils
{
    public static class VisualElementUtils
    {
        /// <summary>
        /// Make visible and affects layout.
        /// </summary>
        public static void Show(this VisualElement ve)
        {
            ve.style.display = DisplayStyle.Flex;
        }

        /// <summary>
        /// Make invisible and absent from layout.
        /// </summary>
        public static void Hide(this VisualElement ve)
        {
            ve.style.display = DisplayStyle.None;
        }

        public static bool IsShowing(this VisualElement ve)
        {
            return ve.style.display == DisplayStyle.Flex;
        }

        public static void SetPosition(this VisualElement ve, float top, float bottom, float left, float right)
        {
            ve.style.top = top;
            ve.style.bottom = bottom;
            ve.style.left = left;
            ve.style.right = right;
        }

        public static void SetSize(this VisualElement ve, float width, float height)
        {
            ve.style.width = width;
            ve.style.height = height;
        }

        public static void SetOpacity(this VisualElement ve, float opacity)
        {
            ve.style.opacity = opacity;
        }
    }
}