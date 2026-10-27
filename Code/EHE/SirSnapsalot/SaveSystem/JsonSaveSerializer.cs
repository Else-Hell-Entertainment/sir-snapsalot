// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using System.IO;
using System.Text.Json;

namespace EHE.SirSnapsalot.SaveSystem
{
    public class JsonSaveSerializer : ISaveSerializer
    {
        public string FileExtension => "json";

        private static readonly JsonSerializerOptions _options = new() { WriteIndented = true };

        public GameData Deserialize(Stream stream)
        {
            return JsonSerializer.Deserialize<GameData>(stream, _options);
        }

        public void Serialize(Stream stream, GameData data)
        {
            JsonSerializer.Serialize(stream, data, _options);
        }
    }
}
