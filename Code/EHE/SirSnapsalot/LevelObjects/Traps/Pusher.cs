using System;
using EHE.SirSnapsalot;
using Godot;

public partial class Pusher : Node3D, IActivateable
{
    public bool Activate()
    {
        GD.Print("Pusher activated!");
        return true;
    }
}
