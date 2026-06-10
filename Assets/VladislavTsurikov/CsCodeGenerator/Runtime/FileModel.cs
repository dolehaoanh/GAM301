using System;
using System.Collections.Generic;
using System.Text;

namespace VladislavTsurikov.CsCodeGenerator.Runtime
{
    public class FileModel
    {
        public bool WarningGeneratedFile = true;

        private List<string> UsingDirectives { get; } = new();

        public List<string> PreprocessorDirectives { get; set; } = new();

        public string Namespace { get; set; }

        public string Name { get; set; }

        public string Extension { get; set; } = Constants.CsExtension;

        public string FullName => Name + "." + Extension;

        public List<EnumModel> Enums { get; set; } = new();

        public List<ClassModel> Classes { get; set; } = new();

        public FileModel(string name) => Name = name;

        public override string ToString()
        {
            string result = WarningGeneratedFile ? GetWarningGeneratedFileText() : "";

            foreach (string preprocessorDirective in PreprocessorDirectives)
            {
                result += "#if " + preprocessorDirective + Constants.NewLine;
            }

            string usingText = UsingDirectives.Count > 0 ? Constants.Using + " " : "";
            result += usingText + string.Join(Constants.NewLine + usingText, UsingDirectives);
            result += Constants.NewLineDouble + Constants.Namespace + " " + Namespace;
            result += Constants.NewLine + "{";
            result += string.Join(Constants.NewLine, Enums);
            result += Enums.Count > 0 && Classes.Count > 0 ? Constants.NewLine : "";
            result += string.Join(Constants.NewLine, Classes);
            result += Constants.NewLine + "}";
            result += Constants.NewLine;

            foreach (string preprocessorDirective in PreprocessorDirectives)
            {
                result += "#endif" + Constants.NewLine;
            }

            return result;
        }

        public void LoadUsingDirectives(List<string> usingDirectives)
        {
            foreach (string usingDirective in usingDirectives)
            {
                UsingDirectives.Add(usingDirective + ";");
            }
        }

        public void LoadUsingDirectives(params Type[] types)
        {
            foreach (string usingDirective in NamespaceUtility.GetUsingDirectives(types))
            {
                UsingDirectives.Add(usingDirective + ";");
            }
        }

        public void SetNamespaceFromFolder(string path, params string[] ignoredFolders)
        {
            Namespace =
                NamespaceUtility.GetNamespaceFromPath(path, "VladislavTsurikov", ignoredFolders);
        }

        private string GetWarningGeneratedFileText()
        {
            StringBuilder namesStringBuilder = new();

            namesStringBuilder.AppendLine("//.........................");
            namesStringBuilder.AppendLine("//.....Generated File......");
            namesStringBuilder.AppendLine("//.........................");
            namesStringBuilder.AppendLine("//.......Do not edit.......");
            namesStringBuilder.AppendLine("//.........................");

            return namesStringBuilder + Constants.NewLine;
        }
    }
}
