#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using VladislavTsurikov.CsCodeGenerator.Runtime;
using VladislavTsurikov.UIElementsUtility.Editor.Core;
using VladislavTsurikov.UIElementsUtility.Runtime.Core.Utility;
using VladislavTsurikov.UIElementsUtility.Runtime.Groups.Styles;

namespace VladislavTsurikov.UIElementsUtility.Editor.Groups.Styles
{
    public class StylesGenerator : DataGroupAPIGenerator<StyleGroup, StyleInfo>
    {
        protected override void Generate(List<StyleGroup> groups)
        {
            ClassModel classModel = new("GetStyle");
            classModel.SingleKeyWord = KeyWord.Static;
            List<ClassModel> nestedClasses = new();
            foreach (StyleGroup group in groups)
            {
                ClassModel nestedClass = new(group.GroupName);
                nestedClass.SingleKeyWord = KeyWord.Static;

                string groupName = "StyleGroup";

                List<Field> fields = new[] {
                    new Field(typeof(StyleGroup), "s_styleGroup")
                        { AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static }
                }.ToList();

                List<Property> properties = new[] {
                    new Property(typeof(StyleGroup), groupName) {
                        AccessModifier = AccessModifier.Private,
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody = "s_styleGroup != null? s_styleGroup: s_styleGroup = " +
                                     $"{nameof(DataGroupUtility)}.GetGroup<{nameof(StyleGroup)}, {nameof(StyleInfo)}>(\"{group.GroupName}\")"
                    }
                }.ToList();

                List<Method> methods = new[] {
                    new Method(typeof(StyleSheet), "GetStyleSheet") {
                        SingleKeyWord = KeyWord.Static, AccessModifier = AccessModifier.Private,
                        Parameters = new List<Parameter> { new("StyleName", "styleName") },
                        BodyLines = new List<string> { $"return {groupName}.GetStyleSheet(styleName.ToString());" }
                    }
                }.ToList();

                EnumModel enumModel = new("StyleName");

                foreach (StyleInfo style in group.Items)
                {
                    enumModel.EnumValues.Add(new EnumValue(style.UssReference.name));
                    string privateFieldItemName = $"s_{style.UssReference.name.ToLowerFirstChar()}";

                    Field field = new(typeof(StyleSheet), privateFieldItemName)
                        { AccessModifier = AccessModifier.Private, SingleKeyWord = KeyWord.Static };

                    Property property = new(typeof(StyleSheet), style.UssReference.name) {
                        SingleKeyWord = KeyWord.Static,
                        IsGetOnly = true,
                        GetterBody =
                            $"{privateFieldItemName} ? {privateFieldItemName} : {privateFieldItemName} = GetStyleSheet(StyleName.{style.UssReference.name})"
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
                typeof(StyleGroup),
                typeof(StyleSheet),
                typeof(DataGroupUtility));
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
