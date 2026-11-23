// (c) 2026 Else Hell Entertainment
// License: MIT License (see LICENSE in project root for details)
// Author: Pekka Heljakka <pekka.heljakka@tuni.fi>
//         Miska Rihu <miska.rihu@tuni.fi>

using EHE.Common.Godot.Extensions;
using EHE.Common.Godot.Logging;
using EHE.SirSnapsalot.CharacterSystem;
using EHE.SirSnapsalot.LevelObjects;
using Godot;

namespace EHE.SirSnapsalot.LevelSystem
{
    [GlobalClass]
    public partial class Level : Node3D
    {
        //TODO: Refactor temp solutions below:
        [Export]
        private Button _startGameButton;

        [Export]
        private Character _character;

        public LevelGrid Grid { get; private set; }

        private Entrance _entrance = null;
        private Exit _exit = null;

        /// <summary>
        /// The <see cref="LevelObjects.Entrance"/> object in this level.
        /// </summary>
        public Entrance Entrance
        {
            get
            {
                if (!IsInstanceValid(_entrance))
                {
                    _entrance = this.GetChild<Entrance>(recurse: true);
                }

                return _entrance;
            }
        }

        /// <summary>
        /// The <see cref="LevelObjects.Exit"/> object in this level.
        /// </summary>
        public Exit Exit
        {
            get
            {
                if (!IsInstanceValid(_exit))
                {
                    _exit = this.GetChild<Exit>(recurse: true);
                }

                return _exit;
            }
        }

        /// <summary>
        /// The GlobalPosition of the <see cref="Entrance"/> object.
        /// If no such object is present in the level, returns <see cref="Vector3.Inf"/>.
        /// </summary>
        public Vector3 EntrancePosition =>
            IsInstanceValid(Entrance)
                ? Entrance.GlobalPosition
                : Vector3.Inf;

        /// <summary>
        /// The GlobalPosition of the <see cref="Exit"/> object.
        /// If no such object is present in the level, returns <see cref="Vector3.Inf"/>.
        /// </summary>
        public Vector3 ExitPosition =>
            IsInstanceValid(Exit)
                ? Exit.GlobalPosition
                : Vector3.Inf;

        public override void _Ready()
        {
            Name = "Level";
            Grid = this.GetChild<LevelGrid>(recurse: true);

            if (Grid == null)
            {
                this.LogInfo($"{nameof(Grid)} is null, creating a new instance.");
                Grid = new LevelGrid();
                Grid.Name = "LevelGrid";
                AddChild(Grid);
                // var sceneOwner = GetTree().CurrentScene;
                // if (sceneOwner != null)
                // {
                //     Grid.Owner = sceneOwner;
                // }
            }

            Grid.Initialize(this);

            //TODO: Refactor temp solutions below:

            _startGameButton.Pressed += StartGame;
        }

        public override void _Input(InputEvent @event)
        {
            base._Input(@event);

            //TODO: Refactor temp solutions below:

            if (@event.IsActionPressed("RotateCW"))
            {
                Grid.RotateGhostComponent(true);
            }
            else if (@event.IsActionPressed("RotateCCW"))
            {
                Grid.RotateGhostComponent(false);
            }
            else if (@event.IsActionPressed("ClearCell"))
            {
                Grid.RemoveCell();
            }
        }

        /// <summary>
        /// Sets the current ghost component to the given cell component.
        /// </summary>
        /// <param name="ghostComponent">The reference to the cell component resource.</param>
        public void SetGhostComponent(CellComponent ghostComponent)
        {
            Grid.GhostComponent = ghostComponent;
            this.LogDebug($"Ghost component set to '{ghostComponent}'");
        }

        /// <summary>
        /// Finds the physics collider under a viewport position and returns it
        /// if it is an instance of the requested Godot object type.
        /// </summary>
        ///
        /// <typeparam name="T">
        /// The Godot object type to return.
        /// </typeparam>
        /// <param name="mousePosition">
        /// The viewport position to query.
        /// </param>
        /// <param name="camera">
        /// The camera used to cast the ray. When omitted, the viewport's
        /// active camera is used.
        /// </param>
        ///
        /// <returns>
        /// The collider cast to <typeparamref name="T"/>, or <see langword="null"/>
        /// if no active camera, no collider, or no compatible collider is found.
        /// </returns>
        public T GetObjectUnderMouse<T>(Vector2 mousePosition, Camera3D camera = null)
            where T : GodotObject
        {
            camera ??= GetViewport().GetCamera3D();

            if (!IsInstanceValid(camera))
            {
                this.LogError($"Cannot find active camera in the scene tree, returning null.");
                return null;
            }

            RayCast3D ray = new RayCast3D();
            ray.CollideWithBodies = true;
            ray.CollideWithAreas = false;
            AddChild(ray);

            switch (camera.Projection)
            {
                case Camera3D.ProjectionType.Perspective:
                    CastRayPerspective(ray, mousePosition, camera);
                    break;
                case Camera3D.ProjectionType.Orthogonal:
                    CastRayOrthogonal(ray, mousePosition, camera);
                    break;
                default:
                    this.LogError(
                        $"The currently active camera has unsupported projection type."
                            + $"Use {nameof(Camera3D.ProjectionType.Perspective)} "
                            + $"or {nameof(Camera3D.ProjectionType.Orthogonal)}."
                    );
                    return null;
            }

            // Force update is required because otherwise the collision check
            // would happen on the next physics frame and possibly only after
            // this method has returned.
            ray.ForceRaycastUpdate();
            GodotObject collider = ray.GetCollider();
            ray.QueueFree();

            if (collider == null)
            {
                this.LogDebug("No collider found.");
                return null;
            }

            this.LogDebug("collider type: ", collider.GetType());
            return collider is T tObject ? tObject : null;
        }

