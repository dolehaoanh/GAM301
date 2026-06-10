#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Editor.Groups.EditorColors;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.SelectableColors
{
    public class
        EditorSelectableColorsGenerator : DataGroupAPIGenerator<EditorSelectableColorPalette, EditorSelectableColorInfo>
    {
        protected override void Generate(List<EditorSelectableColorPalette> groups)
        {
            ClassModel classModel = new("GetSelectableColor");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new();
            foreach (EditorSelectableColorPalette group in groups)
            {
                ClassModel nestedClass = new(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                string groupName = "SelectableColorPalette";

                List<Field> fields = new[] {
                    new Field(typeof(EditorSelectableColorPalette), "s_selectableColorPalette")
                        { AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static }
                }.ToList();

                List<Property> properties = new[] {
                    new Property(typeof(EditorSelectableColorPalette), groupName) {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            "s_selectableColorPalette != null? s_selectableColorPalette: s_selectableColorPalette = " +
                            $"{nameof(DataGroupUtility)}.GetGroup<{nameof(EditorSelectableColorPalette)}, {nameof(EditorSelectableColorInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                List<Method> methods = new[] {
                    new Method(typeof(Color), "GetColor") {
                        SingleKeyWord = KeyWord.Static,
                        Parameters =
                            new List<Parameter> { new("ColorName", "colorName"), new("SelectionState", "state") },
                        BodyLines =
                            new List<string> { $"return {groupName}.GetColor(colorName.ToString(), state);" }
                    },
                    new Method(typeof(EditorThemeColor), "GetThemeColor") {
                        SingleKeyWord = KeyWord.Static,
                        Parameters =
                            new List<Parameter> { new("ColorName", "colorName"), new("SelectionState", "state") },
                        BodyLines =
                            new List<string> { $"return {groupName}.GetThemeColor(colorName.ToString(), state);" }
                    },
                    new Method(typeof(EditorSelectableColorInfo), "GetSelectableColorInfo") {
                        SingleKeyWord = KeyWord.Static, AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("ColorName", "colorName") },
                        BodyLines = new List<string>
                            { $"return {groupName}.GetSelectableColorInfo(colorName.ToString());" }
                    }
                }.ToList();

                EnumModel enumModel = new("ColorName");

                foreach (EditorSelectableColorInfo item in group.Items)
                {
                    string privateFieldItemName = $"s_{item.ColorName.ToLowerFirstChar()}";

                    enumModel.EnumValues.Add(new EnumValue(item.ColorName));

                    Field field = new(typeof(EditorSelectableColorInfo), privateFieldItemName)
                        { AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static };

                    Property property = new(typeof(EditorSelectableColorInfo), item.ColorName) {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            $"{privateFieldItemName} ?? ({privateFieldItemName} = GetSelectableColorInfo(ColorName.{item.ColorName}))"
                    };

                    fields.Add(field);
                    properties.Add(property);
                }

                nestedClass.Fields = fields;
                nestedClass.Properties = properties;
                nestedClass.Methods = methods;
                nestedClass.Enums.Add(enumModel);

                nestedClasses.Add(nestedClass);
            }

            classModel.NestedClasses.AddRange(nestedClasses);

            FileModel fileModel = new(classModel.Name);
            fileModel.PreprocessorDirectives.Add("UNITY_EDITOR");
            fileModel.LoadUsingDirectives(
                typeof(Color),
                typeof(EditorSelectableColorPalette),
                typeof(DataGroupUtility),
                typeof(EditorThemeColor),
                typeof(SelectionState),
                typeof(EditorSelectableColorInfo));
            fileModel.SetNamespaceFromFolder(TargetFilePath, "Assets", "Runtime", "API");
            fileModel.Classes.Add(classModel);

            CsGenerator csGenerator = new();
            csGenerator.Files.Add(fileModel);
            csGenerator.Path = TargetFilePath;
            csGenerator.CreateFiles();
        }
    }
}
#endif
