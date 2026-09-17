namespace EHE.Global.SaveSystem
{
    public sealed class GameData
    {
        public PlayerData PlayerData { get; set; }
        public SceneManagerData SceneManagerData { get; set; }
    }

    public sealed class PlayerData
    {
        public string PlayerId { get; set; }
    }

    public sealed class SceneManagerData { }
}
