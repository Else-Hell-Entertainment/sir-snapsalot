using System;

namespace EHE.Global.UI
{
    public class UIEvents
    {
        public event Action PauseGame;

        public event Action UnpauseGame;

        public void RaisePauseGame()
        {
            PauseGame?.Invoke();
        }

        public void RaiseUnpauseGame()
        {
            UnpauseGame?.Invoke();
        }
    }
}
