using System;
using OdinSerializer;
using UnityEditor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.Nody.Runtime.AdvancedNodeStack;
using VladislavTsurikov.Nody.Runtime.Core;
using VladislavTsurikov.ScriptableObjectUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Runtime.Core.PreferencesSystem
{
    [LocationAsset("MegaWorld/PreferencesSettings")]
    public class PreferencesSettings : SerializedScriptableObjectSingleton<PreferencesSettings>
    {
        [OdinSerialize]
        private NodeStackOnlyDifferentTypes<PreferenceSettings> _stack = new();

#if UNITY_EDITOR
        public IMGUIComponentStackEditor<PreferenceSettings, IMGUIElementEditor> StackEditor;
#endif

        private void OnEnable()
        {
            _stack.CreateAllElementTypes();

#if UNITY_EDITOR
            StackEditor = new IMGUIComponentStackEditor<PreferenceSettings, IMGUIElementEditor>(_stack);
#endif
        }

        public Node GetElement(Type elementType)
        {
            return _stack.GetElement(elementType);
        }

#if UNITY_EDITOR
        public void Save()
        {
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
