#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Layouts;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Layouts
{
    public class LayoutsGenerator : DataGroupAPIGenerator<LayoutGroup, LayoutInfo>
    {
        protected override void Generate(List<LayoutGroup> groups)
        {
            ClassModel classModel = new("GetLayout");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new();
            foreach (LayoutGroup group in groups)
            {
                ClassModel nestedClass = new(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                string groupName = "LayoutGroup";

                List<Field> fields = new[] {
                    new Field(typeof(LayoutGroup), "s_layoutGroup")
                        { AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static }
                }.ToList();

                List<Property> properties = new[] {
                    new Property(typeof(LayoutGroup), groupName) {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = "s_layoutGroup != null ? s_layoutGroup: s_layoutGroup = " +
                                     $"{nameof(DataGroupUtility)}.GetGroup<{nameof(LayoutGroup)}, {nameof(LayoutInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                List<Method> methods = new[] {
                    new Method(typeof(VisualTreeAsset), "GetVisualTreeAsset") {
                        SingleKeyWord = KeyWord.Static, AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("LayoutName", "layoutName") },
                        BodyLines = new List<string>
                            { $"return {groupName}.GetVisualTreeAsset(layoutName.ToString());" }
                    }
                }.ToList();

                EnumModel enumModel = new("LayoutName");

                foreach (LayoutInfo item in group.Items)
                {
                    enumModel.EnumValues.Add(new EnumValue(item.UxmlReference.name));

                    string privateFieldItemName = $"s_{item.UxmlReference.name.ToLowerFirstChar()}";

                    Field field = new(typeof(VisualTreeAsset), privateFieldItemName)
                        { AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static };

                    Property property = new(typeof(VisualTreeAsset), item.UxmlReference.name) {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = $"{privateFieldItemName} ? " +
                                     $"{privateFieldItemName} : " +
                                     $"{privateFieldItemName} = GetVisualTreeAsset(LayoutName.{item.UxmlReference.name})"
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
            fileModel.LoadUsingDirectives(
                typeof(DataGroupUtility),
                typeof(VisualTreeAsset),
                typeof(LayoutGroup));
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
