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
    }
}