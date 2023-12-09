#if UNITY_2020_1_OR_NEWER
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using System;

namespace GraphProcessor
{
    public class StickyNoteView : UnityEditor.Experimental.GraphView.StickyNote
    {
        public event Action<Rect> PositionChanged;

        protected StickyNoteView()
        {
            fontSize = StickyNoteFontSize.Small;
            theme = StickyNoteTheme.Classic;
        }

        public static StickyNoteView Instantiate(string title, string contents, Rect position)
        {
            StickyNoteView view = new()
            {
                title = title,
                contents = contents
            };
            view.SetPosition(position);
            return view;
        }

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);

            PositionChanged?.Invoke(newPos);
        }

        public override void OnResized()
        {
            PositionChanged?.Invoke(layout);
        }
    }
}
#endif