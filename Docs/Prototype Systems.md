# Prototype systems

This guide is for developers new to the project. It describes the implemented C# systems, how they are connected, how to extend them, and where the prototype stops. Scene resources, input actions, and exported node references are part of the implementation: code changes may also require Godot editor changes.

> **Prototype status:** many systems are scaffolding or editor/play-test experiments. A type or menu being present does not mean the associated feature is complete or production-ready.

## Runtime and game states

`GameManager`, `SceneManager`, `SettingsManager`, and `AudioManager` are configured as Godot autoloads in `project.godot`. `GameManager` creates its `UIManager` child and creates and initializes `SaveManager` from `Data/Resources/SaveSystem/SaveConfig.tres`.

The project starts at the bootstrap scene. `BootStrap` waits for its splash animation, asks `SceneManager` to load the title-screen scene, then removes itself. The bootstrap deliberately does not rely on `SceneManager` until after the autoloads are ready.

`GameManager` tracks `Boot`, `TitleScreen`, `Playing`, and `Paused` states using the shared `StateMachine`. State nodes are entered/exited through `GameStateBase`. While playing, Escape requests `Paused`; the pause state displays the pause menu, makes the mouse visible, and pauses the scene tree. Resuming reverses those actions.

**For developers:** request game-state changes through `GameManager.Instance.RequestStateTransition(...)`; use the state classes for state-specific entry/exit behavior. Add a new state to both the `GameState` enum and the state-instance registration in `GameManager.Initialize`.

**Prototype limits:** `GameManager.IsValidTransition` currently accepts every transition. State validity rules are not implemented. Starting and loading a game depend on `SceneManager.CurrentSceneKey`, but the bootstrap loads the title screen using a raw scene path (which has no scene key); new-game/load flows therefore need a valid level key to be assigned before they can reliably start a level. The start data builder also currently does not assign its locally created `SceneManagerData` to `GameData`.

## Scene loading and transitions

`SceneManager` keeps the active scene beneath a private root node, unloads it, loads the next `PackedScene` on a worker thread, instantiates it, and then waits a frame. Only one managed active scene is tracked at a time.

- Use `SceneTransition(SceneCollections.Collections, sceneKey)` for levels and scenes that need a stable collection key. `StartLoadedGame` reuses this key to choose a level. Collection resources live under `Data/Resources/SceneCollections`; their keys must match `SceneCollections`. Scene state itself is not currently saved.
- Use `SceneTransition(scenePath)` for one-off scenes such as the title screen. This path does not set a collection key and is explicitly not compatible with scene-key-based save/load.
- `SceneTransitionCaller` is an editor-configurable helper for collection-based transitions. It can attach a `SceneTransitionPayload`.
- Implement `ILoadableScene` on scenes that need asynchronous `LoadScene`/`UnloadScene` hooks or payload handling. `LoadScene` runs after the scene has entered the tree; `UnloadScene` runs before it is freed.

`SceneTransitionType` currently has `Default`, `Instant`, `Fade`, and `LoadingScreen` values, but transition-type handling is a TODO. Threaded loading progress is exposed for future UI and currently only logged.

## Menus and UI

`UIManager` creates separate canvas layers for the HUD and menus and instantiates the title menu, pause menu, and player HUD from packed scenes. Menu scene paths are resolved through the menu scene collection.

`MenuRoot` provides a stack-based menu state machine. A menu page derives from `MenuScene`, emits `RequestTransitionEvent` to open a keyed page, and emits `ReturnToPreviousEvent` to go back. Add menu states in the root node's `Initialize` method. Additive states can preserve an underlying page while a settings subpage is open. The title menu root registers main/settings pages; the pause root registers pause and a subset of settings pages.

**Implemented interactions:** starting/loading requests go through `GameManager`; pause-menu quick-save calls `SaveManager.QuickSave`; settings navigation is wired through menu states. `PlayerHud` can be instantiated by `UIManager`, but HUD content/updates are not driven by a complete gameplay system.

**Not implemented:** the title-screen Save and Credits buttons only print messages, and the pause-menu Load button has no behavior. There is no save-slot selection UI. The UI manager's methods are building blocks; do not assume every scene change or player lifecycle automatically creates/destroys the HUD.

