// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>

using System;
using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class CellComponent : Resource
    {
        // Caching the reference to the packed scene is necessary only if
        // _scene can change during runtime. Most likely this will never happen
        // but since adding it makes Copilot happy, I'll leave it here (also
        // because I don't wanna spend any more of my dev time trying to cache
        // a single property).
        private PackedScene _cachedScene = null;
        private Type _cachedSceneType = null;

        public enum CellComponentType
        {
            None,
            Floor,
            Wall,
            Trap,
            Prop,
            Navigation,
        }

        public enum Position
        {
            None,
            Floor,
            NorthWall,
            SouthWall,
            EastWall,
            WestWall,
        }

        [Export]
        public string Name;

        [Export]
        private PackedScene _scene;

        /// <summary>
        /// Does this component block movement completely, such as a wall? Character can never be moved through a solid
        /// component under any circumstances.
        /// </summary>
        [Export]
        public bool IsSolid = false;

        /// <summary>
        /// Does this component block pathfinding? Some components may be impassable but not solid, such as a trap pit that
        /// blocks regular movement but character may still fall into it.
        /// </summary>
        [Export]
        public bool BlocksMovement = false;

        // TODO: Could this be exported?
        // -> Makes modifying existing components easier in the editor.
        // -> However, might make rotating walls etc. harder to implement if common rotation method is added.
        public virtual CellComponentType ComponentType => CellComponentType.None;

        // TODO: Maybe add exported RequiresFloor property?
        // -> Makes it easier to check whether the placement is valid.
        // See LevelGrid.PlaceGhostComponent

        /// <summary>
        /// Creates a new instance of the <see cref="PackedScene"/> associated
        /// with this resource. Returns <c>null</c> if no scene is specified or
        /// the scene cannot be instantiated.
        /// </summary>
        ///
        /// <remarks>
        /// This method will also cache the runtime type of the instantiated
        /// instance. This can be retrieved using the <see cref="GetInstanceType"/>
        /// method.
        /// </remarks>
        ///
        /// <seealso cref="GetInstanceType"/>
        public Node GetSceneInstance()
        {
            // AI Assisted

            if (_scene == null)
            {
                _cachedScene = null;
                _cachedSceneType = null;
                return null;
            }

            Node sceneInstance = _scene.Instantiate();

            if (sceneInstance != null)
            {
                _cachedScene = _scene;
                _cachedSceneType = sceneInstance.GetType();
            }
            else
            {
                _cachedScene = null;
                _cachedSceneType = null;
            }

            return sceneInstance;
        }

        /// <summary>
        /// Gets the runtime type of the instance created from this resource's
        /// scene.
        /// </summary>
        ///
        /// <returns>
        /// The scene instance's runtime type, or <c>null</c> if no scene is
        /// specified or it cannot be instantiated.
        /// </returns>
        ///
        /// <remarks>
        /// <para>
        /// The type is cached for the current scene. If it has not yet been
        /// cached, this method temporarily instantiates the scene to determine
        /// the type, then frees that instance.
        /// </para>
        /// <para>
        /// This method is intended to be used with the object removal system
        /// in level editor.
        /// </para>
        /// </remarks>
        public Type GetInstanceType()
        {
            // AI Generated

            if (_scene == null)
            {
                _cachedScene = null;
                _cachedSceneType = null;
                return null;
            }

            if (!ReferenceEquals(_cachedScene, _scene))
            {
                Node sceneInstance = GetSceneInstance();
                sceneInstance?.Free();
            }

            return _cachedSceneType;
        }
    }
}
