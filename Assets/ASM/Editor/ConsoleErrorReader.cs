#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System;
using System.Reflection;
using System.IO;

[InitializeOnLoad]
public class ConsoleErrorReader
{
    static ConsoleErrorReader()
    {
        EditorApplication.delayCall += () =>
        {
            ReadErrors();
        };
    }

    private static void ReadErrors()
    {
        try
        {
            var assembly = Assembly.GetAssembly(typeof(EditorWindow));
            var logEntriesType = assembly.GetType("UnityEditor.LogEntries");
            if (logEntriesType == null) return;

            var getCountMethod = logEntriesType.GetMethod("GetCount", BindingFlags.Static | BindingFlags.Public);
            if (getCountMethod == null) return;
            int count = (int)getCountMethod.Invoke(null, null);

            var getEntryMethod = logEntriesType.GetMethod("GetEntry", BindingFlags.Static | BindingFlags.Public);
            var logEntryType = assembly.GetType("UnityEditor.LogEntry");
            if (getEntryMethod == null || logEntryType == null) return;

            var messageField = logEntryType.GetField("message", BindingFlags.Instance | BindingFlags.Public);
            var fileField = logEntryType.GetField("file", BindingFlags.Instance | BindingFlags.Public);
            var lineField = logEntryType.GetField("line", BindingFlags.Instance | BindingFlags.Public);
            var modeField = logEntryType.GetField("mode", BindingFlags.Instance | BindingFlags.Public);

            var entryInstance = Activator.CreateInstance(logEntryType);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("=== CONSOLE ERRORS DETECTED IN EDITOR ===");
            sb.AppendLine("Time: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sb.AppendLine("Total entries in console: " + count);

            int errorCount = 0;
            for (int i = 0; i < count; i++)
            {
                getEntryMethod.Invoke(null, new object[] { i, entryInstance });

                int mode = (int)modeField.GetValue(entryInstance);
                // Mode checks for errors / asserts / scripting errors (flags: 1, 2, 32, 256)
                bool isError = (mode & (1 | 2 | 32 | 256)) != 0;

                if (isError)
                {
                    errorCount++;
                    string message = (string)messageField.GetValue(entryInstance);
                    string file = (string)fileField.GetValue(entryInstance);
                    int line = (int)lineField.GetValue(entryInstance);
                    sb.AppendLine($"[ERROR #{errorCount}] (Line {line} in {file}):");
                    sb.AppendLine(message);
                    sb.AppendLine("--------------------------------------");
                }
            }

            string dirPath = "/Users/dolehaoanh/.gemini/antigravity/brain/166a8d0e-62fa-41dd-a3a9-0de674cbc7f1/scratch";
            Directory.CreateDirectory(dirPath);
            string filePath = Path.Combine(dirPath, "console_errors.txt");
            File.WriteAllText(filePath, sb.ToString());
            // Debug.Log("[ConsoleErrorReader] Scanned console. Found " + errorCount + " errors. Written to: " + filePath);
        }
        catch (Exception ex)
        {
            Debug.LogError("[ConsoleErrorReader] Exception reading console: " + ex);
        }
    }
}
#endif
