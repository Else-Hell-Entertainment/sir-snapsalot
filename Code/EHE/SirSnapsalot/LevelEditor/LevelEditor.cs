using Godot;
using System;
using System.Collections.Generic;
using EHE.Common.Godot.Extensions;
using EHE.Common.Godot.Logging;
using EHE.SirSnapsalot.LevelSystem;

namespace EHE.SirSnapsalot.LevelEditor
{
    public partial class LevelEditor : Node3D
    {
        private HashSet<ComponentSelector> _componentSelectors = new HashSet<ComponentSelector>();
        private Level _level = null;

        public override void _Ready()
        {
            // Find all ComponentSelectors in the UI and start listening to
            // their ComponentSelected event.
            foreach (var selector in this.GetChildren<ComponentSelector>(recurse: true))
            {
                // Caching the components allows unsubscribing from the events
                // later.
                if (!_componentSelectors.Add(selector))
                {
                    this.LogWarning("Trying to cache an already cached component selector!");
                    continue;
                }

                selector.ComponentSelected += OnComponentSelected;
            }

            // Cache reference to the Level node.
            _level = this.GetChild<Level>(recurse: true);
            if (!IsInstanceValid(_level))
            {
                throw new InvalidOperationException($"There is no level node or the reference is invalid.");
            }
        }

        private void OnComponentSelected(CellComponent selectedComponent)
        {
            if (!IsInstanceValid(_level))
            {
                this.LogError($"There is no level node or the reference is invalid.");
                return;
            }

            _level.SetGhostComponent(selectedComponent);
        }
    }
}
