// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using System;

namespace EHE.SirSnapsalot.UI
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
