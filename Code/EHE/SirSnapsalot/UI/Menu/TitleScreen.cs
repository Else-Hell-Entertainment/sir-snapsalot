using System.Threading.Tasks;
using EHE.SirSnapsalot.Managers;
using EHE.SirSnapsalot.SceneManagement;
using Godot;

namespace EHE.SirSnapsalot.UI
{
    public partial class TitleScreen : Node, ILoadableScene
    {
        public string SceneId { get; } = "TitleScreen";

        public async Task LoadScene()
        {
            GameManager.Instance.UIManager.OpenTitleMenu();
        }

        public async Task UnloadScene() { }

        public void ReceiveSceneTransitionPayload(SceneTransitionPayload payload) { }
    }
}
