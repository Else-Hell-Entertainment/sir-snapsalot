// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

namespace EHE.SirSnapsalot.FSM
{
    public interface IState
    {
        bool IsAdditive => false;

        void Execute() { }

        void Enter();
        void Exit(bool keepLoaded = false);
    }
}
