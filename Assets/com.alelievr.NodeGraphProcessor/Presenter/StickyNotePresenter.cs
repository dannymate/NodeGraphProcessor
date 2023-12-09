using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace GraphProcessor
{
    public class StickyNotePresenter
    {
        private StickyNoteView _view;
        private StickyNote _model;

        public event Action<StickyNotePresenter, StickyNoteView, StickyNote> RemovedRequested;

        public StickyNotePresenter(GraphPresenter parent, StickyNote model, StickyNoteView view)
        {
            _model = model;
            _view = view;

            Initialize();
        }

        private void Initialize()
        {
            _view.PositionChanged += OnPositionChanged;

            _view.Q<TextField>("title-field").RegisterCallback<ChangeEvent<string>>(e =>
            {
                _model.title = e.newValue;
            });

            _view.Q<TextField>("contents-field").RegisterCallback<ChangeEvent<string>>(e =>
            {
                _model.contents = e.newValue;
            });

            _view.RegisterCallback<DetachFromPanelEvent>(e =>
            {
                Remove();
            });
        }

        public void Remove()
        {
            RemovedRequested?.Invoke(this, _view, _model);
        }

        private void OnPositionChanged(Rect position)
        {
            _model.position = position;
        }
    }
}