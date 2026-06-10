#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using VladislavTsurikov.Core.Editor;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.MegaWorld.Runtime.Core.SelectionDatas.Group.Prototypes.PrototypeTerrainTexture;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem
{
    [ElementEditor(typeof(TexturesFilter))]
    public class TexturesFilterEditor : ReorderableListComponentEditor
    {
        private readonly int _сheckTexturesIconHeight = 60;

        private readonly int _сheckTexturesIconWidth = 60;
        private readonly float _сheckTexturesWindowHeight = 140.0f;
        private Vector2 _сheckTexturesWindowsScroll = Vector2.zero;
        private TexturesFilter _texturesFilter;

        public override void OnEnable()
        {
            _texturesFilter = (TexturesFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            if (Terrain.activeTerrain == null)
            {
                CustomEditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    "There is no terrain in the scene", MessageType.Warning);

                rect.y += EditorGUIUtility.singleLineHeight;
                return;
            }

            Event e = Event.current;

            Color initialGUIColor = GUI.backgroundColor;

            if (index != 0)
            {
                _texturesFilter.BlendMode = (BlendMode)CustomEditorGUI.EnumPopup(
                    new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                    new GUIContent("Blend Mode"), _texturesFilter.BlendMode);
                rect.y += EditorGUIUtility.singleLineHeight;
                rect.y += EditorGUIUtility.singleLineHeight;
            }

            Rect windowRect = new(rect.x, rect.y, rect.width, _сheckTexturesWindowHeight);

            Rect virtualRect = new(windowRect);

            if (!_texturesFilter.IsSyncTerrain(Terrain.activeTerrain))
            {
                _texturesFilter.UpdateCheckTerrainTextures(Terrain.activeTerrain);
            }

            if (IsNecessaryToDrawIconsForCheckTerrainTextures(windowRect, initialGUIColor,
                    _texturesFilter.TerrainTextureList))
            {
                DrawLabelForIcons(initialGUIColor, windowRect);
                DrawCheckTerrainTexturesIcons(e, windowRect);
            }

            switch (e.type)
            {
                case EventType.ContextClick:
                {
                    if (virtualRect.Contains(e.mousePosition))
                    {
                        CheckTerrainTexturesWindowMenu().ShowAsContext();
                        e.Use();
                    }

                    break;
                }
            }

            GUILayout.Space(3);
        }

        public override float GetElementHeight(int index)
        {
            if (Terrain.activeTerrain == null)
            {
                return EditorGUIUtility.singleLineHeight * 2;
            }

            if (index != 0)
            {
                return EditorGUIUtility.singleLineHeight * 12;
            }

            return EditorGUIUtility.singleLineHeight * 11;
        }

        private void DrawCheckTerrainTexturesIcons(Event e, Rect windowRect)
        {
            List<TerrainTexture> checkTerrainTextures = _texturesFilter.TerrainTextureList;

            Rect virtualRect = GetVirtualRect(windowRect, checkTerrainTextures.Count, _сheckTexturesIconWidth,
                _сheckTexturesIconHeight);

            Vector2 brushWindowScrollPos = _сheckTexturesWindowsScroll;
            brushWindowScrollPos = GUI.BeginScrollView(windowRect, brushWindowScrollPos, virtualRect, false, true);

            int y = (int)virtualRect.yMin;
            int x = (int)virtualRect.xMin;

            for (int i = 0; i < checkTerrainTextures.Count; i++)
            {
                Rect brushIconRect = new(x, y, _сheckTexturesIconWidth, _сheckTexturesIconHeight);

                Color rectColor;

                if (checkTerrainTextures[i].Selected)
                {
                    rectColor = EditorColors.Instance.ToggleButtonActiveColor;
                }
                else
                {
                    rectColor = Color.white;
                }

                DrawIconRectForCheckTerrainTextures(brushIconRect, checkTerrainTextures[i].Texture, rectColor, e,
                    windowRect, brushWindowScrollPos, () => { HandleSelectCheckTerrainTextures(i, e); });

                SetNextXYIcon(virtualRect, _сheckTexturesIconWidth, _сheckTexturesIconHeight, ref y, ref x);
            }

            _сheckTexturesWindowsScroll = brushWindowScrollPos;

            GUI.EndScrollView();
        }

        private void DrawIconRectForCheckTerrainTextures(Rect brushIconRect, Texture2D preview, Color rectColor,
            Event e, Rect brushWindowRect, Vector2 brushWindowScrollPos, UnityAction clickAction)
        {
            Rect brushIconRectScrolled = new(brushIconRect);
            brushIconRectScrolled.position -= brushWindowScrollPos;

            // only visible incons
            if (brushIconRectScrolled.Overlaps(brushWindowRect))
            {
                if (brushIconRect.Contains(e.mousePosition))
                {
                    clickAction.Invoke();
                }

                EditorGUI.DrawRect(brushIconRect, rectColor);

                // Prefab preview 
                if (e.type == EventType.Repaint)
                {
                    Rect previewRect = new(brushIconRect.x + 2, brushIconRect.y + 2, brushIconRect.width - 4,
                        brushIconRect.width - 4);

                    if (preview != null)
                    {
                        EditorGUI.DrawPreviewTexture(previewRect, preview);
                    }
                    else
                    {
                        Color dimmedColor = new(0.4f, 0.4f, 0.4f, 1.0f);
                        EditorGUI.DrawRect(previewRect, dimmedColor);
                    }
                }
            }
        }

        private void HandleSelectCheckTerrainTextures(int index, Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                {
                    GUI.changed = true;

                    if (e.button == 0)
                    {
                        if (e.control)
                        {
                            SelectCheckTerrainTextureAdditive(index);
                        }
                        else if (e.shift)
                        {
                            SelectCheckTerrainTextureRange(index);
                        }
                        else
                        {
                            SelectCheckTerrainTexture(index);
                        }

                        e.Use();
                    }

                    break;
                }
            }
        }

        public void SelectCheckTerrainTexture(int index)
        {
            SetSelectedAllCheckTerrainTexture(false);

            if (index < 0 && index >= _texturesFilter.TerrainTextureList.Count)
            {
                return;
            }

            _texturesFilter.TerrainTextureList[index].Selected = true;
        }

        public void SelectCheckTerrainTextureAdditive(int index)
        {
            if (index < 0 && index >= _texturesFilter.TerrainTextureList.Count)
            {
                return;
            }

            _texturesFilter.TerrainTextureList[index].Selected = !_texturesFilter.TerrainTextureList[index].Selected;
        }

        public void SelectCheckTerrainTextureRange(int index)
        {
            if (index < 0 && index >= _texturesFilter.TerrainTextureList.Count)
            {
                return;
            }

            int rangeMin = index;
            int rangeMax = index;

            for (int i = 0; i < _texturesFilter.TerrainTextureList.Count; i++)
            {
                if (_texturesFilter.TerrainTextureList[i].Selected)
                {
                    rangeMin = Mathf.Min(rangeMin, i);
                    rangeMax = Mathf.Max(rangeMax, i);
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++)
            {
                if (!_texturesFilter.TerrainTextureList[i].Selected)
                {
                    break;
                }
            }

            for (int i = rangeMin; i <= rangeMax; i++)
            {
                _texturesFilter.TerrainTextureList[i].Selected = true;
            }
        }

        public void SetSelectedAllCheckTerrainTexture(bool select)
        {
            foreach (TerrainTexture checkTerrainTextures in _texturesFilter.TerrainTextureList)
            {
                checkTerrainTextures.Selected = select;
            }
        }

        private void DrawLabelForIcons(Color initialGUIColor, Rect windowRect, string text = null)
        {
            GUIStyle labelTextForSelectedArea = CustomEditorGUILayout.GetStyle(StyleName.LabelTextForSelectedArea);
            GUIStyle boxStyle = CustomEditorGUILayout.GetStyle(StyleName.Box);

            GUI.color = EditorColors.Instance.boxColor;
            GUI.Label(windowRect, "", boxStyle);
            GUI.color = initialGUIColor;

            if (text != null)
            {
                CustomEditorGUI.LabelField(windowRect, text, labelTextForSelectedArea);
            }
        }

        private bool IsNecessaryToDrawIconsForCheckTerrainTextures(Rect brushWindowRect, Color initialGUIColor,
            List<TerrainTexture> checkTerrainTextures)
        {
            if (checkTerrainTextures.Count == 0)
            {
                DrawLabelForIcons(initialGUIColor, brushWindowRect, "Missing textures on terrain");
                return false;
            }

            return true;
        }

        private Rect GetVirtualRect(Rect brushWindowRect, int count, int iconWidth, int iconHeight)
        {
            Rect virtualRect = new(brushWindowRect);
            {
                virtualRect.width = Mathf.Max(virtualRect.width - 20, 1); // space for scroll 

                int presetColumns = Mathf.FloorToInt(Mathf.Max(1, virtualRect.width / iconWidth));
                int virtualRows = Mathf.CeilToInt((float)count / presetColumns);

                virtualRect.height = Mathf.Max(virtualRect.height, iconHeight * virtualRows);
            }

            return virtualRect;
        }

        private void SetNextXYIcon(Rect virtualRect, int iconWidth, int iconHeight, ref int y, ref int x)
        {
            if (x + iconWidth < (int)virtualRect.xMax - iconWidth)
            {
                x += iconWidth;
            }
            else if (y < (int)virtualRect.yMax)
            {
                y += iconHeight;
                x = (int)virtualRect.xMin;
            }
        }

        private GenericMenu CheckTerrainTexturesWindowMenu()
        {
            GenericMenu menu = new();

            menu.AddItem(new GUIContent("Update Check Terrain Textures"), false, ContextMenuCallback,
                new Action(() => _texturesFilter.UpdateCheckTerrainTextures(Terrain.activeTerrain)));

            return menu;
        }

        private void ContextMenuCallback(object obj)
        {
            if (obj != null && obj is Action)
            {
                (obj as Action).Invoke();
            }
        }
    }
}
#endif
