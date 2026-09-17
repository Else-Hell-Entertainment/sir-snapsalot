using EHE.Global.Managers;
using EHE.Global.SceneManagement;
using StateKeys = EHE.Global.SceneManagement.SceneCollections.MenuScenes.Keys;

namespace EHE.UI.Menu
{
    public partial class MainMenuRoot : MenuRoot
    {
        protected override void Initialize()
        {
            AddState(StateKeys.MainMenu);
            AddState(StateKeys.Settings, true, true);
            AddState(StateKeys.AudioSettings, true, true);
            AddState(StateKeys.VideoSettings, true, true);
            AddState(StateKeys.GameplaySettings, true);
            AddState(StateKeys.ControlSettings, true, true);

            GameManager.Instance.RequestStateTransition(GameState.TitleScreen);
            RequestTransition(SceneCollections.MenuScenes.Keys.MainMenu);
        }
    }
}
