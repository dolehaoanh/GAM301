#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.AttributeUtility.Runtime;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.Nody.Runtime.Core;

namespace VladislavTsurikov.MegaWorld.Editor.Core.Window
{
    public class WindowToolStackEditor : TabComponentStackEditor<ToolWindow, ToolWindowEditor>
    {
        public WindowToolStackEditor(WindowToolStack stack) : base(stack)
        {
        }

        public override void OnTabStackGUI()
        {
            _tabStackEditor.OnGUI();

            if (Stack.SelectedElement == null)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                CustomEditorGUILayout.LabelField("No Tool Selected");
                EditorGUILayout.EndVertical();
            }
        }

        protected override GenericMenu TabMenu(int currentTabIndex)
        {
            GenericMenu menu = base.TabMenu(currentTabIndex);

            DocUrlAttribute docAttr = Stack.ElementList[currentTabIndex].GetType().GetAttribute<DocUrlAttribute>();
            if (docAttr != null)
            {
                menu.AddSeparator("");
                menu.AddItem(new GUIContent("Documentation"), false,
                    () => Application.OpenURL(docAttr.Url));
            }

            return menu;
        }
    }
}
#endif
