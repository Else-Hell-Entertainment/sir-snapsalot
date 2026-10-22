using Godot;

namespace EHE.CharacterSystem
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
