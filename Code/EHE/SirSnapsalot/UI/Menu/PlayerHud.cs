using EHE.Common.Godot.Logging;
using EHE.Global.Managers;
using EHE.SirSnapsalot.GlobalPlayer;
using Godot;

namespace EHE.SirSnapsalot.UI.PlayerUI
{
    public partial class PlayerHud : Control
    {
        public override void _EnterTree()
        {
            SubscribeToEvents();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeFromEvents();
        }

        private void SubscribeToEvents()
        {
            this.LogDebug("Subscribing to player events.");
        }

        private void UnsubscribeFromEvents()
        {
            this.LogDebug("Unsubscribing from player events.");
        }
    }
}
