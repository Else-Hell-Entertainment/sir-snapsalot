using EHE.Global.GlobalPlayer;
using EHE.Global.Logging;
using EHE.Global.Managers;
using Godot;

namespace EHE.UI.PlayerUI
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
