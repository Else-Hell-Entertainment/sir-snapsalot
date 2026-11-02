using System;
using EHE.SirSnapsalot;
using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public partial class Pusher : Trap
    {
        public override bool Activate()
        {
            GD.Print("Pusher activated!");
            return true;
        }
    }
}
