using System.Collections.Generic;

namespace EHE.Global.Config
{
    /// <summary>
    /// This class contains the input action strings used in the game. It serves as a centralized location for input action
    /// names, making it easier to manage and reference them throughout the codebase. It is used only to reference the
    /// inputs which have been defined in the Godot project settings. No other data is saved here.
    /// </summary>
    public static class InputConfig
    {
        public const string ESCAPE = "ui_cancel";

        public const string ROTATE_CW = "RotateCW";
        public const string ROTATE_CCW = "RotateCCW";
        public const string CONFIRM = "Confirm";

        /// <summary>
        /// Get a List of all input action strings defined in this class. This can be used to iterate over all actions for
        /// purposes such as validation, remapping, or displaying in a settings menu.
        /// </summary>
        /// <returns>List with all input action strings.</returns>
        public static List<string> GetAllInputActions()
        {
            return new List<string> { ESCAPE, ROTATE_CW, ROTATE_CCW, CONFIRM };
        }
    }
}