## Level grid and component placement

`Level` is a prototype level/editor controller. It expects exported references to component resources, a start button, and a `Character`. It finds or creates a `LevelGrid`, connects the start button, and routes editor input to the grid. Input action names are configured in `project.godot`:

| Action | Current operation |
|---|---|
| `Hotbar1`, `Hotbar2`, `Hotbar3` | Select floor, wall, or prop |
| `LevelEditorSelectPressurePlate`, `LevelEditorSelectPusher` | Select trap component |
| `RotateCW`, `RotateCCW` | Rotate the selected wall preview |
| `Confirm` | Place the selected component |
| `ClearCell` | Remove the cell under the pointer |

`LevelGrid` maps the mouse ray onto the world X/Z plane and divides coordinates by `SystemConfig.GridScale` (currently 2 world units per cell). A cell is a `GridCell` node stored by integer grid coordinate. A floor can create a new cell; walls, props, and traps require an existing cell. The selected resource creates a ghost instance for preview. `CellComponent` resources specify component type, packed scene, `IsSolid`, and `BlocksMovement`; `CellComponentData` records the component and its slot on a cell.

Component scenes are instantiated by `GridCell`. Floors sit at the cell origin; wall slots are positioned and rotated on the four cell edges. Placement validation prevents a second floor and a second wall in the same slot. Current prop/trap validation only checks that a cell exists, so multiple such components can be stacked. `RemoveCell` clears and frees the entire cell, not just one component.

**For developers:** create/configure a `CellComponent` resource and its packed scene, set its type through a concrete resource class (`FloorComponent`, `WallComponent`, `PropComponent`, or `TrapComponent`), and wire it to the relevant exported field in the level scene. Extend placement rules in `LevelGrid.IsPlacementValid` if the component has different placement constraints. Update `project.godot` and `InputConfig` if adding/remapping named actions.

**Prototype limits:** this is not a full editor/inventory/build system. Bounds, ownership/permissions, undo, removal of individual components, and robust placement feedback are absent. Several exported references are assumed to be assigned in the scene. The grid stores component resources and spawns scene instances, but a full level persistence workflow is not connected.

## Navigation and character movement

`PathFinder` builds a Godot `AStar2D` graph from the `LevelGrid.Cells` dictionary. Each existing cell becomes a point; only orthogonally adjacent cells can connect. A connection is permitted only when both cells allow movement across their corresponding edge. `GridCell.IsPassableToDirection` and `IsPassableFromDirection` use each component's `BlocksMovement` flag; `IsSolid` is not currently used by this pathfinding logic.

The level's start button currently resets the character to its captured start position, calculates the graph, requests a path, activates movement, and draws a debug path. Both navigation calls use hard-coded grid positions `(0,0)` and `(5,5)`. `Character` follows the supplied world-space point array at its exported `Speed` and snaps to each target within 0.1 units.

**Prototype limits:** start/goal selection, no-path feedback, path costs, dynamic graph updates, obstacle collision, and navigation integration with a real player controller are not implemented. Graph points are based on cells rather than explicitly checking for a walkable floor. `DefaultMovementBehavior` is separate scaffolding and is not the behavior used by `Character`; `Character` currently contains its own simple path-following implementation. A null/empty path is not handled consistently by all consumers, so validate the returned path before passing it to a character.

## Character, player, and traps

`Character` is a generic `Node3D` path follower. `CharacterBehavior` and `DefaultMovementBehavior` define a future behavior abstraction, but the current `Character` has a private behavior field that is not used to execute movement.

`Player` is a separate `ISaveable<PlayerData>` placeholder, not a `Character` subclass. Its save method returns empty data and its load method is empty. `PlayerManager` stores the player reference and player data, but the current manager does not instantiate a `Player`; HUD creation and player spawning must be wired by a gameplay scene/lifecycle implementation before they can be relied upon.

`IActivateable` is the contract for trap-like nodes (`bool Activate()`). `PressurePlate` listens to two exported `Area3D`s:

