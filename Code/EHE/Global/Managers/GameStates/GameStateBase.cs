using EHE.Global.FSM;
using EHE.Global.Logging;
using EHE.Global.Managers;
using Godot;

namespace EHE.Global.GameStates
{
    public partial class GameStateBase : Node, IState
    {
        public virtual void Enter()
        {
            GameManager.Instance.AddChild(this);
            this.LogDebug($"Entering {Name} state!");
        }

        public virtual void Exit(bool keepLoaded = false)
        {
            GameManager.Instance.RemoveChild(this);
            this.LogDebug($"Exiting {Name} state!");
        }
    }
}
