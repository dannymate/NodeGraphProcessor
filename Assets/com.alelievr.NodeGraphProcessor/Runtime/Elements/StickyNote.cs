using System;
using UnityEngine;

namespace GraphProcessor
{
    /// <summary>
    /// Serializable Sticky node class
    /// </summary>
    [Serializable]
    public class StickyNote
    {
        public readonly static Vector2 DefaultStickyNoteSize = new(200, 300);

        public Rect position;
        public string title = "Hello World!";
        public string contents = "Description";

        public StickyNote(string title, Vector2 position)
        {
            this.title = title;
            this.position = new Rect(position.x, position.y, DefaultStickyNoteSize.x, DefaultStickyNoteSize.y);
        }
    }
}