1. `TriggerArea.BodyEntered` activates the plate when the entering body is a `Character`.
2. `ConnectionDetectors.BodyEntered` adds an entering `IActivateable` to the activation target list.
3. Triggering iterates that list and calls `Activate`.

`Pusher` currently only prints an activation message and returns `true`; it does not push or otherwise affect a character.

**Prototype limits:** pressure-plate connections are discovered from overlap, not explicit editor placement/wiring. The code itself notes that a ghost/preview can be registered; targets are not removed on exit or deduplicated. There is no release/reset signal, activation policy, or persistence for trap state.

## Save/load

`SaveManager` selects a serializer from `SaveConfig`. The checked-in configuration uses the JSON backend, saves under `user://save`, and uses `QuickSave` as its quick slot. `ListSaves`, `Save`, `Load`, `QuickSave`, and `QuickLoad` form the public API. The serializer interface allows a backend to be swapped without changing the manager.

The current `GameData` contains `PlayerData` and `SceneManagerData`. `SaveManager` collects player state and applies both data objects through `ISaveable<T>`. JSON serialization is implemented. `MockSerializer` only logs and returns a fresh empty `GameData`; `BinarySaveSerializer` is explicitly a placeholder whose methods do not serialize or deserialize data. Scene manager save/load methods are empty, and `SceneManagerData` has no fields.

**For developers:** add persistent fields to the appropriate plain data class, implement `ISaveable<T>` on the runtime owner, and wire that owner into `SaveManager.CollectData`/`ApplyState` (or extract a shared registration mechanism when the number of systems grows). When adding a backend, implement both serializer methods and test round-tripping before selecting it in `SaveConfig`.

**Prototype limits:** only player data is collected, and the player itself is currently a stub. Scene/level state is not saved. `GameManager.LoadGame` continues its flow even when quick-load reports that no save exists. There is no save-slot UI, migration/versioning for JSON, or complete error/recovery UX. Do not select the binary backend expecting a working save file.

## Settings and audio

`SettingsManager` holds `SettingsData` and reads/writes `user://settings.json`. Data is grouped into video, audio, gameplay, and controls. The settings menu opens subpages through menu states.

- **Video:** supports a fixed list of resolutions and windowed/fullscreen/exclusive-fullscreen modes. Applying options changes the window immediately; the video settings page supports save, cancel/reload, reset, and a confirmation prompt when navigating away with unsaved changes.
- **Audio:** stores Master/Music/SFX percentages and exposes slider/reset operations. Audio settings are saved when their page exits.
- **Gameplay:** currently only stores a placeholder difficulty string. Applying gameplay settings just prints a message.
- **Controls:** a settings page generates remap buttons for the action names returned by `InputConfig.GetAllInputActions`. Remapping currently listens for a keyboard key and replaces all events on that action.

`AudioManager` defines the expected bus names and is the intended central audio entry point. Its player creation and bus-volume implementation are commented out; `SetLinearVolume` currently returns success without changing the audio server. Consequently, sliders update settings data/labels, but actual game audio is not affected.

**Prototype limits:** gameplay settings are not applied. The controls page has no save/reset workflow of its own, remapping is keyboard-only, and the action list in `InputConfig` is not the complete set of project actions used by the level editor. Settings should not yet be treated as a finished, validated configuration system.

## Key dependencies and extension checklist

- **Godot project settings:** autoloads, input actions, main scene, and audio buses are configured outside the C# source.
- **Scene resources:** scene collection `.tres` resources and scene keys must stay synchronized with `SceneCollections`.
- **Exported scene properties:** level component resources, UI controls, character, and trigger areas must be assigned in the Godot editor.
- **Component resources:** placement, rendering, and path blocking depend on the component's concrete type, packed scene, slot, and movement flags.
- **Save configuration:** selected backend and user path come from `SaveConfig.tres`; changing the backend alone does not make an unimplemented serializer functional.

When extending a prototype system, first identify its runtime owner and scene/configuration dependencies, then wire both the data path and the UI/input path. Add tests or a small in-editor play-test for the behavior and revise the limitation notes above when a prototype gap is actually closed.
