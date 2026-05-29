#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public class TestVisualSceneValidator
{
    static TestVisualSceneValidator()
    {
        EditorApplication.delayCall += () =>
        {
            VerifySceneState();
        };
    }

    private static void VerifySceneState()
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("=== RTS VISUAL SCENE VERIFICATION REPORT ===");
        sb.AppendLine("Time: " + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        // 1. Kiểm tra Barracks phe ta
        GameObject playerB = GameObject.Find("Barracks_Player");
        if (playerB != null)
        {
            sb.AppendLine("[OK] Barracks_Player found at " + playerB.transform.position);
            sb.AppendLine("  Rotation: " + playerB.transform.rotation.eulerAngles);
            
            var renderers = playerB.GetComponentsInChildren<Renderer>(true);
            sb.AppendLine("  Renderers Count: " + renderers.Length);
            foreach (var r in renderers)
            {
                if (r == null || r.sharedMaterial == null) continue;
                string matName = r.sharedMaterial.name;
                Color col = r.sharedMaterial.color;
                bool isMinimap = r.name.Contains("Minimap") || r.name.Contains("Icon") || matName.Contains("MinimapIcon") || matName.Contains("Icon");
                sb.AppendLine("    Renderer GO: " + r.name + ", Mat: " + matName + ", Color: " + col + ", isMinimap: " + isMinimap);
            }
        }
        else
        {
            sb.AppendLine("[ERROR] Barracks_Player NOT FOUND!");
        }

        // 2. Kiểm tra Barracks phe địch
        GameObject enemyB = GameObject.Find("Enemy_Barracks");
        if (enemyB != null)
        {
            sb.AppendLine("[OK] Enemy_Barracks found at " + enemyB.transform.position);
            sb.AppendLine("  Rotation: " + enemyB.transform.rotation.eulerAngles);
            
            var renderers = enemyB.GetComponentsInChildren<Renderer>(true);
            sb.AppendLine("  Renderers Count: " + renderers.Length);
            foreach (var r in renderers)
            {
                if (r == null || r.sharedMaterial == null) continue;
                string matName = r.sharedMaterial.name;
                Color col = r.sharedMaterial.color;
                bool isMinimap = r.name.Contains("Minimap") || r.name.Contains("Icon") || matName.Contains("MinimapIcon") || matName.Contains("Icon");
                sb.AppendLine("    Renderer GO: " + r.name + ", Mat: " + matName + ", Color: " + col + ", isMinimap: " + isMinimap);
            }
        }
        else
        {
            sb.AppendLine("[ERROR] Enemy_Barracks NOT FOUND!");
        }

        // 3. Kiểm tra Quads Minimap trên Units
        int totalMinimapIconsChecked = 0;
        int enemyBrightRedMinimapIcons = 0;
        int playerBrightGreenMinimapIcons = 0;

        foreach (var unit in Object.FindObjectsByType<RTSUnit>(FindObjectsInactive.Exclude))
        {
            if (unit == null) continue;
            foreach (var r in unit.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null || r.sharedMaterial == null) continue;
                string matName = r.sharedMaterial.name;
                bool isMinimap = r.name.Contains("Minimap") || r.name.Contains("Icon") || matName.Contains("MinimapIcon") || matName.Contains("Icon");
                if (isMinimap)
                {
                    totalMinimapIconsChecked++;
                    Color col = r.sharedMaterial.color;
                    if (unit.isEnemy)
                    {
                        if (col.r > 0.95f && col.g < 0.05f && col.b < 0.05f) enemyBrightRedMinimapIcons++;
                    }
                    else
                    {
                        if (col.g > 0.95f && col.r < 0.05f && col.b < 0.05f) playerBrightGreenMinimapIcons++;
                    }
                }
            }
        }

        sb.AppendLine("Minimap Icons Verification on Units:");
        sb.AppendLine("  Total Icons Found: " + totalMinimapIconsChecked);
        sb.AppendLine("  Enemy Bright Red Icons (Target: Bright Red): " + enemyBrightRedMinimapIcons);
        sb.AppendLine("  Player Bright Green Icons (Target: Bright Green): " + playerBrightGreenMinimapIcons);

        // Lưu báo cáo
        string dirPath = "/Users/dolehaoanh/.gemini/antigravity/brain/166a8d0e-62fa-41dd-a3a9-0de674cbc7f1/scratch";
        Directory.CreateDirectory(dirPath);
        string filePath = Path.Combine(dirPath, "scene_validation.txt");
        File.WriteAllText(filePath, sb.ToString());
        Debug.Log("[TestVisualSceneValidator] Báo cáo xác minh đã được ghi vào: " + filePath);
    }
}
#endif
