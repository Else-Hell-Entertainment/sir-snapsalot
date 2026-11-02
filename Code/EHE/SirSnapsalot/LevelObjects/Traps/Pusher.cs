using System;
using EHE.SirSnapsalot;
using Godot;

namespace EHE.SirSnapsalot.LevelObjects
{
    public partial class Pusher : Node3D, IActivateable
    {
        public bool Activate()
        {
            GD.Print("Pusher activated!");
            return true;
        }
    }
}
