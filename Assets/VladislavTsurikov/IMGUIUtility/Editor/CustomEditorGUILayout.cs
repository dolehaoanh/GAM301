#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ColorUtility.Runtime;
using Object = UnityEngine.Object;

namespace VladislavTsurikov.IMGUIUtility.Editor
{
    public static class CustomEditorGUILayout
    {
        public static float LabelWidth = 230;

        public static bool IsInspector = true;
        public static Rect ScreenRect;
        private static GUISkin Skin => AssetDatabase.LoadAssetAtPath<GUISkin>(IMGUIContentPath.SkinPath);

        public static GUIStyle GetStyle(StyleName styleName)
        {
            return GetStyle(styleName.ToString());
        }

        public static float GetCurrentSpace()
        {
            if (EditorGUI.indentLevel == 0)
            {
                if (IsInspector)
                {
                    return 0;
                }

                return 5;
            }

            return 15 * EditorGUI.indentLevel;
        }

        public static float GetCurrentSpace(int indentLevel)
        {
            if (indentLevel == 0)
            {
                if (IsInspector)
                {
                    return 0;
                }

                return 5;
            }

            return 15 * indentLevel;
        }

        public static bool Toggle(GUIContent text, bool value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.Toggle(rect, text, value);
        }

        public static bool Toggle(string text, bool value)
        {
            return Toggle(new GUIContent(text), value);
        }

        public static Enum EnumPopup(GUIContent text, Enum value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.EnumPopup(rect, text, value);
        }

        public static Enum EnumPopup(string text, Enum value)
        {
            return EnumPopup(new GUIContent(text), value);
        }

        public static int Popup(GUIContent text, int value, string[] displayedOptions)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.Popup(rect, text, value, displayedOptions);
        }

        public static int Popup(string text, int value, string[] displayedOptions)
        {
            return Popup(new GUIContent(text), value, displayedOptions);
        }

