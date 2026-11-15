using System;
using EHE.SirSnapsalot.Config;
using EHE.SirSnapsalot.Managers;
using EHE.SirSnapsalot.SceneManagement;
using Godot;

public partial class LevelSelection : Node3D
{
    [Export]
    private Button _level1Button;

    [Export]
    private Button _level2Button;

    private SceneCollection _levelSceneCollection;

    public override void _Ready()
    {
        base._Ready();
        DisplayServer.MouseSetMode(DisplayServer.MouseMode.Visible);
        _levelSceneCollection = SceneCollections.GetSceneCollection(SceneCollections.Collections.LevelScenes);

        //TODO: Change this to something modular with a parameter to specify the level to load.
        _level1Button.Pressed += OnLevel1ButtonPressed;
        _level2Button.Pressed += OnLevel2ButtonPressed;
    }

    private void OnLevel1ButtonPressed()
    {
        SceneManager.Instance.RequestSceneTransition(
            _levelSceneCollection.GetScenePath(SceneCollections.LevelScenes.Keys.Level1)
        );
    }

    private void OnLevel2ButtonPressed()
    {
        SceneManager.Instance.RequestSceneTransition(
            _levelSceneCollection.GetScenePath(SceneCollections.LevelScenes.Keys.Level2)
        );
    }
}
