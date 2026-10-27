// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using System.IO;
using Godot;

namespace EHE.SirSnapsalot.SaveSystem
{
    public class MockSerializer : ISaveSerializer
    {
        public string FileExtension => "mock";

        public GameData Deserialize(Stream stream)
        {
            return new GameData();
        }

        public void Serialize(Stream stream, GameData data)
        {
            GD.Print($"[MockSerializer] Saving data {data}");
        }
    }
}
