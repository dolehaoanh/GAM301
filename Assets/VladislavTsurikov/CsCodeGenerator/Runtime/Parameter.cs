using System;
using System.Collections.Generic;
using System.Linq;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class Parameter
    {
        public KeyWord? KeyWord { get; set; }

        public BuiltInDataType? BuiltInDataType { get; set; }

        public string CustomDataType { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }

        public string KeyWordFormated => KeyWord?.ToTextLower(" ");

        public string DataTypeFormated =>
            CustomDataType == null ? BuiltInDataType?.ToTextLower(" ") : CustomDataType + " ";

        public string NameValueFormated => string.IsNullOrEmpty(Name) || string.IsNullOrEmpty(Value)
            ? Name + Value
            : Name + " = " + Value;

        public Parameter()
        {
        }

        public Parameter(string value) => Value = value;

        public Parameter(BuiltInDataType builtInDataType, string name)
        {
            BuiltInDataType = builtInDataType;
            Name = name;
        }

        public Parameter(Type type, string name)
        {
            CustomDataType = type.ToString().Split('.').Last();
            ;
            Name = name;
        }

        public Parameter(string type, string name)
        {
            CustomDataType = type;
            Name = name;
        }

        public override string ToString()
        {
            return KeyWordFormated + DataTypeFormated + NameValueFormated;
        }
    }

    public static class ParameterExtensions
    {
        public static string ToStringList(this List<Parameter> parameters)
        {
            string parametersString = string.Join(", ", parameters);
            string result = $"({parametersString})";
            return result;
        }
    }
}
