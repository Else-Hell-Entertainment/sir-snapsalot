using System.Threading.Tasks;
using EHE.Global.Managers;
using EHE.Global.SceneManagement;
using Godot;

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
