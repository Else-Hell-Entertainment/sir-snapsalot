using System;
using EHE.Global.Config;
using Godot;

namespace EHE.UI.Menu
{
    public abstract partial class MenuScene : Control
    {
        public event Action<string> RequestTransitionEvent;

        public event Action ReturnToPreviousEvent;

        public override void _GuiInput(InputEvent @event)
        {
            if (@event.IsActionPressed(InputConfig.ESCAPE))
            {
                ReturnToPrevious();
            }
        }

        protected void RequestTransition(string newState)
        {
            RequestTransitionEvent?.Invoke(newState);
        }

        protected void ReturnToPrevious()
        {
            ReturnToPreviousEvent?.Invoke();
        }
    }
}
