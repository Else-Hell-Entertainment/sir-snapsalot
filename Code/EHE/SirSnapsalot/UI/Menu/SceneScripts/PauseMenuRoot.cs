using EHE.Global.SceneManagement;

namespace EHE.UI.Menu
{
    public partial class PauseMenuRoot : MenuRoot
    {
        protected override void Initialize()
        {
            AddState(SceneCollections.MenuScenes.Keys.Pause);
            AddState(SceneCollections.MenuScenes.Keys.Settings, true, true);
            AddState(SceneCollections.MenuScenes.Keys.AudioSettings, true, true);
            AddState(SceneCollections.MenuScenes.Keys.VideoSettings, true, true);
            RequestTransition(SceneCollections.MenuScenes.Keys.Pause);
        }
    }
}
