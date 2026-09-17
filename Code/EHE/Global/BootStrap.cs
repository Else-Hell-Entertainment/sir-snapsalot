using EHE.Global.Config;
using EHE.Global.Managers;
using Godot;

namespace EHE.Global.SceneManagement
{
    /// <summary>
    /// First scene to load when game starts to allow autoloads initialize and then transition to the main menu.
    /// Important note: Scene manager is not yet initialized when this scene is loaded, so you cannot use it here and
    /// you need to manually remove the bootstrap scene.
    /// Splash screens or intro movies can be played here if required, and it's possible to do other setup tasks.
    /// </summary>
    public partial class BootStrap : Node
    {
        [Export]
        private AnimationPlayer _splashAnimationPlayer;

        private SceneTransition _mainMenuTransition;

        public override void _Ready()
        {
            _splashAnimationPlayer.AnimationFinished += OnSplashAnimationFinished;
            PlayGameSplash();
        }

        private void OnSplashAnimationFinished(StringName animationName)
        {
            _splashAnimationPlayer.AnimationFinished -= OnSplashAnimationFinished;
            _mainMenuTransition = new SceneTransition(GlobalScenePaths.Menu.TitleScreen);
            SceneManager.Instance.RequestSceneTransition(_mainMenuTransition);
            QueueFree();
        }

        private void PlayGameSplash()
        {
            GD.Print("Playing an awesome intro movie.");
            _splashAnimationPlayer.Play();
        }
    }
}
