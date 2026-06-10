using System.Collections.Generic;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class AttributeModel
    {
        public string Name { get; set; }

        public List<Parameter> Parameters { get; set; } = new();

        public Parameter SingleParameter
        {
            set => Parameters.Add(value);
        }

        public AttributeModel(string name = null) => Name = name;

        public override string ToString()
        {
            string parametersString = Parameters.Count > 0 ? Parameters.ToStringList() : "";
            string result = $"[{Name}{parametersString}]";
            return result;
        }
    }

    public static class AttributeModelExtensions
    {
        public static string ToStringList(this List<AttributeModel> attributes, string indent)
        {
            string result = attributes.Count > 0
                ? Constants.NewLine + indent + string.Join(Constants.NewLine + indent, attributes)
                : "";
            return result;
        }
    }
}