        public static float Slider(GUIContent text, float value, float min, float max)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.Slider(rect, text, value, min, max);
        }

        public static float Slider(string text, float value, float min, float max)
        {
            return Slider(new GUIContent(text), value, min, max);
        }

        public static int IntSlider(GUIContent text, int value, int min, int max)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.IntSlider(rect, text, value, min, max);
        }

        public static int IntSlider(string text, int value, int min, int max)
        {
            return IntSlider(new GUIContent(text), value, min, max);
        }

        public static Bounds BoundsField(GUIContent text, Bounds value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.BoundsField(rect, text, value);
        }

        public static Bounds BoundsField(string text, Bounds value)
        {
            return BoundsField(new GUIContent(text), value);
        }

        public static float FloatField(GUIContent text, float value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.FloatField(rect, text, value);
        }

        public static float FloatField(string text, float value)
        {
            return FloatField(new GUIContent(text), value);
        }

        public static int IntField(GUIContent text, int value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.IntField(rect, text, value);
        }

        public static int IntField(string text, int value)
        {
            return IntField(new GUIContent(text), value);
        }

        public static Color ColorField(GUIContent text, Color value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.ColorField(rect, text, value);
        }

        public static Color ColorField(string text, Color value)
        {
            return ColorField(new GUIContent(text), value);
        }

        public static Gradient GradientField(GUIContent text, Gradient value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.GradientField(rect, text, value);
        }

        public static Gradient GradientField(string text, Gradient value)
        {
            return GradientField(new GUIContent(text), value);
        }

        public static Vector4 Vector4Field(GUIContent text, Vector4 value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.Vector4Field(rect, text, value);
        }

        public static Vector4 Vector4Field(string text, Vector4 value)
        {
            return Vector4Field(new GUIContent(text), value);
        }

        public static Vector3 Vector3Field(GUIContent text, Vector3 value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.Vector3Field(rect, text, value);
        }

        public static Vector3 Vector3Field(string text, Vector3 value)
        {
            return Vector3Field(new GUIContent(text), value);
        }

        public static Vector2 Vector2Field(GUIContent text, Vector2 value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.Vector2Field(rect, text, value);
        }

        public static Vector2 Vector2Field(string text, Vector2 value)
        {
            return Vector2Field(new GUIContent(text), value);
        }

        public static string TextField(string value)
        {
            return CustomEditorGUI.TextField(value);
        }

        public static string TextField(GUIContent text, string value)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.TextField(rect, text, value);
        }

        public static string TextField(string text, string value)
        {
            return TextField(new GUIContent(text), value);
        }

        public static Object ObjectField(GUIContent text, Object value, Type objType, bool allowSceneObjects = true)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.ObjectField(rect, text, value, objType, allowSceneObjects);
        }

        public static Object ObjectField(string text, Object value, Type objType, bool allowSceneObjects = true)
        {
            return ObjectField(new GUIContent(text), value, objType, allowSceneObjects);
        }

        public static Object ObjectField(Object value, Type objType, bool allowSceneObjects = true)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return CustomEditorGUI.ObjectField(rect, value, objType, allowSceneObjects);
        }

        public static Object ObjectField(GUIContent text, bool isObjectNull, Object value, Type objType,
            int endHorizontalSpace = 5)
        {
            return ObjectField(text, value, objType);
        }

        public static LayerMask LayerField(GUIContent text, LayerMask value)
        {
            return LayerMaskField(text, value);
        }

        public static LayerMask LayerField(string text, LayerMask value)
        {
            return LayerMaskField(new GUIContent(text), value);
        }

        public static void PropertyField(GUIContent text, SerializedProperty property)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUI.GetPropertyHeight(property, GUIContent.none));
            Rect fieldRect = CustomEditorGUI.PrefixLabel(rect, text);
            CustomEditorGUI.PropertyField(fieldRect, property);
        }

        public static void HelpBox(string message, MessageType type)
        {
            float width = Mathf.Max(1f, EditorGUIUtility.currentViewWidth - GetCurrentSpace() - 25f);
            float height = EditorStyles.helpBox.CalcHeight(new GUIContent(message), width);
            Rect rect = EditorGUILayout.GetControlRect(true, height);
            rect = EditorGUI.IndentedRect(rect);
            CustomEditorGUI.HelpBox(rect, message, type);
        }

        public static void LabelField(string text)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.LabelField(rect, text);
        }

        public static void LabelField(GUIContent text)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.LabelField(rect, text);
        }

        public static void LabelField(string text, GUIStyle style)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.LabelField(rect, text, style);
        }

        public static void LabelField(GUIContent text, GUIStyle style)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            CustomEditorGUI.LabelField(rect, text, style);
        }

        public static void MinMaxSlider(GUIContent text, ref float min, ref float max, float minimumValue,
            float maximumValue)
        {
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            float initialMinValue = min;
            float initialMaxValue = max;

            float localMinValue = min;
            float localMaxValue = max;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(GetCurrentSpace(indentLevel));
                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));
                labelRect.width += 25;
                labelRect.x += 4;
                CustomEditorGUI.LabelField(labelRect, text);

                GUILayout.Space(2);
                CustomEditorGUI.MinMaxSlider(ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

                if (!initialMinValue.Equals(localMinValue))
                {
                    GUI.changed = true;
                }
                else if (!initialMaxValue.Equals(localMaxValue))
                {
                    GUI.changed = true;
                }

                min = localMinValue;
                max = localMaxValue;
                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            EditorGUI.indentLevel = indentLevel;
        }

        public static void MinMaxIntSlider(GUIContent text, ref int min, ref int max, int minimumValue,
            int maximumValue)
        {
            int indentLevel = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            int initialMinValue = min;
            int initialMaxValue = max;

            int localMinValue = min;
            int localMaxValue = max;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(GetCurrentSpace(indentLevel));
                Rect labelRect = EditorGUILayout.GetControlRect(GUILayout.Height(15), GUILayout.Width(LabelWidth));
                labelRect.width += 25;
                labelRect.x += 4;
                CustomEditorGUI.LabelField(labelRect, text);

                CustomEditorGUI.MinMaxIntSlider(ref localMinValue, ref localMaxValue, minimumValue, maximumValue);

                if (!initialMinValue.Equals(localMinValue))
                {
                    GUI.changed = true;
                }
                else if (!initialMaxValue.Equals(localMaxValue))
                {
                    GUI.changed = true;
                }

                min = localMinValue;
                max = localMaxValue;
                GUILayout.Space(5);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(2);

            EditorGUI.indentLevel = indentLevel;
        }

        public static void MinMaxSlider(ref float min, ref float max, float minimumValue, float maximumValue)
        {
            CustomEditorGUI.MinMaxSlider(ref min, ref max, minimumValue, maximumValue);
        }

        private static void MinMaxIntSlider(ref int min, ref int max, int minimumValue, int maximumValue)
        {
            CustomEditorGUI.MinMaxIntSlider(ref min, ref max, minimumValue, maximumValue);
        }

        public static bool DrawIcon(StyleName styleName, Color iconColor, float rowHeight = -1)
        {
            GUIStyle style = GetStyle(styleName);

            Color color = GUI.color;
            if (rowHeight > 0)
            {
                GUILayout.BeginVertical(GUILayout.Width(style.fixedWidth), GUILayout.Height(rowHeight));
                GUILayout.Space((rowHeight - style.fixedHeight) / 2);
            }

            GUI.color = iconColor;
            bool buttonClicked = GUILayout.Button(GUIContent.none, style);
            GUI.color = color;
            if (rowHeight > 0)
            {
                GUILayout.EndVertical();
            }

            if (!buttonClicked)
            {
                return false;
            }

            GUIUtility.keyboardControl = 0;
            Event.current.Use();
            return true;
        }

        public static bool ToggleButton(string text, bool included, ButtonStyle colorSpace,
            ButtonSize buttonSize = ButtonSize.General)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color barColor;
            Color labelColor = EditorColors.Instance.LabelColor;

            if (included)
            {
                barColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                barColor = EditorColors.Instance.ToggleButtonInactiveColor;
            }

            if (buttonSize == ButtonSize.ToolButton)
            {
                return Button(text, labelStyle, barStyle, labelColor, barColor, 25);
            }

            return Button(text, labelStyle, barStyle, labelColor, barColor, 21);
        }

        public static bool ClickButton(string text)
        {
            return ClickButton(text, ButtonStyle.ButtonClick);
        }

        public static bool ClickButton(string text, ButtonStyle buttonStyle, ButtonSize buttonSize = ButtonSize.General)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButtonClick);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color labelColor = EditorColors.Instance.LabelColor;
            Color barColor = EditorColors.Instance.ClickButtonColor;

            SetButtonColors(buttonStyle, out barColor, out labelColor);

            if (buttonSize == ButtonSize.DragAndDropButton)
            {
                return Button(text, labelStyle, barStyle, labelColor, barColor, 40);
            }

            return Button(text, labelStyle, barStyle, labelColor, barColor, 20);
        }

        public static bool Button(string text, GUIStyle labelStyle, GUIStyle barStyle, Color labelColor, Color barColor,
            float height)
        {
            GUIStyle localBarStyle = new(barStyle) { fixedHeight = height };

            float iconPadding = height * 0.1f;

            bool result;

            Color color = GUI.color;

            GUILayout.BeginVertical();
            {
                GUI.color = barColor;
                result = GUILayout.Button(GUIContent.none, localBarStyle);
                GUILayout.Space(-height);
                GUI.color = color;

                labelStyle.normal.textColor = labelColor;

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label(new GUIContent(text), labelStyle, GUILayout.ExpandWidth(true),
                        GUILayout.Height(height));
                    GUILayout.Space(iconPadding);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            return result;
        }

        public static void RectTab(Rect rect, string text, ButtonStyle colorSpace, float height, int fontSize)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color labelColor = EditorColors.Instance.LabelColor;

            SetButtonColors(colorSpace, out Color barColor, out labelColor);

            GUIStyle localBarStyle = new(barStyle) { fixedHeight = height };

            GUIStyle localLabelStyle = new(labelStyle) { normal = { textColor = labelColor }, fontSize = fontSize };

            RectTab(text, rect, localLabelStyle, localBarStyle, barColor);
        }

        public static void RectTab(Rect rect, string text, bool included, float height)
        {
            Color barColor;
            Color labelColor = EditorColors.Instance.LabelColor;

            if (included)
            {
                barColor = EditorColors.Instance.ToggleButtonActiveColor;
            }
            else
            {
                barColor = EditorColors.Instance.ToggleButtonInactiveColor;
            }

            RectTab(rect, text, barColor, labelColor, height);
        }

        public static void RectTab(Rect rect, string text, Color barColor, Color labelColor, float height)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelButton);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            GUIStyle localBarStyle = new(barStyle) { fixedHeight = height };

            GUIStyle localLabelStyle = new(labelStyle) { normal = { textColor = labelColor } };

            RectTab(text, rect, localLabelStyle, localBarStyle, barColor);
        }

        public static void RectTab(string text, Rect tabRect, GUIStyle labelStyle, GUIStyle barStyle, Color barColor)
        {
            Color initialColor = GUI.color;

            GUI.color = barColor;
            EditorGUI.LabelField(tabRect, "", barStyle);
            GUI.color = initialColor;

            EditorGUI.LabelField(tabRect, text, labelStyle);
        }

        public static bool HeaderWithMenu(string content, bool foldout, ref bool active, Action menu)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            if (IsInspector)
            {
                rect.x -= 5;
            }

            DrawMenu(rect, menu);

            return DrawHeader(rect, content, foldout, ref active, menu);
        }

        public static bool HeaderWithMenu(string content, bool foldout, Action menu)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            if (IsInspector)
            {
                rect.x -= 5;
            }

            DrawMenu(rect, menu);

            return DrawHeader(rect, content, foldout);
        }

        private static void DrawMenu(Rect rect, Action menu)
        {
            Rect buttonRect = rect;
            buttonRect.x += rect.width - EditorGUIUtility.singleLineHeight;
            buttonRect.width = EditorGUIUtility.singleLineHeight;

            Color color = GUI.color;
            Texture2D menuIcon = Styling.paneOptionsIcon;
            Rect menuRect = new(buttonRect.x, buttonRect.y + 4f, menuIcon.width, menuIcon.height);
            GUI.DrawTexture(menuRect, Styling.paneOptionsIcon);
            if (GUI.Button(menuRect, GUIContent.none, GUIStyle.none))
            {
                menu.Invoke();
            }

            GUI.color = color;
        }

        public static bool DrawHeader(Rect rect, string content, bool foldout, ref bool active, Action menu)
        {
            Texture texture;

            if (foldout)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutDownPath);
            }
            else
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutRightPath);
            }

            Rect foldoutRect = rect;

            Color initialColor = GUI.color;
            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(foldoutRect, texture);
            GUI.color = initialColor;

            Rect toggleRect = foldoutRect;

            toggleRect.x += EditorGUIUtility.singleLineHeight;
            toggleRect.width = 30;

            active = EditorGUI.Toggle(toggleRect, "", active);

            Rect labelRect = toggleRect;
            labelRect.x += EditorGUIUtility.singleLineHeight;
            labelRect.width = rect.width;

            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(labelRect, content, labelStyle);

            Event e = Event.current;

            if (e.type == EventType.MouseDown)
            {
                if (rect.Contains(e.mousePosition))
                {
                    // Left click: Expand/Collapse
                    if (e.button == 0)
                    {
                        foldout = !foldout;
                    }
                    // Right click: Context menu
                    else if (menu != null)
                    {
                        menu();
                    }

                    e.Use();
                }
            }

            return foldout;
        }

        internal static bool DrawHeader(Rect rect, string content, bool foldout)
        {
            Texture texture;

            if (foldout)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutDownPath);
            }
            else
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutRightPath);
            }

            if (GUI.Button(rect, "", GUIStyle.none))
            {
                foldout = !foldout;
            }

            Color initialColor = GUI.color;
            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(EditorGUI.IndentedRect(rect), texture);
            GUI.color = initialColor;

            rect.x += EditorGUIUtility.singleLineHeight;
            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(rect, content, labelStyle);

            return foldout;
        }

        public static bool Foldout(bool foldout, string content)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, EditorStyles.foldout);

            if (IsInspector)
            {
                rect.x -= 5;
            }

            Color initialColor = GUI.color;

            if (GUI.Button(EditorGUI.IndentedRect(rect), "", GUIStyle.none))
            {
                foldout = !foldout;
            }

            Texture texture;

            if (foldout)
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutDownPath);
            }
            else
            {
                texture = AssetDatabase.LoadAssetAtPath<Texture>(IMGUIContentPath.FoldoutRightPath);
            }

            GUI.color = EditorColors.Instance.LabelColor;
            GUI.Label(EditorGUI.IndentedRect(rect), texture);
            GUI.color = initialColor;

            rect.x += EditorGUIUtility.singleLineHeight;
            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            labelStyle.normal.textColor = EditorColors.Instance.LabelColor;
            EditorGUI.LabelField(rect, content, labelStyle);

            return foldout;
        }

        public static void Separator()
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(3));
            r = EditorGUI.IndentedRect(r);
            Vector2 start = new(r.min.x, (r.min.y + r.max.y) / 2);
            Vector2 end = new(r.max.x, (r.min.y + r.max.y) / 2);
            Handles.BeginGUI();
            Handles.color = EditorColors.Instance.separatorColor;
            Handles.DrawLine(start, end);
            Handles.EndGUI();
        }

        private static void SetCurrentColorForObjectGUIParameter(bool isObjectNull, out Color GUIcolor)
        {
            if (isObjectNull)
            {
                GUIcolor = EditorColors.Instance.Red.WithAlpha(0.4f);
            }
            else
            {
                GUIcolor = EditorColors.Instance.Green.WithAlpha(0.4f);
            }
        }

        private static void SetButtonColors(ButtonStyle colorSpace, out Color barColor, out Color labelColor)
        {
            switch (colorSpace)
            {
                case ButtonStyle.Add:
                    labelColor = EditorColors.Instance.LabelColor;
                    barColor = EditorColors.Instance.Green.WithAlpha(0.4f);
                    break;
                case ButtonStyle.Remove:
                    labelColor = EditorColors.Instance.LabelColor;
                    barColor = EditorColors.Instance.Red.WithAlpha(0.4f);
                    break;
                default:
                    labelColor = EditorColors.Instance.LabelColor;
                    barColor = EditorColors.Instance.ClickButtonColor;
                    break;
            }
        }

        public static LayerMask LayerMaskField(GUIContent label, LayerMask layerMask)
        {
            Rect rect = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight);
            return LayerMaskField(rect, label, layerMask);
        }

        public static LayerMask LayerMaskField(Rect rect, GUIContent label, LayerMask layerMask)
        {
            Rect fieldRect = CustomEditorGUI.PrefixLabel(rect, label ?? GUIContent.none);
            return CustomEditorGUI.DrawLayerMaskField(fieldRect, layerMask);
        }

        public static LayerMask DrawLayerMaskField(Rect rect, LayerMask layerMask)
        {
            return CustomEditorGUI.DrawLayerMaskField(rect, layerMask);
        }

        public static void DrawHelpBanner(string helpURL, string title = "Help")
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelHelp);
            GUIStyle barStyle = GetStyle(StyleName.ActiveBar);

            Color labelColor = EditorColors.Instance.docsLabelColor;
            Color barColor = EditorColors.Instance.docsButtonColor;

            if (Button(title, labelStyle, barStyle, labelColor, barColor, 25))
            {
                Application.OpenURL(helpURL);
            }
        }

        public static void DrawCustomLabel(string text, bool center = true)
        {
            GUIStyle labelStyle = GetStyle(StyleName.LabelFoldout);
            Color labelColor = EditorColors.Instance.LabelColor;

            DrawCustomLabel(text, labelStyle, labelColor, center);
        }

        private static void DrawCustomLabel(string text, GUIStyle style, Color labelColor, bool center = true)
        {
            if (center)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }

            style.normal.textColor = labelColor;

            GUILayout.Label(text, style);

            if (center)
            {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
        }

        private static GUIStyle GetStyle(string styleName)
        {
            GUIStyle style = Skin.GetStyle(styleName);
            return style;
        }
    }
}
#endif
