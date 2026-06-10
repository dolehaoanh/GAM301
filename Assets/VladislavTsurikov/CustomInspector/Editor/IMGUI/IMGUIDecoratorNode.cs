#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.CustomInspector.Editor.Core;

namespace VladislavTsurikov.CustomInspector.Editor.IMGUI
{
    public sealed class IMGUIDecoratorNode : InspectorNode
    {
        private readonly IMGUIDecoratorDrawer _decorator;

        public IMGUIDecoratorNode(IMGUIDecoratorDrawer decorator) => _decorator = decorator;

        public override void Execute(InspectorNodeContext context)
        {
            if (context is not IMGUIInspectorNodeContext imguiContext)
            {
                return;
            }

            float decoratorHeight = _decorator.GetHeight(context.Field, context.Target);
            Rect decoratorRect = EditorGUI.IndentedRect(new Rect(
                imguiContext.Rect.x,
                imguiContext.Rect.y,
                imguiContext.Rect.width,
                decoratorHeight));

            _decorator.Draw(decoratorRect, context.Field, context.Target);

            Rect rect = imguiContext.Rect;
            rect.y += decoratorHeight;
            imguiContext.Rect = rect;
            imguiContext.TotalHeight += decoratorHeight;
        }
    }
}
#endif
