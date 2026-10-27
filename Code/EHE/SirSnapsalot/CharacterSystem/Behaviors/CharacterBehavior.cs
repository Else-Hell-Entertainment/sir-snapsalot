// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using Godot;

namespace EHE.SirSnapsalot.CharacterSystem
{
    public abstract partial class CharacterBehavior : Node3D
    {
        public enum BehaviorTypeDef
        {
            Standard,
            Forced,
            Uninterruptible,
        }

        public abstract BehaviorTypeDef BehaviorType { get; }

        public abstract void Initialize(Character character);
        public abstract void Execute();
    }
}
