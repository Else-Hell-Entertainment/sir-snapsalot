// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using System.IO;

namespace EHE.SirSnapsalot.SaveSystem
{
    /// <summary>
    /// Strategy interface for serializing and deserializing game save data.
    /// Swap implementations at runtime to switch between different formats.
    /// </summary>
    public interface ISaveSerializer
    {
        /// <summary>
        /// File extension used for save files produced by this serializer (without the dot).
        /// </summary>
        string FileExtension { get; }

        /// <summary>
        /// Write <paramref name="data"/> to <paramref name="stream"/>.
        /// </summary>
        void Serialize(Stream stream, GameData data);

        /// <summary>
        /// Read and return a <see cref="GameData"/> from <paramref name="stream"/>.
        /// </summary>
        GameData Deserialize(Stream stream);
    }
}
