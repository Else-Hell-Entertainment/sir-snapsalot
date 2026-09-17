using EHE.Global.Managers;
using Godot;
using Godot.Collections;

namespace EHE.Global.SceneManagement
{
    /// <summary>
    /// Node for calling scene transitions from within a level.
    /// Drop this node into a scene and set the target scene in the editor.
    /// If necessary, create the Payload in code before calling RequestTransition()
    /// to switch scenes.
    /// </summary>
    [GlobalClass]
    [Tool]
    public partial class SceneTransitionCaller : Node
    {
        private SceneCollection _sceneCollection;
        private SceneCollections.Collections _collectionEnum;

        [Export]
        public SceneCollection SceneCollection
        {
            get => _sceneCollection;
            set
            {
                _sceneCollection = value;
                NotifyPropertyListChanged();
            }
        }

        [Export]
        public SceneCollections.Collections Collection
        {
            get => _collectionEnum;
            set
            {
                _collectionEnum = value;
                Set("TransitionTarget", "");
                SceneCollection = SceneCollections.GetSceneCollection(value);
            }
        }

        [Export]
        private SceneManager.SceneTransitionType _sceneTransitionType = SceneManager.SceneTransitionType.Default;

        private string _targetSceneKey;

        public string TargetScene => _sceneCollection?.GetScenePath(_targetSceneKey);

        public void RequestTransition(SceneTransitionPayload payload = null)
        {
            if (TargetScene == null)
            {
                GD.PrintErr($"SceneTransitionCaller on node '{Name}': TargetScene is not set!");
                return;
            }

            SceneTransition transition = new SceneTransition(_collectionEnum, _targetSceneKey);
            transition.SetTransitionType(_sceneTransitionType);
            if (payload != null)
            {
                transition.SetPayload(payload);
            }

            SceneManager.Instance.RequestSceneTransition(transition);
        }

        public override Array<Dictionary> _GetPropertyList()
        {
            Array<Dictionary> properties = [];
            string transitionHintString = "";
            int propertyHint = (int)PropertyHint.None;
            PropertyUsageFlags usage = PropertyUsageFlags.ReadOnly | PropertyUsageFlags.Default;
            if (_sceneCollection != null)
            {
                transitionHintString = _sceneCollection.GetKeysAsString();
                propertyHint = (int)PropertyHint.Enum;
                usage = PropertyUsageFlags.Default;
            }

            properties.Add(
                new Dictionary()
                {
                    { "name", "TransitionTarget" },
                    { "type", (int)Variant.Type.String },
                    { "hint", propertyHint },
                    { "hint_string", transitionHintString },
                    { "usage", (int)usage },
                }
            );

            return properties;
        }

        public override bool _Set(StringName property, Variant value)
        {
            if (property == "TransitionTarget")
            {
                _targetSceneKey = value.AsString();
                //GD.Print("TransitionTarget set to: " + _targetSceneKey);
                NotifyPropertyListChanged();
                return true;
            }

            return false;
        }

        public override Variant _Get(StringName property)
        {
            if (property == "TransitionTarget")
            {
                if (_sceneCollection == null)
                {
                    _targetSceneKey = "";
                }

                return _targetSceneKey;
            }

            return default;
        }
    }
}
