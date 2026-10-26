namespace EHE.Global.Config
{
    public static class SystemConfig
    {
        public const string SaveConfigPath = "res://Data/Resources/SaveSystem/SaveConfig.tres";

        // The scale of the grid in the game world. Each grid cell will be 2x2 units in size.
        // Required for mapping grid logic to graphical assets;
        public const int GridScale = 2;
    }
}
