using System.Threading.Tasks;
using EHE.Global.SaveSystem;
using Godot;

namespace EHE.Global.SceneManagement
{
    public interface ILoadableScene
    {
        /// <summary>
        /// Actions to perform after the scene has been loaded and added into the scene tree. This is where you can
        /// initialize the scene, spawn the player and set up any necessary states as it's called after _Ready is finished.
        /// </summary>
        /// <returns></returns>
        Task LoadScene();

        Task UnloadScene();

        void ReceiveSceneTransitionPayload(SceneTransitionPayload payload);
    }
}