        /// <summary>
        /// Configures a ray for an orthogonal camera by projecting the mouse
        /// position into the scene and setting the ray to extend along the
        /// camera's projection direction.
        /// </summary>
        ///
        /// <param name="ray">The ray to configure.</param>
        /// <param name="mousePosition">The viewport position used to create the ray.</param>
        /// <param name="camera">The orthogonal camera used for projection.</param>
        ///
        /// <remarks>
        /// No ray configuration is performed if the ray or camera is no longer
        /// valid.
        /// </remarks>
        ///
        /// <seealso cref="GetObjectUnderMouse"/>
        /// <seealso cref="CastRayPerspective"/>
        private void CastRayOrthogonal(RayCast3D ray, Vector2 mousePosition, Camera3D camera)
        {
            if (!IsInstanceValid(ray) || !IsInstanceValid(camera))
            {
                this.LogError($"{nameof(ray)} or {nameof(camera)} was null, returning.");
                return;
            }

            Vector3 rayOrigin = camera.ProjectPosition(mousePosition, 0.0f);
            Vector3 testPoint = camera.ProjectPosition(mousePosition, 1.0f);
            Vector3 rayDirection = rayOrigin.DirectionTo(testPoint);
            Vector3 rayTarget = rayOrigin + rayDirection * 10000;

            ray.GlobalPosition = rayOrigin;
            ray.TargetPosition = rayTarget;
        }

        /// <summary>
        /// Configures a ray for a perspective camera using the ray origin and
        /// direction projected from the specified viewport position.
        /// </summary>
        ///
        /// <param name="ray">The ray to configure.</param>
        /// <param name="mousePosition">The viewport position used to create the ray.</param>
        /// <param name="camera">The perspective camera used for projection.</param>
        ///
        /// <remarks>
        /// No ray configuration is performed if the ray or camera is no longer
        /// valid.
        /// </remarks>
        ///
        /// <seealso cref="GetObjectUnderMouse"/>
        /// <seealso cref="CastRayOrthogonal"/>
        private void CastRayPerspective(RayCast3D ray, Vector2 mousePosition, Camera3D camera)
        {
            if (!IsInstanceValid(ray) || !IsInstanceValid(camera))
            {
                this.LogError($"{nameof(ray)} or {nameof(camera)} was null, returning.");
                return;
            }

            Vector3 rayOrigin = camera.ProjectRayOrigin(mousePosition);
            Vector3 rayDirection = camera.ProjectRayNormal(mousePosition);
            Vector3 rayTarget = rayOrigin + rayDirection * 10000;

            ray.GlobalPosition = camera.GlobalPosition;
            ray.TargetPosition = rayTarget;
        }

        private void StartGame()
        {
            ConnectActivatorsToTraps();

            _character.GlobalPosition = EntrancePosition;
            Grid.CalculateNavigation();

            //TODO: Refactor placeholder solution below:
            _character.ReceivePath(Grid.GetNavPath(_character.GlobalPosition, ExitPosition));
            _character.Activate();
            ShowPath();
        }

        private void ShowPath()
        {
            Grid.DrawNavigationPath(_character.GlobalPosition, ExitPosition);
        }

        private void ConnectActivatorsToTraps()
        {
            foreach (var activator in this.GetChildren<TrapActivator>(recurse: true))
            {
                // Find activators and clear their existing connections.
                activator.ClearConnections();

                foreach (var gridCell in Grid.GetNeighbors(activator.GlobalPosition))
                {
                    // Find neighboring cells and if they contain a trap, connect it to the activator.
                    var trap = gridCell.GetChild<Trap>(recurse: true);
                    if (trap != null)
                    {
                        activator.AddConnection(trap);
                    }
                }
            }
        }
    }
}
