namespace EHE.Global.FSM
{
    public interface IState
    {
        bool IsAdditive => false;

        void Execute() { }

        void Enter();
        void Exit(bool keepLoaded = false);
    }
}
