// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

namespace EHE.SirSnapsalot.Config
{
    public static class SystemConfig
    {
        public const string SaveConfigPath = "res://Data/Resources/SaveSystem/SaveConfig.tres";

        /// <summary>
        /// The scale of the grid in the game world. Each grid cell will be 2x2 units in size.
        /// Required for mapping grid logic to graphical assets;
        /// </summary>
        public const int GridScale = 2;
    }
}
